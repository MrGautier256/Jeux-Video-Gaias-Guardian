using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class PowerCollectible : MonoBehaviour
{
    public enum AbilityName
    {
        None,
        Dash,
        DoubleJump,
        Grapple,
        Sword
    }

    [Header("Capacité à débloquer")]
    public AbilityName abilityToUnlock = AbilityName.None;

    private static readonly Dictionary<AbilityName, string> abilityFieldMap = new()
    {
        { AbilityName.Dash, "hasDash" },
        { AbilityName.DoubleJump, "hasDoubleJump" },
        { AbilityName.Grapple, "hasGrapple" },
        { AbilityName.Sword, "hasSword" }
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected || abilityToUnlock == AbilityName.None) return;

        if (collision.CompareTag("Player"))
        {
            collected = true;

            spriteRenderer.enabled = false;
            GetComponent<Collider2D>().enabled = false;

            if (audioSource != null && collectSound != null)
            {
                audioSource.clip = collectSound; 
                audioSource.Play();
                Destroy(gameObject, collectSound.length); 
            }
            else
            {
                Destroy(gameObject);
            }

            UnlockAbility();
        }
    }

    private void UnlockAbility()
    {
        if (SaveManager.Instance == null) return;

        if (abilityFieldMap.TryGetValue(abilityToUnlock, out string fieldName))
        {
            var save = SaveManager.Instance.CurrentSave;
            var abilityField = typeof(AbilityData).GetField(fieldName);

            if (abilityField != null && abilityField.FieldType == typeof(bool))
            {
                abilityField.SetValue(save.abilities, true);

                SaveManager.Instance.SaveGame();
                SaveManager.Instance.TriggerAbilitiesUpdated();
                AbilityData a = SaveManager.Instance.CurrentSave.abilities;
            }
        }
    }
}
