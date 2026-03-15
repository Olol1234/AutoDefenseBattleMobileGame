using UnityEngine;
using System.Collections;
using NUnit.Framework;

public class EnemyMelee : MonoBehaviour, IKnockbackable
{
    [Header("Movement")]
    public float speed = 2.5f;
    public float stopDistanceOffset = 0.5f; // Very close to the fortress
    private Vector2 knockbackVelocity;
    [SerializeField] private float knockbackResist = 1f;
    
    [Header("Combat")]
    public float damage = 20f;
    public float attackCooldown = 1.5f;
    public float lungeDistance = 0.3f; // How much it "jumps" forward to hit

    private Rigidbody2D rb;
    private float stopY;
    private bool isAttacking = false;
    private float attackTimer;
    private Vector3 originalLocalPos;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void OnEnable()
    {
        isAttacking = false;
        attackTimer = 0f;
        CalculateStopPoint();
    }

    void CalculateStopPoint()
    {
        GameObject fortress = GameObject.FindGameObjectWithTag("Fortress");
        if (fortress != null)
        {
            // Stop just slightly above the fortress collider
            stopY = fortress.transform.position.y + stopDistanceOffset;
        }
    }

    public void ApplyKnockback(Vector2 pushDirection, float force)
    {
        knockbackVelocity += pushDirection.normalized * force;
    }

    void FixedUpdate()
    {
        if (PauseManager.IsPaused) 
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, knockbackResist * Time.fixedDeltaTime);
        if (knockbackVelocity.magnitude > 0.5f) 
        {
            isAttacking = false;
            // Optional: Stop the Lunge Coroutine if it was running
            StopAllCoroutines(); 
        }
        if (!isAttacking)
        {
            rb.linearVelocity = (Vector2.down * speed) + knockbackVelocity;

            if (rb.position.y <= stopY)
            {
                rb.position = new Vector2(rb.position.x, stopY);
                rb.linearVelocity = Vector2.zero;
                isAttacking = true;
            }
        }
        else
        {
            rb.linearVelocity = knockbackVelocity;
        }
    }

    void Update()
    {
        if (PauseManager.IsPaused || !isAttacking) return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            StartCoroutine(PerformMeleeAttack());
            attackTimer = 0f;
        }
    }

    IEnumerator PerformMeleeAttack()
    {
        // 1. Move up slightly (Wind up)
        Vector2 startPos = rb.position;
        Vector2 lungePos = startPos + (Vector2.down * lungeDistance);

        // 2. Quick Lunge Forward
        float t = 0;
        while (t < 1)
        {
            if (!PauseManager.IsPaused)
            {
                t += Time.deltaTime * 15f; // Fast lunge
                rb.MovePosition(Vector2.Lerp(startPos, lungePos, t));
            }
            yield return null;
        }

        // 3. Apply Damage to Fortress
        ApplyDamage();

        // 4. Return to original stop position
        t = 0;
        while (t < 1)
        {
            if (!PauseManager.IsPaused)
            {
                t += Time.deltaTime * 5f;
                rb.MovePosition(Vector2.Lerp(lungePos, startPos, t));
            }
            yield return null;
        }
    }

    void ApplyDamage()
    {
        GameObject fortress = GameObject.FindGameObjectWithTag("Fortress");
        if (fortress != null)
        {
            // if (LevelManager.Instance != null)
            //     damage *= LevelManager.Instance.GetDifficultyMultiplier();
            fortress.GetComponent<FortressHealth>()?.TakeDamage((int)damage);
        }
    }
}