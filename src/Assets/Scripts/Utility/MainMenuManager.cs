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
        if (SaveManager.Instance.CurrentSave.hasSave)
        {
            continueButton.SetActive(true);
            deleteSaveButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
            deleteSaveButton.SetActive(false);
        }
    }

    public void StartNewGame()
    {
        Debug.Log("[MainMenuManager] Nouvelle partie lancée.");
        SaveManager.Instance.ResetSave();
        SaveManager.Instance.SaveGame();

        SceneTransitionManager.Instance.LoadSceneWithFade("Level_1");
    }


    public void ContinueGame()
    {
        Debug.Log("[MainMenuManager] Nouvelle partie lancée.");

        string levelToLoad = SaveManager.Instance.CurrentSave.progression.currentLevel;
        SceneTransitionManager.Instance.LoadSceneWithFade(levelToLoad);
    }

}
