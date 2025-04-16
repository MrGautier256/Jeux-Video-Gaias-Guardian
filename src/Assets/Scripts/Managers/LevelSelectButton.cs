using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private string sceneName;           // ex: "Level_1"
    [SerializeField] private string sceneIfClaimed;      // ex: "Level_1_clear"

    private void Start()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager not loaded.");
            return;
        }

        var claimed = SaveManager.Instance.CurrentSave.progression.levelsClaimed;

        bool isClaimed = sceneName switch
        {
            "Level_1" => claimed.Level_1,
            "Level_2" => claimed.Level_2,
            "Level_3" => claimed.Level_3,
            "Level_4" => claimed.Level_4,
            _ => false
        };

        if (!isClaimed)
        {
            gameObject.SetActive(false); // cache le bouton s'il est bloqué
        }
    }

    public void TryLoadLevel()
    {
        if (SaveManager.Instance == null) return;

        var claimed = SaveManager.Instance.CurrentSave.progression.levelsClaimed;

        bool isClaimed = sceneName switch
        {
            "Level_1" => claimed.Level_1,
            "Level_2" => claimed.Level_2,
            "Level_3" => claimed.Level_3,
            "Level_4" => claimed.Level_4,
            _ => false
        };

        if (isClaimed)
        {
            string targetScene = string.IsNullOrEmpty(sceneIfClaimed) ? sceneName : sceneIfClaimed;
            SceneTransitionManager.Instance.LoadSceneWithFade(targetScene);
        }
        else
        {
            Debug.Log($"Le niveau {sceneName} n’est pas encore débloqué.");
        }
    }
}
