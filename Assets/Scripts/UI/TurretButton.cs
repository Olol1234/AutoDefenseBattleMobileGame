using UnityEngine;
using UnityEngine.UI;

public class TurretButtonUI : MonoBehaviour
{
    public TurretType turretType;
    public TurretDetailPanel turretDetailPanel;

    public void OnClick()
    {
        turretDetailPanel.Show(turretType);
    }
}
