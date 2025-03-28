using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damage = 1;
    public Vector2 boxSize = new Vector2(1f, 1f);
    public Vector2 boxOffset = Vector2.zero;
    public float damageInterval = 0.5f;
    public LayerMask playerLayer;

    private float nextDamageTime = 0f;

    private void Update()
    {
        // Scan la zone chaque frame
        Collider2D player = Physics2D.OverlapBox((Vector2)transform.position + boxOffset, boxSize, 0f, playerLayer);

        if (player != null && Time.time >= nextDamageTime)
        {
            PlayerCollision pc = player.GetComponent<PlayerCollision>();
            if (pc != null && !pc.IsInvulnerable())
            {
                float directionX = Mathf.Sign(player.transform.position.x - transform.position.x);
                Vector2 knockbackDir = new Vector2(directionX, 1f);

                pc.TakeDamages(damage, knockbackDir);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + boxOffset, boxSize);
    }
}
