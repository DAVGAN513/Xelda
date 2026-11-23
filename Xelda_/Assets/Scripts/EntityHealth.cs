using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public float invulnerabilityTime = 0.6f;

    private float nextDamageTime = 0f;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetInvulnerable(float duration)
    {
        float invulnUntil = Time.time + duration;
        if (invulnUntil > nextDamageTime)
        {
            nextDamageTime = invulnUntil;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0 || damage <= 0) return;
        if (Time.time < nextDamageTime) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        nextDamageTime = Time.time + invulnerabilityTime;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}