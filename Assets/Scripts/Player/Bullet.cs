using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f;
    // private float damage = PlayerStats.Instance.GetDamage();
    private float remainingPenetration;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    void Start()
    {
        Destroy(gameObject, lifeTime);
        remainingPenetration = PlayerStats.Instance.GetPenetration();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HealthEnemy enemy = other.GetComponent<HealthEnemy>();
        if (enemy == null) return;
        if (hitEnemies.Contains(other.gameObject)) return;
        hitEnemies.Add(other.gameObject);

        enemy.TakeDamage(PlayerStats.Instance.GetDamage());
        if (remainingPenetration > 0)
        {
            remainingPenetration--;
            // Bullet continues flying
        }
        else
        {
            Destroy(gameObject);
        }
    }
}