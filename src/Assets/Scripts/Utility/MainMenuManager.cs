using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Level_1";

    public void StartGame()
    {
        SceneTransitionManager.Instance.LoadSceneWithFade(sceneToLoad);
    }
}
