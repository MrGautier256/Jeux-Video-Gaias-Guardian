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
        string finalLevelToLoad = levelToLoad;

        var claimed = SaveManager.Instance.CurrentSave.progression.levelsClaimed;

        bool isClaimed = levelToLoad switch
        {
            "Level_1" => claimed.Level_1,
            "Level_2" => claimed.Level_2,
            "Level_3" => claimed.Level_3,
            "Level_4" => claimed.Level_4,
            "Level_5" => claimed.Level_5,
            _ => false
        };

        if (isClaimed)
        {
            finalLevelToLoad = levelToLoad + "_clear";
        }

        SceneTransitionManager.Instance.LoadSceneWithFade(finalLevelToLoad);
    }

    public void Credits()
    {
        SceneTransitionManager.Instance.LoadSceneWithFade("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    

}
