using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public bool CanDoubleJump { get; private set; }
    public bool CanDash { get; private set; }
    public bool CanGrapple { get; private set; }
    public bool CanUseSword { get; private set; }

    void Start()
    {
        LoadAbilities();
    }

    public void LoadAbilities()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("[PlayerAbilities] SaveManager.Instance est null !");
            return;
        }

        var abilities = SaveManager.Instance.CurrentSave.abilities;
        CanDoubleJump = abilities.hasDoubleJump;
        CanDash = abilities.hasDash;
        CanGrapple = abilities.hasGrapple;
        CanUseSword = abilities.hasSword;
    }

    private void OnEnable()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.AddAbilitiesUpdatedListener(LoadAbilities);
    }

    private void OnDisable()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.RemoveAbilitiesUpdatedListener(LoadAbilities);
    }

}
