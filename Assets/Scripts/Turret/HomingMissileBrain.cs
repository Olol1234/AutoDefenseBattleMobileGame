using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileBrain : MonoBehaviour
{
    public static HomingMissileBrain Instance;
    [SerializeField] public float fireCooldown = 3f;
    [SerializeField] public float damage = 50f;
    [SerializeField] public int missileCount = 2;
    private float fireTimer;
    private List<Transform> enemiesInRange = new List<Transform>();
    private Transform currentTarget;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public GameObject missilePrefab;
    [HideInInspector] public GameObject myPrefab;
    public float spreadAngle = 15f;
    public Transform firePoint;

    [Header("Run Stats")]
    public float damageMultiplier = 1f;
    public float damageAddition = 0f;
    public float damagePenalty = 0f;
    public int extraMissile = 0;
    public float lowerCooldownFactor = 0f;
    public bool hasShockwaveOnImpact = false;
    public bool hasMiniMissiles = false;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitFromProfile()
    {
        var profile = PlayerProfile.Instance;

        damage = profile.homingMissileTurretBaseDamage;
        fireCooldown = profile.homingMissileTurretCooldown;

        ResetRunStats();
    }

    public void ResetRunStats()
    {
        damageMultiplier = 1f;
        damageAddition = 0f;
        damagePenalty = 0f;
        extraMissile = 0;
        lowerCooldownFactor = 0f;
        hasShockwaveOnImpact = false;
        hasMiniMissiles = false;
    }

    public float GetDamage()
    {
        float basePlusAdd = damage + damageAddition;
        float final = basePlusAdd * damageMultiplier;

        return Mathf.Max(1f, final - damagePenalty);
    }

    public int GetMissileCount()
    {
        return missileCount + extraMissile;
    }

    public float GetFireCooldown()
    {
        float finalCooldown = fireCooldown - (fireCooldown * lowerCooldownFactor);
        return finalCooldown;
    }

    void Update()
    {
        if (PauseManager.IsPaused) return;
        CleanEnemyList();
        SelectTarget();
        AimAtTarget();

        fireTimer += Time.deltaTime;

        if (fireTimer >= GetFireCooldown())
        {
            Fire();
            fireTimer = 0f;
        }
    }

    void Fire()
    {
        // if (currentTarget == null) return;
        if (enemiesInRange.Count == 0) return;

        if (animator != null) {
            // Force the animator to start from the Idle state
            // animator.Play("Idle", 0, 0f); 
            // Tell it to transition to the firing animation
            animator.SetTrigger("Shoot");
        }

        float startAngle = -(GetMissileCount() - 1) * spreadAngle * 0.5f;

        for (int i = 0; i < GetMissileCount(); i++)
        {
            float angleOffset = startAngle + (i * spreadAngle);

            Quaternion rot = firePoint.rotation *
                Quaternion.Euler(0, 0, angleOffset);

            // GameObject missileObj = Instantiate(
            //     missilePrefab, firePoint.position, rot
            // );
            GameObject missileObj = ObjectPooler.Instance.GetFromPool(
                missilePrefab, firePoint.position, rot
            );

            if (missileObj != null)
            {
                HomingMissile missile = missileObj.GetComponent<HomingMissile>();

                // missile.myPrefab = missilePrefab;
                // missile.isMiniMissile = false;
                missile.Damage(GetDamage());

                Transform target = enemiesInRange[i % enemiesInRange.Count];
                missile.SetTarget(target);
            }
        }
        
        // Debug.Log("Missile Fired!");
    }

    // ---------------- COLLIDER RANGE ----------------

    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("Something entered trigger: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            // Debug.Log("Enemy entered range!");
            enemiesInRange.Add(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.transform);
        }
    }

    // ---------------- TARGETING ----------------

    void CleanEnemyList()
    {
        enemiesInRange.RemoveAll(e => e == null);
    }

    void SelectTarget()
    {
        currentTarget = null;
        var enemies = EnemyManager.Instance.ActiveEnemies;

        // foreach (var enemy in enemiesInRange)
        // {
        //     if (IsVisibleOnScreen(enemy.position))
        //     {
        //         currentTarget = enemy;
        //         return;
        //     }
        // }
        float closestDist = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            if (IsVisibleOnScreen(enemy.transform.position))
            {
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    currentTarget = enemy.transform;
                }
            }
        }
    }

    void AimAtTarget()
    {
        if (currentTarget == null) return;

        Vector2 dir = (currentTarget.position - firePoint.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        firePoint.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // ---------------- CAMERA VISIBILITY ----------------

    bool IsVisibleOnScreen(Vector3 worldPos)
    {
        Vector3 viewPos = mainCam.WorldToViewportPoint(worldPos);

        return viewPos.z > 0 &&
               viewPos.x >= 0 && viewPos.x <= 1 &&
               viewPos.y >= 0 && viewPos.y <= 1;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
