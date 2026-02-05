using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAimFortress : MonoBehaviour
{
    public float speed = 2.0f;
    public float stopDistance = 0.05f;

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
    }

    public void LockRandomFortressPoint()
    {
        GameObject fortress = GameObject.FindGameObjectWithTag("Fortress");
        if (fortress == null)
        {
            Debug.LogError("No object tagged 'Fortress' found.");
            return;
        }

        FortressEdgeTargets edgeTargets = fortress.GetComponent<FortressEdgeTargets>();
        if (edgeTargets == null)
        {
            Debug.LogError("Fortress is missing FortressEdgeTargets component.");
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
        // rb.linearVelocity = dir * (speed* Time.timeScale);
        rb.linearVelocity = dir * speed;
        // Debug.Log($"Enemy moving with speed: {speed}");
    }

    // Optional: visualize chosen target in Scene view
    void OnDrawGizmosSelected()
    {
        if (!targetLocked) return;
        Gizmos.DrawSphere(targetPoint, 0.08f);
        Gizmos.DrawLine(transform.position, targetPoint);
    }
}
