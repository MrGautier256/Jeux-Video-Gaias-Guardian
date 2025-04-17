using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MessageTrigger : MonoBehaviour
{
    [Header("Message a afficher"), TextArea(2, 4)]
    [SerializeField] private string message;

    [Header("Touches pour fermer le message")]
    [SerializeField] private KeyCode[] skipKeys = { KeyCode.Return };

    [Header("Afficher une seule fois ?")]
    [SerializeField] private bool triggerOnce = true;

    [Header("Afficher meme dans les niveaux clear (ex: level_1_clear)")]
    [SerializeField] private bool showOnClearLevel = false;

    [SerializeField] private TMP_FontAsset overrideFont;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        if (!collision.CompareTag("Player")) return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (!showOnClearLevel && currentScene.Contains("_clear")) return;

        MessageSpawner.Instance?.DisplayMessageWithPause(message, skipKeys);

        if (triggerOnce)
            triggered = true;
    }
}