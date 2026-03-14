using UnityEngine;

public class ZigZagMovement : MonoBehaviour, IKnockbackable
{
    [Header("Movement Settings")]
    public float verticalSpeed = 2f;
    public float horizontalSpeed = 3f;
    private Vector2 knockbackVelocity;
    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;
    public float leanAngle = 15f;
    
    private int directionX = 1; // 1 for Right, -1 for Left
    private float screenLimitX;

    void Start()
    {
        CalculateScreenEdges();
    }

    void CalculateScreenEdges()
    {
        // Get the horizontal edge based on Camera Zoom
        Camera mainCam = Camera.main;
        float screenHeight = mainCam.orthographicSize * 2f;
        float screenWidth = screenHeight * mainCam.aspect;
        
        // Subtract a small offset (0.5f) so the enemy doesn't half-disappear
        screenLimitX = (screenWidth / 2f) - 0.5f;
    }

    void Update()
    {
        if (PauseManager.IsPaused) return;

        // // Downward Movement
        // float moveY = -verticalSpeed * Time.deltaTime;
        // // Horizontal Zig-Zag Movement
        // float moveX = horizontalSpeed * directionX * Time.deltaTime;

        // transform.Translate(new Vector3(moveX, moveY, 0), Space.World);

        Vector3 movement = new Vector3(horizontalSpeed * directionX, -verticalSpeed, 0) * Time.deltaTime;
        // transform.Translate(movement + (knockbackVelocity * Time.deltaTime), Space.World);
        transform.Translate(movement + (new Vector3(knockbackVelocity.x, knockbackVelocity.y, 0) * Time.deltaTime), Space.World);
        knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, Time.deltaTime * 1f); // Smoothly reduce knockback over time

        // Rotate to Lean in the Direction of Movement
        // float targetAngle = directionX * leanAngle;
        // Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // spinning
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);

        // Check for Camera Edges
        if (transform.position.x >= screenLimitX)
        {
            directionX = -1;
            transform.position = new Vector3(screenLimitX, transform.position.y, 0);
        }
        else if (transform.position.x <= -screenLimitX)
        {
            directionX = 1;
            transform.position = new Vector3(-screenLimitX, transform.position.y, 0);
        }
    }

    public void ApplyKnockback(Vector2 pushDirection, float force)
    {
        knockbackVelocity += pushDirection.normalized * force;
    }

}