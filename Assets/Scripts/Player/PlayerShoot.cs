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
    private float fireTimer = 0f;
    // private ElementalType currentElement;

    void Update()
    {
        if (PauseManager.IsPaused)
        {
            return;
        }
        
        GameObject target = GetNearestEnemyInRange();

        if (target != null)
        {
            fireTimer += Time.deltaTime;
            AimAtTarget(target.transform);

            float speed = PlayerStats.Instance.GetAttackSpeed();

            if (fireTimer >= speed)
            {
                Shoot(target);
                fireTimer = 0f;
            }
            else
            {
                // could add a "charging" effect here based on fireTimer / speed
            }
        }
    }

    public void ResetFireTimer()
    {
        nextFireTime = Time.time;
    }

    void Shoot(GameObject target)
    {
        if (target == null) return;

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

            GameObject pooledBullet = ObjectPooler.Instance.GetFromPool(
                bulletPrefab,
                spawnPos,
                bulletRot
            );

            if (pooledBullet != null)
            {
                Bullet proj = pooledBullet.GetComponent<Bullet>();
                if (proj != null)
                {
                    // proj.Initialize(ElementalType.Physical, PlayerStats.Instance.GetDamage());
                    proj.Initialize(PlayerProfile.Instance.elementalType, PlayerStats.Instance.GetDamage());
                }
                Rigidbody2D rb = pooledBullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.simulated = true;
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;

                    rb.linearVelocity = bulletDir * bulletSpeed;

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayPlayerShoot();
                    }
                }
            }
            // Rigidbody2D rb = pooledBullet.GetComponent<Rigidbody2D>();
            // rb.linearVelocity = bulletDir * (bulletSpeed * Time.timeScale);
            // rb.linearVelocity = bulletDir * bulletSpeed;
        }

    }

    GameObject GetNearestEnemyInRange()
    {
        // GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var enemies = EnemyManager.Instance.ActiveEnemies;

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (HealthEnemy enemy in enemies)
        {
            // Collider2D enemyCol = enemy.GetComponent<Collider2D>();
            // if (enemyCol == null) continue;
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            // Vector2 closestPoint = enemyCol.ClosestPoint(firePoint.position);
            // float distance = Vector2.Distance(firePoint.position, closestPoint);
            float distance = Vector2.Distance(firePoint.position, enemy.transform.position);

            if (distance <= shootRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.gameObject;
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