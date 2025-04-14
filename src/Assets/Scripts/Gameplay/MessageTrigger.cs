using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    [Header("Message a afficher"), TextArea(2, 4)]
    [SerializeField] private string message;

    [Header("Touches pour fermer le message")]
    [SerializeField] private KeyCode[] skipKeys = { KeyCode.Return };

    [Header("Afficher une seule fois ?")]
    [SerializeField] private bool triggerOnce = true;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            MessageSpawner.Instance?.DisplayMessageWithPause(message, skipKeys);


            if (triggerOnce)
                triggered = true;
        }
    }
}
