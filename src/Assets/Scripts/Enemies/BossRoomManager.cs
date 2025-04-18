using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System;

public class BossRoomManager : MonoBehaviour
{
    [Header("Triggers à désactiver")]
    public MonoBehaviour[] triggersToDisableRaw;

    private ITriggerDesactivable[] triggersToDisable;

    [Header("Colliders à activer pour bloquer le joueur")]
    public Collider2D[] playerBarriers;

    [Header("Nom du niveau à marquer comme terminé")]
    public string levelID = "Level_2";

    [Header("Référence au boss")]
    public EnemyHealth bossHealth;

    [Header("Message de récompense"), TextArea(2, 4)]
    public string rewardMessage = "";

    [Header("Capacité à débloquer en battant le boss")]
    public AbilityName abilityToUnlock = AbilityName.None;

    [Header("Touches pour fermer le message")]
    [SerializeField] private KeyCode[] skipKeys = { KeyCode.Return };

    public enum AbilityName
    {
        None,
        Dash,
        DoubleJump,
        Grapple,
        Sword,
        PollenVortex,
        WaterJet
    }

    private static readonly Dictionary<AbilityName, string> abilityFieldMap = new()
    {
        { AbilityName.Dash, "hasDash" },
        { AbilityName.DoubleJump, "hasDoubleJump" },
        { AbilityName.Grapple, "hasGrapple" },
        { AbilityName.Sword, "hasSword" },
        { AbilityName.PollenVortex, "hasPollenVortex" },
        { AbilityName.WaterJet, "hasWaterJet" }

    };


    private bool bossDefeated = false;

    private void Awake()
    {
        triggersToDisable = Array.ConvertAll(triggersToDisableRaw, item => item as ITriggerDesactivable);
    }

    private void Start()
    {
        foreach (var barrier in playerBarriers)
        {
            barrier.enabled = false;
        }

        foreach (var trigger in triggersToDisable)
            trigger?.SetEnabled(true);
    }

    public void ActivateBossRoom()
    {
        LockRoom();

        if (bossHealth != null)
        {
            bossHealth.OnDeath += HandleBossDeath;
        }
    }

    private void LockRoom()
    {
        foreach (var trigger in triggersToDisable)
            trigger?.SetEnabled(false);

        foreach (var barrier in playerBarriers)
            barrier.enabled = true;
    }

    private void UnlockRoom()
    {
        foreach (var trigger in triggersToDisable)
            trigger?.SetEnabled(true);

        foreach (var barrier in playerBarriers)
            barrier.enabled = false;
    }

    private void HandleBossDeath()
    {
        if (bossDefeated) return;

        bossDefeated = true;
        UnlockRoom();

        if (SaveManager.Instance != null)
        {
            var save = SaveManager.Instance.CurrentSave;

            var claimed = save.progression.levelsClaimed;
            if (levelID == "Level_2") claimed.Level_2 = true;
            else if (levelID == "Level_1") claimed.Level_1 = true;
            else if (levelID == "Level_3") claimed.Level_3 = true;
            else if (levelID == "Level_4") claimed.Level_4 = true;
            else if (levelID == "Level_5") claimed.Level_5 = true;

            if (abilityToUnlock != AbilityName.None && abilityFieldMap.TryGetValue(abilityToUnlock, out string fieldName))
            {
                var abilityField = typeof(AbilityData).GetField(fieldName);
                if (abilityField != null && abilityField.FieldType == typeof(bool))
                {
                    abilityField.SetValue(save.abilities, true);
                }
            }

            SaveManager.Instance.SaveGame();
            SaveManager.Instance.TriggerAbilitiesUpdated();
            SaveManager.Instance.TriggerLevelClaimed(levelID);

            if (!string.IsNullOrWhiteSpace(rewardMessage))
                MessageSpawner.Instance?.DisplayMessageWithPause(rewardMessage);
        }
    }
}
