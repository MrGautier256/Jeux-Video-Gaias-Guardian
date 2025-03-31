using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    public int remainingLives = 3;
    public int apples = 0;

    [Header("Invulnerabilite")]
    public float invulnerabilityDuration = 0.85f;

    [Header("Audio")]
    public AudioClip deathSound;
    public AudioClip appleCollectSound;
    public AudioClip hitSound;

    private AudioSource audioSource;
    private bool isDead = false;
    private bool isInvulnerable = false;
    private bool shouldLoseLife = true;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
    }


    public bool IsInvulnerable() => isInvulnerable;

    public void CollectApple()
    {
        apples++;
        if (appleCollectSound && audioSource) audioSource.PlayOneShot(appleCollectSound);
    }


    public void TakeDamages(int damage, Vector2 knockbackDirection)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Kill(); 
        }
        else
        {
            if (hitSound && audioSource) audioSource.PlayOneShot(hitSound);
            StartCoroutine(TemporaryKnockback());
            StartCoroutine(InvulnerabilityRoutine());
            ApplyKnockback(knockbackDirection);
        }
    }

    public void Kill(bool loseLife = true)
    {
        if (isDead) return;

        isDead = true;
        shouldLoseLife = loseLife;

        GetComponent<Player>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * 110f);

        if (deathSound && audioSource) audioSource.PlayOneShot(deathSound);

        foreach (Collider2D col in GetComponents<Collider2D>())
            col.isTrigger = true;

        StartCoroutine(FakeDeathEffect());

        Invoke(nameof(RestartLogic), 2f);
    }

    private void RestartLogic()
    {
        if (shouldLoseLife) remainingLives--;

        if (remainingLives > 0)
        {
            currentHealth = maxHealth; // on r�g�n�re les HP
            Transform spawn = RespawnManager.Instance?.GetRespawnPoint();

            if (spawn != null)
            {
                StartCoroutine(RespawnAt(spawn.position));
                return;
            }
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator RespawnAt(Vector3 position)
    {
        yield return new WaitForSeconds(0.1f);

        transform.position = position;
        isDead = false;

        foreach (Collider2D col in GetComponents<Collider2D>())
            col.isTrigger = false;

        GetComponent<Player>().enabled = true;
    }

    private void ApplyKnockback(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * 120f);
    }

    private IEnumerator FakeDeathEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;

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
        yield return new WaitForSeconds(0.25f);
        GetComponent<Player>().enabled = true;
    }
}
