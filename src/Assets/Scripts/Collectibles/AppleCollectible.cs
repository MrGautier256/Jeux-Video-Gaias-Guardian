using UnityEngine;

public class AppleCollectible : MonoBehaviour
{
    private bool collected = false;
    public int restorePV = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        if (collision.CompareTag("Player"))
        {
            collected = true;

            PlayerCollision pc = collision.GetComponent<PlayerCollision>();
            if (pc != null)
            {
                pc.CollectApple();
                pc.Heal(restorePV);
            }
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject);
        }
    }
}
