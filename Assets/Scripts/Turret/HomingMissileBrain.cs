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

    public GameObject missilePrefab;
    public float spreadAngle = 15f;
    public Transform firePoint;

    [Header("Run Stats")]
    public float damageMultiplier = 1f;
    public float damageAddition = 0f;
    public float damagePenalty = 0f;
    public int extraMissile = 0;
    public float lowerCooldownFactor = 0f;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Awake()
    {
        Instance = this;
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
        // Debug.Log("Turret Update Running");

        CleanEnemyList();
        SelectTarget();
        AimAtTarget();

        // if (currentTarget != null)
        //     Debug.Log("TARGET LOCKED: " + currentTarget.name);
        // else
        //     Debug.Log("NO TARGET");

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

        float startAngle = -(GetMissileCount() - 1) * spreadAngle * 0.5f;

        for (int i = 0; i < GetMissileCount(); i++)
        {
            float angleOffset = startAngle + (i * spreadAngle);

            Quaternion rot = firePoint.rotation *
                Quaternion.Euler(0, 0, angleOffset);

            GameObject missileObj = Instantiate(
                missilePrefab, firePoint.position, rot
            );

            HomingMissile missile = missileObj.GetComponent<HomingMissile>();
            missile.Damage(GetDamage());

            if (missile != null)
            {
                Transform target = enemiesInRange[i % enemiesInRange.Count];
                missile.SetTarget(target);
            }
        }
        Debug.Log("Missile Fired!");
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

        foreach (var enemy in enemiesInRange)
        {
            if (IsVisibleOnScreen(enemy.position))
            {
                currentTarget = enemy;
                return;
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
