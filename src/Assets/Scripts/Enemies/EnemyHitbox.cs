using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public EnemyHealth enemyHealth;

    private void Awake()
    {
        if (enemyHealth == null)
            enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    public void ReceiveHit(int damage)
    {
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
    }
}