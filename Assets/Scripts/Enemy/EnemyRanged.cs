using UnityEngine;

public class EnemyRanged : MonoBehaviour, IKnockbackable
{
    [Header("Movement")]
    public float speed = 2f;
    public float stopDistanceOffset = 2f;
    private Vector2 knockbackVelocity;
    [SerializeField] private float knockbackResist = 1f;
    private float screenLimitX;
    private Camera mainCam;
    
    [Header("Combat")]
    public GameObject projectilePrefab;
    public float fireRate = 2f;
    public float damage = 10f;
    public Transform firePoint;

    private Rigidbody2D rb;
    private float stopY;
    private bool isAttacking = false;
    private float fireTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        mainCam = Camera.main;
        UpdateScreenLimits();
    }

    void UpdateScreenLimits()
    {
        if (mainCam == null) return;
        float screenWidth = (mainCam.orthographicSize * 2f) * mainCam.aspect;
        screenLimitX = (screenWidth / 2f) - 0.4f;
    }

    void OnEnable()
    {
        // Reset states for object pooling
        isAttacking = false;
        fireTimer = 0f;
        
        CalculateStopPoint();
    }

    void CalculateStopPoint()
    {
        // Find the fortress to know where to stop
        GameObject fortress = GameObject.FindGameObjectWithTag("Fortress");
        if (fortress != null)
        {
            // If fortress is at Y=0, and we want to stop 2 units above it:
            // stopY = 0 + 2 = 2.
            stopY = fortress.transform.position.y + stopDistanceOffset;
        }
        else
        {
            stopY = 0f; // Fallback
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

        if (!isAttacking)
        {
            // Simple downward movement
            rb.linearVelocity = (Vector2.down * speed) + knockbackVelocity;
            knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, knockbackResist * Time.fixedDeltaTime);

            // Clamp X position within screen limits
            UpdateScreenLimits();
            float clampedX = Mathf.Clamp(rb.position.x, -screenLimitX, screenLimitX);
            rb.position = new Vector2(clampedX, rb.position.y);

            // Cheap float comparison instead of Distance math
            if (rb.position.y <= stopY)
            {
                rb.position = new Vector2(rb.position.x, stopY); // Snap to position
                rb.linearVelocity = Vector2.zero;
                isAttacking = true;
            }
        }
    }

    void Update()
    {
        if (PauseManager.IsPaused || !isAttacking) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void Shoot()
    {
        GameObject bulletObj = ObjectPooler.Instance.GetFromPool(projectilePrefab, firePoint.position, Quaternion.identity);

        if (bulletObj != null)
        {
            EnemyBullet bulletScript = bulletObj.GetComponent<EnemyBullet>();
            if (bulletScript != null)
            {
                bulletScript.Setup(damage);
            }
        }
    }
}