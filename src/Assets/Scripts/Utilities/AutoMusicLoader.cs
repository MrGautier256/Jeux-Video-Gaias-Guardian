using UnityEngine;

public class AutoMusicLoader : MonoBehaviour
{
    private static bool loaded = false;
    [SerializeField] private GameObject musicControllerPrefab;

    private void Awake()
    {
        if (!loaded)
        {
            DontDestroyOnLoad(Instantiate(musicControllerPrefab));
            loaded = true;
        }
    }
}
