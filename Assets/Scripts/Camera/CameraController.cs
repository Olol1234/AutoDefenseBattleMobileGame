using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    public Transform backgroundTransform;

    [Header("Zoom Settings")]
    public float zoomSpeed = 1.5f;

    private float baseSize;
    private float baseY;
    private Coroutine zoomRoutine;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        baseSize = cam.orthographicSize;
        baseY = cam.transform.position.y;
    }

    /// <summary>
    /// Smooth zoom while keeping bottom anchored
    /// </summary>
    public void SmoothZoomKeepingBottom(float targetSize)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(ZoomRoutine(targetSize));
    }

    public void ResetZoomSmooth()
    {
        SmoothZoomKeepingBottom(baseSize);
    }

    IEnumerator ZoomRoutine(float targetSize)
    {
        float startSize = cam.orthographicSize;
        float startY = cam.transform.position.y;

        float targetY = baseY + (targetSize - baseSize);

        Vector3 startScale = backgroundTransform.localScale;
        Vector3 targetScale = startScale * (targetSize / startSize);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;

            float size = Mathf.Lerp(startSize, targetSize, t);
            float y = Mathf.Lerp(startY, targetY, t);

            cam.orthographicSize = size;
            cam.transform.position = new Vector3(
                cam.transform.position.x,
                y,
                cam.transform.position.z
            );

            backgroundTransform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        cam.orthographicSize = targetSize;
        cam.transform.position = new Vector3(
            cam.transform.position.x,
            targetY,
            cam.transform.position.z
        );
        backgroundTransform.localScale = targetScale;
    }
}
