using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 5f;
    public float lifetime = 4f;
    private float currentLifetime;
    private float damage;

    // [HideInInspector] public string myPoolTag = "EnemyBullet";
    public GameObject enemyBulletPrefab;

    public void Setup(float dmg)
    {
        damage = dmg;
        currentLifetime = 0f;
    }

    void Update()
    {
        // 1. Pause Check
        if (PauseManager.IsPaused) return;

        // 2. Lifetime Management
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifetime)
        {
            ReturnToPool();
            return;
        }

        // 3. Movement (Straight Down)
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fortress"))
        {
            // Assuming your fortress has a health script
            FortressHealth fortress = other.GetComponent<FortressHealth>();
            if (fortress != null)
            {
                fortress.TakeDamage((int)damage);
            }

            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        ObjectPooler.Instance.ReturnToPool(enemyBulletPrefab, gameObject);
    }
}