using UnityEngine;
using TMPro;

public class MessageSpawner : MonoBehaviour
{
    public static MessageSpawner Instance;

    [Header("Référence du prefab")]
    public GameObject messageUIPrefab;

    [Header("Canvas parent (UI)")]
    public Transform canvasParent;

    [Header("Police par défaut")]
    public TMP_FontAsset defaultFont;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void DisplayMessage(string message, TMP_FontAsset customFont = null)
    {
        GameObject instance = Instantiate(messageUIPrefab, canvasParent);
        MessageUI messageUI = instance.GetComponent<MessageUI>();
        if (messageUI != null)
        {
            ApplyFont(messageUI, customFont);
            messageUI.Show(message);
        }
    }

    public void DisplayMessageWithPause(string message, KeyCode[] skipKeys, TMP_FontAsset customFont = null)
    {
        GameObject instance = Instantiate(messageUIPrefab, canvasParent);
        MessageUI messageUI = instance.GetComponent<MessageUI>();
        if (messageUI != null)
        {
            ApplyFont(messageUI, customFont);
            messageUI.ShowWithPause(message, skipKeys);
        }
    }

    private void ApplyFont(MessageUI messageUI, TMP_FontAsset font)
    {
        TMP_Text textComponent = messageUI.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.font = font != null ? font : defaultFont;
        }
    }
}
