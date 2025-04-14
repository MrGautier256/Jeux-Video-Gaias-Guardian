using UnityEngine;

public class GlobalManagerAutoLoader : MonoBehaviour
{
    [Header("Prefabs à instancier s’ils sont absents")]
    public GameObject sceneTransitionManagerPrefab;
    public GameObject musicControllerPrefab;
    public GameObject saveManagerPrefab;
    public GameObject pauseMenuPrefab;
    public GameObject HUDPrefab;


    private void Awake()
    {
        LoadIfMissing<SceneTransitionManager>(sceneTransitionManagerPrefab);
        LoadIfMissing<MusicController>(musicControllerPrefab);
        LoadIfMissing<SaveManager>(saveManagerPrefab);
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu")
        {
            LoadIfMissing<PauseMenu>(pauseMenuPrefab);
            LoadIfMissing<PlayerHUD>(HUDPrefab);
        }

    }

    private void LoadIfMissing<T>(GameObject prefab) where T : MonoBehaviour
    {
        if (Object.FindAnyObjectByType<T>() != null) return;

        if (prefab != null)
        {
            Instantiate(prefab);
        }
        else
        {
            Debug.LogWarning($"[AutoLoader] Prefab manquant pour le type {typeof(T).Name} !");
        }
    }
}
