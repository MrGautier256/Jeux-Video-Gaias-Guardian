using System;
using UnityEngine;

public class HealthSystem
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }

    public int Lives { get; private set; }

    public event Action OnDeath;

    public HealthSystem(int maxHealth, int initialLives)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        Lives = initialLives;
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }

    public bool HasLivesLeft()
    {
        return Lives > 0;
    }

    public void LoseLife()
    {
        Lives--;
    }

    public void SetLives(int count)
    {
        Lives = count;
    }

    public void SetMaxHealth(int newMax)
    {
        MaxHealth = newMax;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }
}
