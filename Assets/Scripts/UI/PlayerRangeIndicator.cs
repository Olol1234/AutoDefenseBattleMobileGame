using UnityEngine;

public class PlayerRangeIndicator : MonoBehaviour
{
    public Transform target;
    public GameObject indicatorVisual;
    public PlayerShoot playerShoot;

    public KeyCode toggleKey = KeyCode.R;

    void Start()
    {
        if (indicatorVisual != null)
            indicatorVisual.SetActive(false);

        UpdateScale();
    }

    void Update()
    {
        HandleToggle();
        FollowTarget();
        UpdateScale();
    }

    void HandleToggle()
    {
        if (Input.GetKeyDown(toggleKey) && indicatorVisual != null)
        {
            indicatorVisual.SetActive(!indicatorVisual.activeSelf);
        }
    }

    void FollowTarget()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }

    void UpdateScale()
    {
        if (indicatorVisual != null && playerShoot != null)
        {
            float diameter = playerShoot.shootRange * 2f;
            indicatorVisual.transform.localScale =
                new Vector3(diameter, diameter, 1f);
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        UpdateScale();
    }
#endif
}
