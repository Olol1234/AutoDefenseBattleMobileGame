using UnityEngine;
using UnityEngine.UI;

public class TurretButtonUI : MonoBehaviour
{
    public TurretType turretType;
    public TurretDetailPanel turretDetailPanel;
    public int requiredStage;
    public Image lockOverlay;

    private void OnEnable()
    {
        CheckUnlockStatus();
    }

    public void OnClick()
    {   
        if (lockOverlay.gameObject.activeSelf)
        {
            return;
        }
        AudioManager.Instance.PlayClick();
        turretDetailPanel.Show(turretType);
    }

    public void CheckUnlockStatus()
    {
        var profile = PlayerProfile.Instance;

        if (profile.highestStageCleared >= requiredStage)
        {
            lockOverlay.gameObject.SetActive(false);
        }
        else
        {
            lockOverlay.gameObject.SetActive(true);
        }
    }
}
