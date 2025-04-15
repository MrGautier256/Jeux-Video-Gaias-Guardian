using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsReturn : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}
