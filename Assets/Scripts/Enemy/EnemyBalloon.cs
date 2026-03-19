using UnityEngine;
using System.Collections;

public class EnemyBalloon : MonoBehaviour, IKnockbackable
{
    [Header("Movement")]
    public float speed = 2.5f;
    private Vector2 knockbackVelocity;
    [SerializeField] private float knockbackResist = 1f;
    private float screenLimitX;
    private Camera mainCam;
    private Rigidbody2D rb;


    [Header("Spawn Settings")]
    public GameObject[] minionPrefabs;
    public int spawnCount = 3;
    public float spawnSpread = 0.5f;

    void Start()
    {
        mainCam = Camera.main;
        UpdateScreenLimits();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void UpdateScreenLimits()
    {
        if (mainCam == null) return;
        float screenWidth = (mainCam.orthographicSize * 2f) * mainCam.aspect;
        screenLimitX = (screenWidth / 2f) - 0.4f;
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

        rb.linearVelocity = (Vector2.down * speed) + knockbackVelocity;

        UpdateScreenLimits();
        float clampedX = Mathf.Clamp(rb.position.x, -screenLimitX, screenLimitX);
        rb.position = new Vector2(clampedX, rb.position.y);
    }

    public void OnDeath()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, minionPrefabs.Length);
            
            Vector3 spawnOffset = new Vector3(Random.Range(-spawnSpread, spawnSpread), 0, 0);

            GameObject minion = ObjectPooler.Instance.GetFromPool(
                minionPrefabs[randomIndex], 
                transform.position + spawnOffset, 
                Quaternion.identity
            );

            // Can be optional
            if(minion.TryGetComponent(out IKnockbackable k))
            {
                Vector2 burstDir = (minion.transform.position - transform.position).normalized;
                k.ApplyKnockback(burstDir, 0.8f); 
            }
        }
        
        // Return the Balloon itself to the pool
        gameObject.SetActive(false);
    }

}