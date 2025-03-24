using UnityEngine;
using UnityEngine.SceneManagement;  // Nécessaire pour charger les scènes

public class LevelManager : MonoBehaviour
{
    // Variables pour suivre la section actuelle
    public int currentSection = 1;  // Section actuelle (1 ou 2 pour ce niveau)

    // Nom du niveau actuel (peut être utilisé pour le niveau 1 dans ton cas)
    private string currentLevelName = "Level_1";  // Le nom du niveau est "Level_1"

    void Start()
    {
        LoadSection(currentSection);  // Charger la première section au démarrage
    }

    // Charger une section donnée du niveau
    public void LoadSection(int section)
    {
        // Construire le nom de la scène en fonction de la section
        string sceneName = currentLevelName + "_Section_" + section;
        SceneManager.LoadScene(sceneName);  // Charger la scène
    }

    // Passer à la section suivante (appelée après la fin de chaque section)
    public void NextSection()
    {
        if (currentSection == 1)
        {
            currentSection = 2;  // Passer à la section 2
        }
        else
        {
            // Si la section est déjà la 2, ça peut être la fin du niveau ou du jeu
            Debug.Log("Fin du niveau !");
            // Si tu veux recommencer, tu pourrais appeler ResetToFirstSection()
        }

        LoadSection(currentSection);  // Charger la nouvelle section
    }

    // Fonction pour réinitialiser à la première section (pour test ou redémarrage)
    public void ResetToFirstSection()
    {
        currentSection = 1;
        LoadSection(currentSection);  // Recharger la première section
    }
}
