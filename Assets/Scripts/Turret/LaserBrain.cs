using UnityEngine;
using System.Collections;
using System.Net;

public class LaserTurret : MonoBehaviour
{
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

    private bool isFiring = false;
    private bool isCooling = false;

    void Start()
    {
        lineRenderer.enabled = false;
        StartCoroutine(LaserRoutine());
    }

    IEnumerator LaserRoutine()
    {
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

        while (timer < laserDuration)
        {
            timer += Time.deltaTime;

            Vector2 start = firePoint.position;

            // Implement raycast here
            RaycastHit2D[] hits = Physics2D.RaycastAll(start, dir, maxLaserLength);

            Vector2 end = start + dir * maxLaserLength;

            if (hits.Length > 0)
            {
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                end = hits[hits.Length -1].point;
                foreach (var hit in hits)
                {
                    HealthEnemy enemy = hit.collider.GetComponent<HealthEnemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(laserDPS * Time.deltaTime);
                    }
                }
            }

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            yield return null;
        }

        lineRenderer.enabled = false;

        isFiring = false;
        isCooling = true;

        yield return new WaitForSeconds(laserCooldown);

        isCooling = false;
    }

    // void DamageEnemiesAlongBeam(Vector2 start, Vector2 dir)
    // {
    //     RaycastHit2D[] hits = Physics2D.RaycastAll(start, dir, maxLaserLength, enemyLayer);

    //     foreach (var hit in hits)
    //     {
    //         HealthEnemy enemy = hit.collider.GetComponent<HealthEnemy>();
    //         if (enemy != null)
    //         {
    //             enemy.TakeDamage(laserDPS * Time.deltaTime);
    //         }
    //     }
    // }

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
