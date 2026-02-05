using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float padding = 0.5f;

    private Camera mainCam;
    private float minX;
    private float maxX;

    private Vector3 mouseStartPos;
    private float lastMouseX;
    private bool dragging = false;
    [SerializeField]
    private float dragThreshold = 10f;
    private float targetX;

    void Start()
    {
        mainCam = Camera.main;
        CalculateBounds();
        targetX = transform.position.x;
    }

    void Update()
    {
        if (!GameInput.GameplayEnabled)
            return;

        // BLOCK UI CLICK (PC)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // BLOCK UI TOUCH (Mobile)
        if (Input.touchCount > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;
        }

        bool blockInput = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                blockInput = true;
            }
        }

        if (!blockInput)
        {
            HandleInput();
        }

        MovePlayer();
    }

    void HandleInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // Keyboard (PC testing)
        float horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontal) > 0.01f)
        {
            targetX += horizontal * moveSpeed * Time.deltaTime;
        }

        // Mouse drag (optional, nice for testing)
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            mouseStartPos = Input.mousePosition;
            lastMouseX = Input.mousePosition.x;
            dragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            float deltaPixels = Input.mousePosition.x - mouseStartPos.x;

            // Only start dragging if mouse moved enough
            if (Mathf.Abs(deltaPixels) > dragThreshold)
            {
                dragging = true;
            }

            if (dragging)
            {
                float currentMouseX = Input.mousePosition.x;
                float deltaX = currentMouseX - lastMouseX;

                float worldDeltaX = deltaX * 
                    (Camera.main.orthographicSize * 2f * Camera.main.aspect)
                    / Screen.width;

                targetX += worldDeltaX;
                lastMouseX = currentMouseX;
            }
        }

        if (Input.GetMouseButtonUp(0))
            dragging = false;

        // if (dragging && Input.GetMouseButton(0))
        // {
        //     Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        //     targetX = mouseWorld.x;
        // }
#else
        // Mobile touch drag
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            if (touch.phase == TouchPhase.Moved)
            {
                float deltaPixels = touch.deltaPosition.x;
                float worldDeltaX = deltaPixels * 
                    (mainCam.orthographicSize * 2f * mainCam.aspect)
                    / Screen.width;
                targetX += worldDeltaX;
            }
        }
#endif
    }

    void MovePlayer()
    {
        targetX = Mathf.Clamp(targetX, minX, maxX);

        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * moveSpeed);

        transform.position = pos;
    }

    void CalculateBounds()
    {
        Vector3 leftEdge = mainCam.ViewportToWorldPoint(new Vector3(0, 0, mainCam.nearClipPlane));
        Vector3 rightEdge = mainCam.ViewportToWorldPoint(new Vector3(1, 0, mainCam.nearClipPlane));

        minX = leftEdge.x + padding;
        maxX = rightEdge.x - padding;
    }

    // 🔥 IMPORTANT: call this when camera zoom changes
    public void RecalculateBounds()
    {
        CalculateBounds();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, Camera.main.nearClipPlane));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(leftEdge.x, transform.position.y - 10, 0),
                        new Vector3(leftEdge.x, transform.position.y + 10, 0));
        Gizmos.DrawLine(new Vector3(rightEdge.x, transform.position.y - 10, 0),
                        new Vector3(rightEdge.x, transform.position.y + 10, 0));
    }
#endif
}
