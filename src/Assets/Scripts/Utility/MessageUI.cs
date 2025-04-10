using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private float displayDuration = 3f;

    public void Show(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        Destroy(gameObject);
    }
}
