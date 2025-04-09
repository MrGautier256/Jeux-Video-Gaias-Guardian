using UnityEngine;

public class MessageSpawner : MonoBehaviour
{
    public static MessageSpawner Instance;

    [Header("Référence du prefab")]
    public GameObject messageUIPrefab;

    [Header("Canvas parent (UI)")]
    public Transform canvasParent;

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

    public void DisplayMessage(string message)
    {
        GameObject instance = Instantiate(messageUIPrefab, canvasParent);
        MessageUI messageUI = instance.GetComponent<MessageUI>();
        if (messageUI != null)
        {
            messageUI.Show(message);
        }
    }
}
