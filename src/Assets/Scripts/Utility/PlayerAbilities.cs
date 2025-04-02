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

        Debug.Log("[PlayerAbilities] Abilities chargées : " +
                  $"DoubleJump={CanDoubleJump}, Dash={CanDash}, Grapple={CanGrapple}, Sword={CanUseSword}");
    }
}
