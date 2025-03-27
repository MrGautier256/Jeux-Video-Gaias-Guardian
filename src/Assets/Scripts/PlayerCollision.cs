using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerCollision : MonoBehaviour
{
    public int life = 3;
    public int apples = 0;
    private bool isDead = false;
    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 0.85f;
    public AudioClip deathSound;
    public AudioClip appleCollectSound;
    public AudioClip hitSound;
    private AudioSource audioSource;



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }


    public void CollectApple()
    {
        apples++;

        if (appleCollectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(appleCollectSound);
        }
    }


    public void TakeDamages(int damage, Vector2 knockbackDirection)

    {
        if (isDead || isInvulnerable) return; // Ignore si déjà mort
        life -= damage;

        if (life <= 0)
        {
            Die();
        }
        else
        {
            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
            StartCoroutine(TemporaryKnockback());
            StartCoroutine(InvulnerabilityRoutine());
            ApplyKnockback(knockbackDirection);
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        GetComponent<Player>().enabled = false;


        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(0, 110f));

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.isTrigger = true;
        }

        StartCoroutine(FakeDeathEffect());

        Invoke("RestartLevel", 2);
    }

    private void ApplyKnockback(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * 120f); // Force ajustable selon besoin
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    private IEnumerator FakeDeathEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;

        // Petit clignotement rouge
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.red;
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float elapsed = 0f;
        while (elapsed < invulnerabilityDuration)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sr.enabled = true;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }

        isInvulnerable = false;
    }

    private IEnumerator TemporaryKnockback()
    {
        GetComponent<Player>().enabled = false;

        yield return new WaitForSeconds(0.25f); // Durée du knockback

        GetComponent<Player>().enabled = true;
    }
}
