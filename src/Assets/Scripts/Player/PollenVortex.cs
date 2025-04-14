using UnityEngine;

public class PollenVortexProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float slowFactor = 0.3f;
    public float slowDuration = 15f;

    private Vector2 direction;

    public void Launch(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Start()
    {
        Destroy(gameObject, 1.5f);
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IEnemySlowable slowable = other.GetComponent<IEnemySlowable>();
            if (slowable != null)
            {
                slowable.ApplySlow(slowFactor, slowDuration);
            }

            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            Destroy(gameObject);
        }
    }
}