using UnityEngine;
using UnityEngine.UI;

public class TurretButtonUI : MonoBehaviour
{
    public TurretType turretType;
    public TurretDetailPanel turretDetailPanel;

    public void OnClick()
    {
        AudioManager.Instance.PlayClick();
        turretDetailPanel.Show(turretType);
    }
}
