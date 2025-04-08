using UnityEngine;

public class BossRoomManager : MonoBehaviour
{
    [Header("Triggers à désactiver")]
    public RoomTransitionTrigger[] triggersToDisable;

    [Header("Colliders à activer pour bloquer le joueur")]
    public Collider2D[] playerBarriers;

    [Header("Nom du niveau à marquer comme terminé")]
    public string levelID = "Level_2";

    [Header("Référence au boss")]
    public EnemyHealth bossHealth;

    private bool bossDefeated = false;

    private void Start()
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
            trigger.enabled = false;

        foreach (var barrier in playerBarriers)
            barrier.enabled = true;
    }

    private void UnlockRoom()
    {
        foreach (var trigger in triggersToDisable)
            trigger.enabled = true;

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
            var claimed = SaveManager.Instance.CurrentSave.progression.levelsClaimed;

            if (levelID == "Level_2") claimed.Level_2 = true;
            else if (levelID == "Level_1") claimed.Level_1 = true;
            else if (levelID == "Level_3") claimed.Level_3 = true;
            else if (levelID == "Level_4") claimed.Level_4 = true;
            else if (levelID == "Level_5") claimed.Level_5 = true;

            SaveManager.Instance.SaveGame();
        }
    }
}
