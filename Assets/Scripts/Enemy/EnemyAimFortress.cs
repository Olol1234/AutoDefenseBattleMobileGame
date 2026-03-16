using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAimFortress : MonoBehaviour, IKnockbackable
{
    public float speed = 2.0f;
    public float stopDistance = 0.05f;
    private Vector2 knockbackVelocity;
    [SerializeField] private float knockbackResist = 1f;

    private float screenLimitX;
    private Camera mainCam;

    Rigidbody2D rb;
    Vector2 targetPoint;
    bool targetLocked;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        LockRandomFortressPoint();
        mainCam = Camera.main;
        UpdateScreenLimits();
    }

    void UpdateScreenLimits()
    {
        if (mainCam == null) return;
        float screenWidth = (mainCam.orthographicSize * 2f) * mainCam.aspect;
        screenLimitX = (screenWidth / 2f) - 0.4f;
    }

    public void LockRandomFortressPoint()
    {
        GameObject fortress = GameObject.FindGameObjectWithTag("Fortress");
        if (fortress == null)
        {
            // Debug.LogError("No object tagged 'Fortress' found.");
            return;
        }

        FortressEdgeTargets edgeTargets = fortress.GetComponent<FortressEdgeTargets>();
        if (edgeTargets == null)
        {
            // Debug.LogError("Fortress is missing FortressEdgeTargets component.");
            return;
        }

        targetPoint = edgeTargets.GetRandomEdgePoint();
        targetLocked = true;
    }

    void FixedUpdate()
    {
        if (!targetLocked) return;

        Vector2 pos = rb.position;
        float dist = Vector2.Distance(pos, targetPoint);

        if (dist <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = (targetPoint - pos).normalized;
        // rb.linearVelocity = dir * speed;
        Vector2 movementVelocity = dir * speed;

        rb.linearVelocity = movementVelocity + knockbackVelocity;
        knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, knockbackResist * Time.fixedDeltaTime);
    }

    public void ApplyKnockback(Vector2 pushDirection, float force)
    {
        knockbackVelocity += pushDirection.normalized * force;
    }

    // Optional: visualize chosen target in Scene view
    void OnDrawGizmosSelected()
    {
        if (!targetLocked) return;
        Gizmos.DrawSphere(targetPoint, 0.08f);
        Gizmos.DrawLine(transform.position, targetPoint);
    }
}
