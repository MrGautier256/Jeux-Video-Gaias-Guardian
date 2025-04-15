using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionTrigger : MonoBehaviour
{
    [Header("Nom de la scene a charger")]
    [SerializeField] private string sceneToLoad;

    [Header("Scene alternative si ce niveau est claim (ex: Level_1_clear)")]
    [SerializeField] private string sceneIfClaimed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        string sceneToLaunch = sceneToLoad;

        if (SaveManager.Instance != null)
        {
            var claimed = SaveManager.Instance.CurrentSave.progression.levelsClaimed;

            bool isClaimed = sceneToLoad switch
            {
                "Level_1" => claimed.Level_1,
                "Level_2" => claimed.Level_2,
                "Level_3" => claimed.Level_3,
                "Level_4" => claimed.Level_4,
                "Level_5" => claimed.Level_5,
                _ => false 
            };

            if (isClaimed && !string.IsNullOrEmpty(sceneIfClaimed))
            {
                sceneToLaunch = sceneIfClaimed;
            }
        }

        SceneTransitionManager.Instance.LoadSceneWithFade(sceneToLaunch);
    }
}
