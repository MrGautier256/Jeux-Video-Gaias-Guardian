using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerHUD : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Lives")]
    public TMP_Text lifeText; 


    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
        }
    }

    public void UpdateLives(int currentLives)
    {
        lifeText.text = $"x{currentLives}";
    }

}
