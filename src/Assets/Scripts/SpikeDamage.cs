using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryApplyDamage(collision);
    }

       private void OnTriggerStay2D(Collider2D collision)
    {
        TryApplyDamage(collision);
    }


    private void TryApplyDamage(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCollision pc = collision.GetComponent<PlayerCollision>();
            if (pc != null)
            {
                float directionX = Mathf.Sign(collision.transform.position.x - transform.position.x);
                Vector2 knockbackDir = new Vector2(directionX, 1f);
                pc.TakeDamages(damage, knockbackDir);
            }
        }
    }

}
