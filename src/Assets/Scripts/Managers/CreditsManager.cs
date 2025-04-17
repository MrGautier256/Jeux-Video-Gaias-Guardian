using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private float creditsDuration = 20f; 

    private void Start()
    {
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    private System.Collections.IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSeconds(creditsDuration);
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}
