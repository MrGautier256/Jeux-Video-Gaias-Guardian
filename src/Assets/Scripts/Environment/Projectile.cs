using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;

    [Header("Auto-Destruction")]
    public float lifetime = 5f;

    [Header("Death Feedback")]
    public GameObject deathEffect;
    public AudioClip deathSound;

    private bool isDying = false;

    public void Launch(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
        Invoke(nameof(Explode), lifetime);
    }

    void Update()
    {
        if (!isDying)
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void Explode()
    {
        if (isDying) return;
        isDying = true;

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        Destroy(gameObject);
    }
}
