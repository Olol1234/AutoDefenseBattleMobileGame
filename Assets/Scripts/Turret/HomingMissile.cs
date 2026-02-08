using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 3f;
    public float damage = 30f;
    // public float damage = PlayerProfile.Instance.homingMissileTurretBaseDamage;
    public float lifetime = 10f;
    public float turnSpeed = 180f;
    public GameObject hitEffectPrefab;

    private Transform target;
    private Rigidbody2D rb;

    [SerializeField] GameObject shockwaveEffectPrefab;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Damage(float newDamage)
    {
        damage = newDamage;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            target = FindNewTarget();
        }
        if (target == null)
        {
            // Move straight if no target
            rb.linearVelocity = transform.up * speed;
            return;
        }

        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * turnSpeed;
        rb.linearVelocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthEnemy enemyHealth = other.GetComponent<HealthEnemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            if (shockwaveEffectPrefab != null)
            {
                GameObject shockwaveInstance = Instantiate(shockwaveEffectPrefab, transform.position, Quaternion.identity);
                ShockwaveExpand shockwave = shockwaveInstance.GetComponent<ShockwaveExpand>();
                if (shockwave != null)
                {
                    shockwave.damage = damage;
                }
            }
            Destroy(gameObject);
        }
    }

    Transform FindNewTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;        
    }

}
