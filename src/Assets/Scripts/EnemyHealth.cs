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
        Debug.Log($"{gameObject.name} a pris {amount} dégâts. Vie restante : {currentHealth}");




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
        Debug.Log($"{gameObject.name} est détruit !");

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