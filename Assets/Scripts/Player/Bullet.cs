using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float lifeTime = 5f;
    private float remainingPenetration;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    private float currentLifeTime;

    // void Start()
    // {
    //     Destroy(gameObject, lifeTime);
    //     remainingPenetration = PlayerStats.Instance.GetPenetration();
    // }

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