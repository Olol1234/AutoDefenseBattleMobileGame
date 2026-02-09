using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

    // Mini missile
    [SerializeField] GameObject miniMissilePrefab;
    [SerializeField] int miniMissileCount = 3;
    [SerializeField] float miniDamageMultiplier = 0.6f;
    public bool isMiniMissile = false;

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

    void SpawnMiniMissiles()
    {
        float spreadAngle = 100f;

        for (int i = 0; i < miniMissileCount; i++)
        {
            float angle = -spreadAngle / 2f + spreadAngle * i / (miniMissileCount - 1);

            Quaternion rot = Quaternion.Euler(0, 0, transform.eulerAngles.z + angle);

            GameObject mini = Instantiate(miniMissilePrefab, transform.position, rot);
            mini.transform.localScale *= 0.7f;

            HomingMissile hm = mini.GetComponent<HomingMissile>();

            if (hm != null)
            {
                hm.damage = damage * miniDamageMultiplier;
                hm.isMiniMissile = true;
            }
        }
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
            if (shockwaveEffectPrefab != null && HomingMissileBrain.Instance.hasShockwaveOnImpact)
            {
                GameObject shockwaveInstance = Instantiate(shockwaveEffectPrefab, transform.position, Quaternion.identity);
                ShockwaveExpand shockwave = shockwaveInstance.GetComponent<ShockwaveExpand>();
                if (shockwave != null)
                {
                    shockwave.damage = damage;
                }
            }
            if (!isMiniMissile && HomingMissileBrain.Instance.hasMiniMissiles)
            {
                SpawnMiniMissiles();
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
