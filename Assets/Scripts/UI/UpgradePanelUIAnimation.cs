using UnityEngine;
using System.Collections;

public class UIAppearAnimation : MonoBehaviour
{
    public float duration = 0.3f; // How fast it appears
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 originalScale;


    void Awake()
    {
        originalScale = transform.localScale; 
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AppearRoutine());
    }

    IEnumerator AppearRoutine()
    {
        transform.localScale = Vector3.zero;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; // Use unscaled because the game is paused!
            float progress = timer / duration;
            float curveValue = easeCurve.Evaluate(progress);

            transform.localScale = Vector3.one * curveValue;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}