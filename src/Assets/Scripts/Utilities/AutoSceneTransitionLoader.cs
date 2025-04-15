using UnityEngine;

public class AutoSceneTransitionLoader : MonoBehaviour
{
    private static bool isLoaded = false;
    [SerializeField] private GameObject sceneTransitionPrefab;

    private void Awake()
    {
        if (!isLoaded)
        {
            DontDestroyOnLoad(Instantiate(sceneTransitionPrefab));
            isLoaded = true;
        }
    }
}
