using UnityEngine;
using System.Collections;
using System.Net;
using NUnit.Framework;

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

    [Header("Sweep Laser")]
    [SerializeField] float sweepAngle = 100f;
    [SerializeField] float sweepSpeed = 2f;
    private float sweepTimer;
    public LineRenderer sweepLineRenderer;

    [Header("Another 2 laser")]
    [SerializeField] LineRenderer leftLineRenderer;
    [SerializeField] LineRenderer rightLineRenderer;
    [SerializeField] float sideAngle = 40f;
    [SerializeField] float sideDamageMultiplier = 0.8f;
    public bool hasSideLaser = false;

    [Header("Run Stats")]
    public float damageMultiplier = 1f;
    public float damageAddition = 0f;
    public float damagePenalty = 0f;
    public int extraLaser = 0;
    public float lowerCooldownFactor = 0f;
    public float extraDuration = 0f;
    public bool hasSweepLaser = false;

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
        hasSweepLaser = false;
        hasSideLaser = false;
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

        if (hasSweepLaser && sweepLineRenderer != null)
            sweepLineRenderer.enabled = true;

        if (hasSideLaser)
        {
            if (leftLineRenderer) leftLineRenderer.enabled = true;
            if (rightLineRenderer) rightLineRenderer.enabled = true;
        }

        float timer = 0f;

        Vector2 dir = (target.position - firePoint.position).normalized;

        float duration = GetDuration();

        while (timer < duration)
        {
            timer += Time.deltaTime;

            Vector2 start = firePoint.position;

            // Main Laser
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

            // Side laser
            if (hasSideLaser)
            {
                Vector2 leftDir = Quaternion.Euler(0, 0, -sideAngle) * dir;
                Vector2 rightDir = Quaternion.Euler(0, 0, sideAngle) * dir;

                FireSideLaser(leftDir, leftLineRenderer, start);
                FireSideLaser(rightDir, rightLineRenderer, start);
            }

            // Sweep Laser
            if (hasSweepLaser && sweepLineRenderer != null)
            {
                float sweepProgress = timer / duration;
                float angle = Mathf.Lerp(-sweepAngle * 0.5f, sweepAngle * 0.5f, sweepProgress);

                Quaternion rot = Quaternion.Euler(0, 0, angle);
                Vector2 sweepDir = rot * firePoint.up;

                RaycastHit2D[] sweepHits = Physics2D.RaycastAll(start, sweepDir, maxLaserLength);
                Vector2 sweepEnd = start + sweepDir * maxLaserLength;
                foreach (var hit in sweepHits)
                {
                    HealthEnemy enemy = hit.collider.GetComponent<HealthEnemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(GetDamage() * 0.65f * Time.deltaTime);
                    }
                }

                sweepLineRenderer.SetPosition(0, start);
                sweepLineRenderer.SetPosition(1, sweepEnd);
            }

            yield return null;
        }

        lineRenderer.enabled = false;

        if (sweepLineRenderer != null) sweepLineRenderer.enabled = false;
        if (leftLineRenderer != null) leftLineRenderer.enabled = false;
        if (rightLineRenderer != null) rightLineRenderer.enabled = false;

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

    void FireSideLaser(Vector2 dir, LineRenderer lr, Vector2 start)
    {
        if (lr == null) return;

        // Vector2 start = firePoint.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, dir, maxLaserLength);
        Vector2 end = start + dir * maxLaserLength;

        foreach (var hit in hits)
        {
            var enemy = hit.collider.GetComponent<HealthEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(GetDamage() * sideDamageMultiplier * Time.deltaTime);
            }
        }

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

}
