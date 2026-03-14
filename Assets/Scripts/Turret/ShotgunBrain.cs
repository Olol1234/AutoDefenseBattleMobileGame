using System.Collections.Generic;
using UnityEngine;

public class ShotgunBrain : MonoBehaviour
{
    public static ShotgunBrain Instance;
    [SerializeField] public float fireCooldown = 6f;
    [SerializeField] public float damage = 300f;
    [SerializeField] public int pelletCount = 2;
    [SerializeField] public float spreadAngle = 20f;
    private float knockbackForce = 5f;

    [Header("Shotgun Specifics")]
    public float coneAngle = 60f;
    public float coneRadius = 3f;

    public GameObject pelletPrefab;
    public Transform firePoint;
    private float fireTimer;
    private List<Transform> enemiesInRange = new List<Transform>();
    private Transform currentTarget;
    private Camera mainCam;

    [Header("Run Stats")]
    public float damageMultiplier = 1f;
    public float damageAddition = 0f;
    public float damagePenalty = 0f;
    public int extraPellets = 0;
    public float lowerCooldownFactor = 0f;
    public float knockbackForceMultiplier = 0f;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mainCam = Camera.main;
    }

    public void InitFromProfile()
    {
        var profile = PlayerProfile.Instance;

        damage = profile.shotgunTurretBaseDamage;
        fireCooldown = profile.shotgunTurretCooldown;

        ResetRunStats();
    }

    public void ResetRunStats()
    {
        damageMultiplier = 1f;
        damageAddition = 0f;
        damagePenalty = 0f;
        extraPellets = 0;
        lowerCooldownFactor = 0f;
        knockbackForceMultiplier = 0f;
    }

    public float GetDamage()
    {
        float basePlusAdd = damage + damageAddition;
        float final = basePlusAdd * damageMultiplier;

        return Mathf.Max(1f, final - damagePenalty);
    }

    public float GetPelletCount()
    {
        return pelletCount + extraPellets;
    }

    public float GetFireCooldown()
    {
        float finalCooldown = fireCooldown - (fireCooldown * lowerCooldownFactor);
        return finalCooldown;
    }

    public float GetKnockbackForce()
    {
        return knockbackForce * (1f + knockbackForceMultiplier);
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

    void CleanEnemyList()
    {
        enemiesInRange.RemoveAll(e => e == null);
    }

    void Fire()
    {
        // if (currentTarget == null) return;
        if (enemiesInRange.Count == 0) return;

        // Fan out the 2 cases slightly
        float startAngle = -(GetPelletCount() - 1) * spreadAngle * 0.5f;

        for (int i = 0; i < GetPelletCount(); i++)
        {
            float angleOffset = startAngle + (i * spreadAngle);
            Quaternion rot = firePoint.rotation * Quaternion.Euler(0, 0, angleOffset);

            GameObject pelletObj = ObjectPooler.Instance.GetFromPool(pelletPrefab, firePoint.position, rot);

            if (pelletObj != null)
            {
                // We pass the cone stats to the pellet so it knows how to explode
                ShotgunPellet pelletScript = pelletObj.GetComponent<ShotgunPellet>();
                pelletScript.Initialize(GetDamage(), coneAngle, coneRadius);
            }
        }
    }

    void SelectTarget()
    {
        currentTarget = null;
        var enemies = EnemyManager.Instance.ActiveEnemies;

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

    bool IsVisibleOnScreen(Vector3 worldPos)
    {
        Vector3 viewPos = mainCam.WorldToViewportPoint(worldPos);

        return viewPos.z > 0 &&
               viewPos.x >= 0 && viewPos.x <= 1 &&
               viewPos.y >= 0 && viewPos.y <= 1;
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

}