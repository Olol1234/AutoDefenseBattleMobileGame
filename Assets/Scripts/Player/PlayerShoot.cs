using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform RightHand;
    public float shootRange = 8f;
    public float bulletSpeed = 10f;
    // private float fireRate = PlayerStats.Instance.GetAttackSpeed();
    private float nextFireTime = 0f;

    void Update()
    {
        GameObject target = GetNearestEnemyInRange();

        if (target != null)
        {
            AimAtTarget(target.transform);

            if (Time.time >= nextFireTime)
            {
                Shoot(target);
                nextFireTime = Time.time + PlayerStats.Instance.GetAttackSpeed();
            }
        }
    }

    void Shoot(GameObject target)
    {
        Collider2D enemyCol = target.GetComponent<Collider2D>();
        if (enemyCol == null) return;

        Vector2 targetPoint = enemyCol.bounds.center;
        Vector2 baseDir = (targetPoint - (Vector2)firePoint.position).normalized;

        float angle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
        RightHand.rotation = Quaternion.Euler(0, 0, angle - 90f);

        float totalBullets = 1 + PlayerStats.Instance.GetExtraBullets();

        float spreadAngle = 1f;
        float startAngle = -(totalBullets - 1) * spreadAngle * 0.5f;
        float lateralSpacing = 0.005f;
        Vector2 right = new Vector2(-baseDir.y, baseDir.x);
        
        for (int i = 0; i < totalBullets; i++)
        {
            Vector2 bulletDir = baseDir;
            Vector3 spawnPos = firePoint.position;

            if (i > 0)
            {
                // Alternate left/right: 1 = right, 2 = left, 3 = right, etc
                int side = (i % 2 == 1) ? 1 : -1;

                // How far from center (for i=3,4,...)
                int layer = (i + 1) / 2;

                float angleOffset = spreadAngle * layer * side;
                Quaternion rot = Quaternion.Euler(0, 0, angleOffset);
                bulletDir = rot * baseDir;

                spawnPos += (Vector3)(right.normalized * lateralSpacing * layer * side);
            }

            Quaternion bulletRot = Quaternion.Euler(
                0, 0,
                Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg - 90f
            );

            GameObject bullet = Instantiate(
                bulletPrefab,
                spawnPos,
                bulletRot
            );

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            // rb.linearVelocity = bulletDir * (bulletSpeed * Time.timeScale);
            rb.linearVelocity = bulletDir * bulletSpeed;
        }

    }

    GameObject GetNearestEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCol = enemy.GetComponent<Collider2D>();
            if (enemyCol == null) continue;

            Vector2 closestPoint = enemyCol.ClosestPoint(firePoint.position);
            float distance = Vector2.Distance(firePoint.position, closestPoint);

            if (distance <= shootRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    void AimAtTarget(Transform target)
    {
        if (target == null) return;

        Vector2 dir = target.position - RightHand.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        RightHand.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
    
}