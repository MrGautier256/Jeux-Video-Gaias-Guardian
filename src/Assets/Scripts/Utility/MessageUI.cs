using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private float displayDuration;
    [SerializeField] private AudioClip blipSound;


    private IEnumerator TypeMessage(string message)
    {
        messageText.text = "";
        foreach (char c in message)
        {
            messageText.text += c;
            if (blipSound != null)
            {
                AudioSource.PlayClipAtPoint(blipSound, Camera.main.transform.position, 0.5f);
            }
            yield return new WaitForSeconds(0.05f); //
        }

        yield return new WaitForSecondsRealtime(displayDuration);
        Destroy(gameObject);
    }

    public void Show(string message)
    {
        // Calcule une durée selon la taille du message
        displayDuration = Mathf.Clamp(1f + (message.Length * 0.05f), 2f, 6f);
        StartCoroutine(TypeMessage(message)); gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration + 1.5f);
        Destroy(gameObject);
    }
}
