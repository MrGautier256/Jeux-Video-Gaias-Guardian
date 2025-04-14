using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private float displayDuration;
    [SerializeField] private AudioClip blipSound;

    private bool isAwaitingInput = false;
    private KeyCode[] skipKeys = { KeyCode.Return }; 


    public void ShowWithPause(string message, KeyCode[] keysToSkip)
    {
        messageText.text = message;
        skipKeys = keysToSkip;
        StartCoroutine(TypeMessage(message));
        gameObject.SetActive(true);

        isAwaitingInput = true;
    }
    private void Update()
    {
        if (!isAwaitingInput) return;

        foreach (KeyCode key in skipKeys)
        {
            if (Input.GetKeyDown(key))
            {
                isAwaitingInput = false;
                Time.timeScale = 1f;
                Destroy(gameObject);
                break;
            }
        }
    }

    private IEnumerator TypeMessage(string message)
    {
        messageText.text = "";
        int counter = 0;
        foreach (char c in message)
        {
            messageText.text += c;
            counter++;
            if (!char.IsWhiteSpace(c) && counter % 2 == 0)
            {
                AudioSource.PlayClipAtPoint(blipSound, Camera.main.transform.position, 0.2f);
            }
            yield return new WaitForSeconds(0.05f); //
        }

        yield return new WaitForSecondsRealtime(displayDuration);
        Destroy(gameObject);
    }

    public void Show(string message)
    {
        displayDuration = Mathf.Clamp(1f + (message.Length * 0.05f), 2f, 6f);
        StartCoroutine(TypeMessage(message));
        gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration + 1.5f);
        Destroy(gameObject);
    }
}
