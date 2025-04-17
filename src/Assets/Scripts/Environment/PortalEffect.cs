using UnityEngine;


public class PortalEffect : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayOpen()
    {
        gameObject.SetActive(true);
        animator.Play("PortalOpen", -1, 0f);
    }

    public void PlayClose()
    {
        animator.SetBool("PortalCloseTrigger", true);
    }

    public void DeactivateAfter(float seconds)
    {
        Invoke(nameof(Disable), seconds);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
        animator.SetBool("PortalCloseTrigger", false);
    }
}