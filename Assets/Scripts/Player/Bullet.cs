using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float lifeTime = 5f;
    private float remainingPenetration;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    private float currentLifeTime;

    [Header("Elemental Settings")]
    private ElementalType currentElement;
    private float currentDamage;

    void Update()
    {
        // If the game is paused, don't count the lifetime!
        if (PauseManager.IsPaused) return;

        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= lifeTime)
        {
            DisableBullet();
        }
    }

    public void Initialize(ElementalType type, float dmg)
    {
        currentElement = type;
        currentDamage = dmg;
        
        // Optional: Change bullet color based on element
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)        {
            switch (currentElement)
            {
                case ElementalType.Fire:
                    sr.color = Color.red;
                    break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HealthEnemy enemy = other.GetComponent<HealthEnemy>();
        if (enemy == null) return;
        if (hitEnemies.Contains(other.gameObject)) return;
        hitEnemies.Add(other.gameObject);

        // Apply knockback
        Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
        float knockbackForce = PlayerStats.Instance.GetKnockbackForce();
        enemy.RequestKnockback(knockbackDirection, knockbackForce);
        // var normal = other.GetComponent<EnemyAimFortress>();
        // if (normal != null) normal.ApplyKnockback(knockbackDirection, knockbackForce);
        // var zigzag = other.GetComponent<ZigZagMovement>();
        // if (zigzag != null) zigzag.ApplyKnockback(knockbackDirection, knockbackForce);

        // Take damage
        enemy.TakeDamage(currentDamage, currentElement);
        if (remainingPenetration > 0)
        {
            remainingPenetration--;
            // Bullet continues flying
        }
        else
        {
            // Destroy(gameObject);
            DisableBullet();
        }
    }

    void OnEnable()
    {
        remainingPenetration = PlayerStats.Instance.GetPenetration();
        hitEnemies.Clear();
        currentLifeTime = 0f;
        // CancelInvoke();
        // Invoke("DisableBullet", lifeTime);
    }

    void DisableBullet()
    {
        ObjectPooler.Instance.ReturnToPool(bulletPrefab, gameObject);
    }
}