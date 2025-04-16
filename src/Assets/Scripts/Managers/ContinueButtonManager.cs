using UnityEngine;

public class ContinueButtonManager : MonoBehaviour
{
    private void Start()
    {
        if (SaveManager.Instance == null || !SaveManager.Instance.CurrentSave.hasSave)
        {
            gameObject.SetActive(false); // cache ce bouton
        }
    }
}