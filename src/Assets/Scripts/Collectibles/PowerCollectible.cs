using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class PowerCollectible : MonoBehaviour
{
    [Header("Capacite a debloquer")]
    public AbilityName abilityToUnlock = AbilityName.None;

    [Header("Message de recompense"), TextArea(2, 4)]
    public string rewardMessage = "";

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

    private bool collected = false;

    [Header("Audio")]
    [SerializeField] private AudioClip collectSound; 

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (IsAbilityUnlocked(abilityToUnlock))
        {
            Destroy(gameObject);
            return;

        }
    }

    private void HandleMessageAndAbility()
    {
        if (!string.IsNullOrWhiteSpace(rewardMessage))
            MessageSpawner.Instance?.DisplayMessageWithPause(rewardMessage);

        UnlockAbility();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        if (collision.CompareTag("Player"))
        {
            collected = true;

            spriteRenderer.enabled = false;
            GetComponent<Collider2D>().enabled = false;

            if (audioSource != null && collectSound != null)
            {
                audioSource.clip = collectSound;
                audioSource.Play();
                Invoke(nameof(HandleMessageAndAbility), collectSound.length);
                Destroy(gameObject, collectSound.length);
            }
            else
            {
                HandleMessageAndAbility();
                Destroy(gameObject);
            }
        }
    }

    private void UnlockAbility()
    {
        if (SaveManager.Instance == null || abilityToUnlock == AbilityName.None) return;

        if (IsAbilityUnlocked(abilityToUnlock)) return;

        if (abilityFieldMap.TryGetValue(abilityToUnlock, out string fieldName))
        {
            var abilityField = typeof(AbilityData).GetField(fieldName);
            if (abilityField != null && abilityField.FieldType == typeof(bool))
            {
                var save = SaveManager.Instance.CurrentSave;
                abilityField.SetValue(save.abilities, true);

                SaveManager.Instance.SaveGame();
                SaveManager.Instance.TriggerAbilitiesUpdated();
            }
        }
    }

    private bool IsAbilityUnlocked(AbilityName ability)
    {
        if (SaveManager.Instance == null || ability == AbilityName.None) return false;

        if (abilityFieldMap.TryGetValue(ability, out string fieldName))
        {
            var abilityField = typeof(AbilityData).GetField(fieldName);
            if (abilityField != null && abilityField.FieldType == typeof(bool))
            {
                var save = SaveManager.Instance.CurrentSave;
                return (bool)abilityField.GetValue(save.abilities);
            }
        }

        return false;
    }

}
