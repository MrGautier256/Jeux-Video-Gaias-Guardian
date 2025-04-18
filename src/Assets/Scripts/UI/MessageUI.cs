using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


public class MessageUI : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private float displayDuration;
    [SerializeField] private AudioClip blipSound;
    [SerializeField] private float minDisplayTime = 1f; 

    private float timeSinceShown = 0f;

    private bool isAwaitingInput = false;
    private bool messageFullyDisplayed = false;



    public void ShowWithPause(string message)
    {
        messageFullyDisplayed = false;
        messageText.text = message;
        StartCoroutine(TypeMessage(message));
        gameObject.SetActive(true);
        isAwaitingInput = true;
        timeSinceShown = 0f;
        Time.timeScale = 0f;
    }
    private void Update()
    {
        if (!isAwaitingInput) return;

        timeSinceShown += Time.unscaledDeltaTime;
        if (timeSinceShown < minDisplayTime || !messageFullyDisplayed) return;

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            CloseMessage();
            return;
        }

        if (Gamepad.current != null)
        {
            foreach (var control in Gamepad.current.allControls)
            {
                if (control is ButtonControl button && button.wasPressedThisFrame)
                {
                    CloseMessage();
                    return;
                }
            }
        }

        if (Mouse.current != null &&
            (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame))
        {
            CloseMessage();
        }
    }



    private void CloseMessage()
    {
        isAwaitingInput = false;
        Time.timeScale = 1f;
        Destroy(gameObject);
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
            yield return new WaitForSecondsRealtime(0.05f);

        }
        messageFullyDisplayed = true;
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
