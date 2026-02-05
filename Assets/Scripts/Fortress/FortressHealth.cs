using UnityEngine;

public class FortressHealth : MonoBehaviour
{
    // public int maxHealth = 100;
    [Header("Base Stats (from PlayerProfile)")]
    [SerializeField] private float baseMaxHealth;

    [Header("Runtime")]
    [SerializeField] private int maxHealth;
    public int currentHealth;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public bool logDamage = true;

    public void InitFromProfile()
    {
        baseMaxHealth = PlayerProfile.Instance.baseFortressMaxHP;
        maxHealth = Mathf.RoundToInt(baseMaxHealth);
        currentHealth = maxHealth;
    }

    void Awake()
    {
        // currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (logDamage)
            Debug.Log($"Fortress took {amount} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnFortressDestroyed();
        }
    }

    public float GetHPPercent01()
    {
        return (float)currentHealth / maxHealth;
    }

    void OnFortressDestroyed()
    {
        Debug.Log("💥 FORTRESS DESTROYED — GAME OVER");

        LevelManager.Instance.EndStageLose();
    }
}
