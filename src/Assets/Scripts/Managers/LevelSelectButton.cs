using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    [Header("Nom de la scène associée")]
    [SerializeField] private string sceneName;

    [Header("Bouton UI à activer/désactiver")]
    [SerializeField] private Button levelButton;

    private void Start()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.CurrentSave == null)
        {
            Debug.LogWarning("SaveManager ou Save non chargé.");
            return;
        }

        bool isClaimed = sceneName switch
        {
            "Level_1" => SaveManager.Instance.CurrentSave.progression.levelsClaimed.Level_1,
            "Level_2" => SaveManager.Instance.CurrentSave.progression.levelsClaimed.Level_2,
            "Level_3" => SaveManager.Instance.CurrentSave.progression.levelsClaimed.Level_3,
            "Level_4" => SaveManager.Instance.CurrentSave.progression.levelsClaimed.Level_4,
            "Level_5" => SaveManager.Instance.CurrentSave.progression.levelsClaimed.Level_5,
            _ => false
        };

        levelButton.interactable = isClaimed;
    }

    public void LoadLevel()
    {
        SceneTransitionManager.Instance.LoadSceneWithFade(sceneName);
    }
}
