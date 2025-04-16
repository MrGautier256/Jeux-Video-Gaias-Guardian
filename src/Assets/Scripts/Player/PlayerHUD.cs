using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerHUD : MonoBehaviour
{
    [Header("Hearts")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Lives")]
    public TMP_Text lifeText;

    [Header("Special Attacks UI")]
    public Image vortexIcon;
    public TextMeshProUGUI vortexCooldownText;

    public Image waterJetIcon;
    public TextMeshProUGUI waterJetCooldownText;

    private float vortexCooldown = 3f;
    private float waterJetCooldown = 2f;


    void Start()
    {
        UpdateAbilityVisibility(); 
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

    public void SetCooldowns(float vortexCD, float waterJetCD)
    {
        vortexCooldown = vortexCD;
        waterJetCooldown = waterJetCD;
    }

    public void UpdateCooldowns(float timeSinceVortex, float timeSinceWater, float currentTime)
    {
        float vortexRemaining = vortexCooldown - (currentTime - timeSinceVortex);
        float waterRemaining = waterJetCooldown - (currentTime - timeSinceWater);

        UpdateIcon(vortexIcon, vortexCooldownText, vortexRemaining);
        UpdateIcon(waterJetIcon, waterJetCooldownText, waterRemaining);
    }

    private void UpdateIcon(Image icon, TextMeshProUGUI text, float remaining)
    {
        if (remaining <= 0)
        {
            icon.color = Color.white;
            text.text = "";
        }
        else
        {
            icon.color = new Color(1f, 1f, 1f, 0.5f); 
            text.text = Mathf.CeilToInt(remaining).ToString();
        }
    }

    public void SetAbilitiesVisibility(bool showVortex, bool showWaterJet)
    {
        vortexIcon.gameObject.SetActive(showVortex);
        vortexCooldownText.gameObject.SetActive(showVortex);

        waterJetIcon.gameObject.SetActive(showWaterJet);
        waterJetCooldownText.gameObject.SetActive(showWaterJet);
    }

    private void UpdateAbilityVisibility()
    {
        var abilities = SaveManager.Instance?.CurrentSave?.abilities;
        if (abilities == null) return;

        SetAbilitiesVisibility(abilities.hasPollenVortex, abilities.hasWaterJet);
    }

    private void OnEnable()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.AddAbilitiesUpdatedListener(UpdateAbilityVisibility);
    }

    private void OnDisable()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.RemoveAbilitiesUpdatedListener(UpdateAbilityVisibility);
    }

    

}
