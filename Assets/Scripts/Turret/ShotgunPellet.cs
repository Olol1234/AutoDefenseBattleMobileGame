using UnityEngine;

public class ShotgunPellet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 12f;
    private Rigidbody2D rb;

    [Header("Explosion Settings")]
    private float baseDamage;
    private float coneAngle;
    private float coneRadius;
    public LayerMask enemyLayerMask;

    [HideInInspector] public GameObject myPrefab;
    public float lifetime = 5f;
    private float currentLifetime;

    public void Initialize(float dmg, float angle, float radius)
    {
        baseDamage = dmg;
        coneAngle = angle;
        coneRadius = radius;
    }

    void Update()
    {
        if (PauseManager.IsPaused) return;

        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifetime)
        {
            ObjectPooler.Instance.ReturnToPool(myPrefab, gameObject);
        }
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnEnable()
    {
        currentLifetime = 0f;
        // Safety resets
        transform.localScale = new Vector3(0.1f, 0.2f, 1f); 
        // if (rb != null) rb.linearVelocity = Vector2.zero;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true; 
            rb.linearVelocity = Vector2.zero; 
            rb.angularVelocity = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Explode();
            // Return to pool after explosion
            ObjectPooler.Instance.ReturnToPool(myPrefab, gameObject);
        }
    }

    void Explode()
    {
        // Vector2 explosionOrigin = transform.position;
        Vector2 explosionOrigin = (Vector2)transform.position - ((Vector2)transform.up * 0.5f);
        Vector2 fireDirection = transform.up;

        // 1. Find all potential targets in the radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(explosionOrigin, coneRadius, enemyLayerMask);

        foreach (var hit in hitEnemies)
        {
            HealthEnemy enemy = hit.GetComponent<HealthEnemy>();
            if (enemy == null) continue;

            // Vector2 dirToEnemy = (Vector2)hit.transform.position - explosionOrigin;
            Vector2 closestPoint = hit.ClosestPoint(explosionOrigin);
            Vector2 dirToEnemy = closestPoint - explosionOrigin;
            float distance = dirToEnemy.magnitude;

            // Angle check based on movement direction
            if (Vector2.Angle(fireDirection, dirToEnemy) <= coneAngle / 2f)
            {
                float falloff = 1f - Mathf.Clamp01(distance / coneRadius);

                float finalDamage = baseDamage * falloff;
                float finalKnockback = ShotgunBrain.Instance.GetKnockbackForce() * falloff;

                enemy.TakeDamage(finalDamage, ElementalType.Physical);
                enemy.RequestKnockback(dirToEnemy.normalized, finalKnockback);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 rightEdge = Quaternion.AngleAxis(coneAngle / 2f, Vector3.forward) * transform.up;
        Vector3 leftEdge = Quaternion.AngleAxis(-coneAngle / 2f, Vector3.forward) * transform.up;

        Gizmos.DrawRay(transform.position, rightEdge * coneRadius);
        Gizmos.DrawRay(transform.position, leftEdge * coneRadius);
        // Draw an arc or line connecting them
        Gizmos.DrawLine(transform.position + rightEdge * coneRadius, transform.position + leftEdge * coneRadius);
    }
}