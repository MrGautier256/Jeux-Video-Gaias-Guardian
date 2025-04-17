using UnityEngine;

public class WaterJetProjectile : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 2;
    public float lifetime = 1.5f;
    private bool hasHit = false;

    private Vector2 direction;

    public void Launch(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        Transform target = other.transform;

        if (other.CompareTag("Enemy") || (target.parent != null && target.parent.CompareTag("Enemy")))
        {
            EnemyHealth enemy = target.GetComponent<EnemyHealth>();
            if (enemy == null && target.parent != null)
            {
                enemy = target.parent.GetComponent<EnemyHealth>();
            }

            if (enemy != null)
            {
                hasHit = true; 
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            Destroy(gameObject);
        }
    }


}
