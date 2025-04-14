using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayerCollision : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;
    public int initialLives = 3;
    public int apples = 0;

    [Header("Invulnérabilité")]
    public float invulnerabilityDuration = 0.85f;

    [Header("Audio")]
    public AudioClip deathSound;
    public AudioClip appleCollectSound;
    public AudioClip hitSound;

    private AudioSource audioSource;
    private HealthSystem healthSystem;

    private bool isDead = false;
    private bool isInvulnerable = false;
    private bool shouldLoseLife = true;

    public bool IsDead() => isDead;

    [Header("Interface")]
    private PlayerHUD playerHUD;



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        healthSystem = new HealthSystem(maxHealth, initialLives);
        healthSystem.OnDeath += HandleDeath;

        if (playerHUD == null)
        {
            playerHUD = FindAnyObjectByType<PlayerHUD>();
            if (playerHUD == null)
                Debug.LogWarning("[PlayerCollision] Aucun PlayerHUD trouvé dans la scène !");
        }

        playerHUD?.UpdateHearts(healthSystem.CurrentHealth);
        playerHUD?.UpdateLives(healthSystem.Lives);
    }



    public bool IsInvulnerable() => isInvulnerable;

    public void CollectApple()
    {
        apples++;
        if (appleCollectSound && audioSource) audioSource.PlayOneShot(appleCollectSound);
    }
    public void Heal(int amount)
    {
        if (isDead) return;

        healthSystem.Heal(amount);

        if (playerHUD != null)
        {
            playerHUD.UpdateHearts(healthSystem.CurrentHealth);
        }
    }


    public void TakeDamages(int damage, Vector2 knockbackDirection)
    {
        if (isDead || isInvulnerable) return;

        healthSystem.TakeDamage(damage);
        playerHUD.UpdateHearts(healthSystem.CurrentHealth);
        if (hitSound && audioSource) audioSource.PlayOneShot(hitSound);
        StartCoroutine(TemporaryKnockback());
        StartCoroutine(InvulnerabilityRoutine());
        ApplyKnockback(knockbackDirection);

    }

    public void Kill(bool loseLife = true)
    {
        if (isDead) return;

        isDead = true;
        shouldLoseLife = loseLife;

        if (playerHUD != null)
        {
            playerHUD.UpdateHearts(0);
        }

        // Désactiver le script de mouvement
        GetComponent<Player>().enabled = false;

        // Empêcher tout déclencheur ou interaction
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * 110f);

        if (deathSound && audioSource) audioSource.PlayOneShot(deathSound);

        // Désactive tous les colliders pour éviter triggers & collisions
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        // Optionnel : changer le layer du joueur temporairement pour l'exclure de tout (ex: "IgnoreEverything")
        gameObject.layer = LayerMask.NameToLayer("IgnoreEverything");

        StartCoroutine(FakeDeathEffect());

        Invoke(nameof(RestartLogic), 2f);
    }


    private void HandleDeath()
    {
        Kill();
    }

    private void RestartLogic()
    {
        if (shouldLoseLife)
        {
            healthSystem.LoseLife();
            playerHUD.UpdateLives(healthSystem.Lives);


            if (!healthSystem.HasLivesLeft())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                playerHUD.UpdateLives(0);
                return;
            }

            healthSystem.ResetHealth();

            if (playerHUD != null)
            {
                playerHUD.UpdateHearts(healthSystem.CurrentHealth);
                playerHUD.UpdateLives(healthSystem.Lives);
            }

        }

        Transform spawn = RespawnManager.Instance?.GetRespawnPoint();
        if (spawn != null)
        {
            StartCoroutine(RespawnAt(spawn.position));
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    private IEnumerator RespawnAt(Vector3 position)
    {
        yield return new WaitForSeconds(0.1f);

        transform.position = position;
        isDead = false;

        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = true;

        gameObject.layer = LayerMask.NameToLayer("Default");

        GetComponent<Player>().enabled = true;
    }


    private void ApplyKnockback(Vector2 direction)
    {
        if (isDead) return;

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
        yield return new WaitForSeconds(0.1f);

        sr.color = originalColor;
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
