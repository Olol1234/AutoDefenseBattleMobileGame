using UnityEngine;

public class HealthEnemy : MonoBehaviour
{
    public float baseHealth = 10f;
    private float maxHealth;
    public float GetMaxHealth() => maxHealth;
    private float currentHealth;
    public int expReward = 25;
    private bool isDead = false;

    [HideInInspector] public GameObject myPrefab;


    // private void Start()
    // {
    //     float mult = 1f;

    //     if (LevelManager.Instance != null)
    //         mult = LevelManager.Instance.GetDifficultyMultiplier();

    //     maxHealth = baseHealth * mult;
    //     currentHealth = maxHealth;
    //     // Debug.Log($"{gameObject.name} spawned with HP: {maxHealth}");
    // }

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
    }

    public void ApplyHealthMultiplier(float multiplier)
    {
        maxHealth *= multiplier;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        DamageTextSpawner.Instance?.Spawn((int)damage, transform.position);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

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
