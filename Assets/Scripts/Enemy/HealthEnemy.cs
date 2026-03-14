using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : MonoBehaviour
{
    public float baseHealth = 10f;
    private float maxHealth;
    public float GetMaxHealth() => maxHealth;
    private float currentHealth;
    public int expReward = 25;
    private bool isDead = false;
    private IKnockbackable knockbackComponent;

    [HideInInspector] public GameObject myPrefab;

    [Header("Elemental Affinities")]
    public List<ElementalType> weakness;   // Takes 2x Damage
    public List<ElementalType> resistance; // Takes 0.5x Damage
    public List<ElementalType> immunity;   // Takes 0 Damage

    void OnEnable()
    {
        isDead = false;

        float mult = 1f;    
        if (LevelManager.Instance != null)
            mult = LevelManager.Instance.GetDifficultyMultiplier();
        maxHealth = baseHealth * mult;
        currentHealth = maxHealth;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        EnemyManager.Instance?.RegisterEnemy(this);
    }

    void Awake()
    {
        knockbackComponent = GetComponent<IKnockbackable>();
    }

    public void RequestKnockback(Vector2 direction, float force)
    {
        knockbackComponent?.ApplyKnockback(direction, force);
    }

    public void ApplyHealthMultiplier(float multiplier)
    {
        maxHealth *= multiplier;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, ElementalType damageType)
    {
        if (isDead) return;

        float multiplier = 1f;
        if (immunity != null && immunity.Contains(damageType))
        {
            multiplier = 0f;
        }
        else if (weakness != null && weakness.Contains(damageType))
        {
            multiplier *= 2f;
        }
        else if (resistance != null && resistance.Contains(damageType))
        {
            multiplier *= 0.5f;
        }

        float finalDamage = damage * multiplier;

        currentHealth -= finalDamage;
        DamageTextSpawner.Instance?.Spawn((int)finalDamage, transform.position);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        EnemyManager.Instance?.UnregisterEnemy(this);

        int finalExp = expReward;
        if (LevelManager.Instance != null)
        {
            finalExp *= (int)LevelManager.Instance.GetDifficultyMultiplier();
        }
        PlayerExp.Instance.AddExp(finalExp);

        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.OnEnemyDestroyed();
        }

        ObjectPooler.Instance.ReturnToPool(myPrefab, gameObject);
        // Destroy(gameObject);
    }

}
