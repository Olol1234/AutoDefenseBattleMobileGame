using UnityEngine;

public class GameClickManager : MonoBehaviour
{
    public LayerMask turretClickLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, turretClickLayer);

            if (hit.collider != null)
            {
                TurretRangeIndicator indicator =
                    hit.collider.GetComponent<TurretRangeIndicator>();

                if (indicator != null)
                    indicator.ShowRange();
            }
        }
    }
}
