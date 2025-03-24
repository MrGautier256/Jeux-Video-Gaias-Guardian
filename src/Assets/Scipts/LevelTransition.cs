using UnityEngine;
using UnityEngine.SceneManagement;  // Nécessaire pour charger les scènes

public class LevelTransition : MonoBehaviour
{
    // Nom de la scène suivante
    public string nextScene = "Level_1_Section_1";  // Nom de la scène suivante (assurez-vous que le nom est correct)

    // Lorsqu'un autre objet entre dans la zone de transition
    void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifier si l'objet est le joueur
        if (other.CompareTag("Player"))
        {
            // Charger la scène suivante
            SceneManager.LoadScene(nextScene);
        }
    }

    // Optionnel : Permet de passer à la scène suivante en appuyant sur une touche (par exemple "E")
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
