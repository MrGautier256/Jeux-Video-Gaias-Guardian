using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTracker : MonoBehaviour
{
    private void Start()
    {
        if (SaveManager.Instance != null)
        {
            string scene = SceneManager.GetActiveScene().name;
            SaveManager.Instance.SetCurrentLevel(scene);
        }
    }
}
