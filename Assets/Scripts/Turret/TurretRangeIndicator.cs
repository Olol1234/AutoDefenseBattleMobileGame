using UnityEngine;
using System.Collections;

public class TurretRangeIndicator : MonoBehaviour
{
    public GameObject rangeVisual;
    public float showTime = 5f;

    private Coroutine showRoutine;

    // void OnMouseDown()
    // {
    //     ShowRange();
    //     Debug.Log("Turret Clicked!");
    // }

    public void ShowRange()
    {
        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        rangeVisual.SetActive(true);
        yield return new WaitForSeconds(showTime);
        rangeVisual.SetActive(false);
    }
}
