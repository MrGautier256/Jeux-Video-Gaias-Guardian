using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject deleteSaveButton;

    private void Start()
    {
       
    }

    public void StartNewGame()
    {
        SaveManager.Instance.ResetSave();
        SaveManager.Instance.SaveGame();
        SceneTransitionManager.Instance.LoadSceneWithFade("Level_1");
    }


    public void ContinueGame()
    {
        string levelToLoad = SaveManager.Instance.CurrentSave.progression.currentLevel;
        SceneTransitionManager.Instance.LoadSceneWithFade(levelToLoad);
    }

    public void Credits()
    {
        SceneTransitionManager.Instance.LoadSceneWithFade("Credits");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
    

}
