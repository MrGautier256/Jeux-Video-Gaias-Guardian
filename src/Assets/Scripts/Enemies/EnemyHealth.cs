using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public AudioClip hitSound;
    public AudioClip deathSound;
    private AudioSource audioSource;
    private SpriteRenderer sr;
    private Animator animator;
    public event System.Action OnDeath;


    private void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (hitSound != null && audioSource != null)
                audioSource.PlayOneShot(hitSound);
            StartCoroutine(DamageFlash());
        }
    }

    private IEnumerator DamageFlash()
    {
        if (sr != null)
        {
            Color original = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = original;
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();

        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);

        if (animator != null)
        {
            animator.SetTrigger("deathTrigger");
            StartCoroutine(WaitForDeathAnimation());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

}