using UnityEngine;
using System.Collections;
using System.Net;

public class LaserTurretBrain : MonoBehaviour
{
    public static LaserTurretBrain Instance;

    [Header("References")]
    public Transform firePoint;
    public LineRenderer lineRenderer;

    [Header("Laser Settings")]
    public float laserDuration = 2.5f;
    public float laserCooldown = 4f;
    public float laserDPS = 50f;
    public float maxLaserLength = 150f; // safety cap

    [Header("Targeting")]
    public float range = 14f;
    public LayerMask enemyLayer;

    [Header("Run Stats")]
    public float damageMultiplier = 1f;
    public float damageAddition = 0f;
    public float damagePenalty = 0f;
    public int extraLaser = 0;
    public float lowerCooldownFactor = 0f;
    public float extraDuration = 0f;

    private bool isFiring = false;
    private bool isCooling = false;

    void Start()
    {
        lineRenderer.enabled = false;
        StartCoroutine(LaserRoutine());
    }

    void Awake()
    {
        Instance = this;
    }

    public void InitFromProfile()
    {
        var profile = PlayerProfile.Instance;

        laserDPS = profile.laserTurretBaseDPS;
        laserCooldown = profile.laserTurretCooldown;
        laserDuration = profile.laserTurretDuration;

        ResetRunStats();
    }

    public void ResetRunStats()
    {
        damageMultiplier = 1f;
        damageAddition = 0f;
        damagePenalty = 0f;
        extraLaser = 0;
        lowerCooldownFactor = 0f;
        extraDuration = 0f;
    }

    public float GetDamage()
    {
        float basePlusAdd = laserDPS + damageAddition;
        float final = basePlusAdd * damageMultiplier;
        final -= damagePenalty;
        return Mathf.Max(final, 0f);
    }

    public float GetDuration()
    {
        return laserDuration + extraDuration;
    }

    public float GetCooldown()
    {
        float finalCooldown = laserCooldown - (laserCooldown * lowerCooldownFactor);
        return finalCooldown;
    }

    IEnumerator LaserRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (!isFiring && !isCooling)
            {
                Transform target = FindClosestEnemy();

                if (target != null)
                {
                    yield return StartCoroutine(FireLaser(target));
                }
            }

            yield return null;
        }
    }

    IEnumerator FireLaser(Transform target)
    {
        isFiring = true;
        lineRenderer.enabled = true;

        float timer = 0f;

        Vector2 dir = (target.position - firePoint.position).normalized;

        // while (timer < laserDuration)
        while (timer < GetDuration())
        {
            timer += Time.deltaTime;

            Vector2 start = firePoint.position;

            // Implement raycast here
            RaycastHit2D[] hits = Physics2D.RaycastAll(start, dir, maxLaserLength);

            Vector2 end = start + dir * maxLaserLength;

            foreach (var hit in hits)
            {
                HealthEnemy enemy = hit.collider.GetComponent<HealthEnemy>();
                if (enemy != null)
                {
                    // enemy.TakeDamage(laserDPS * Time.deltaTime);
                    enemy.TakeDamage(GetDamage() * Time.deltaTime);
                }
            }

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            yield return null;
        }

        lineRenderer.enabled = false;

        isFiring = false;
        isCooling = true;

        // yield return new WaitForSeconds(laserCooldown);
        yield return new WaitForSeconds(GetCooldown());

        isCooling = false;
    }

    Transform FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);

        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }

        return closest;
    }
}
