using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 3f;
    public float damage = 30f;
    public float lifetime = 10f;
    private float currentLifetime;
    public float turnSpeed = 180f;
    public GameObject hitEffectPrefab;

    private Transform target;
    private Rigidbody2D rb;

    [SerializeField] GameObject shockwaveEffectPrefab;

    // Mini missile
    [SerializeField] GameObject miniMissilePrefab;
    [HideInInspector] public GameObject myPrefab;
    [SerializeField] int miniMissileCount = 3;
    [SerializeField] float miniDamageMultiplier = 0.6f;
    public bool isMiniMissile = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // void Start()
    // {
    //     // Destroy(gameObject, lifetime);
    // }

    void OnEnable()
    {
        currentLifetime = 0f;
        target = null;

        // Safety resets
        transform.localScale = new Vector3(0.1f, 0.25f, 0f); 
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        if (PauseManager.IsPaused) return;

        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifetime)
        {
            ObjectPooler.Instance.ReturnToPool(myPrefab, gameObject);
        }
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
        float spawnOffset = 0.85f;

        for (int i = 0; i < miniMissileCount; i++)
        {
            float angle = -spreadAngle / 2f + spreadAngle * i / (miniMissileCount - 1);
            Quaternion rot = Quaternion.Euler(0, 0, transform.eulerAngles.z + angle);

            Vector3 spawnPos = transform.position + (rot * Vector3.up * spawnOffset);

            // GameObject mini = Instantiate(miniMissilePrefab, spawnPos, rot);
            GameObject mini = ObjectPooler.Instance.GetFromPool(miniMissilePrefab, spawnPos, rot);
            mini.transform.localScale = new Vector3(0.08f, 0.18f, 0f);

            HomingMissile hm = mini.GetComponent<HomingMissile>();
            Rigidbody2D miniRb = mini.GetComponent<Rigidbody2D>();

            if (hm != null)
            {
                hm.myPrefab = miniMissilePrefab;
                hm.isMiniMissile = true;

                hm.damage = damage * miniDamageMultiplier;
                hm.isMiniMissile = true;

                if (miniRb != null)
                {
                    miniRb.simulated = true;
                    miniRb.linearVelocity = mini.transform.up * speed;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (PauseManager.IsPaused) return;

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
            // Destroy(gameObject);
            ObjectPooler.Instance.ReturnToPool(myPrefab, gameObject);
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
