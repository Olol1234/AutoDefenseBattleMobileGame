using UnityEngine;
using UnityEngine.UI;

public class BottomNavManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject storePanel;
    public GameObject upgradePanel;
    public GameObject stagePanel;
    public GameObject eventPanel;
    public GameObject turretPanel;
    public TurretDetailPanel turretDetailPanel;

    [Header("Buttons")]
    public Image storeButtonImage;
    public Image upgradeButtonImage;
    public Image stageButtonImage;
    public Image eventButtonImage;
    public Image turretButtonImage;

    [Header("Colors")]
    private Color selectedColor = new Color32(0, 57, 255, 255);
    private Color normalColor = new Color32(0, 132, 255, 255);

    void Start()
    {
        ShowStage(); // Default tab
    }

    void HideAll()
    {
        storePanel.SetActive(false);
        upgradePanel.SetActive(false);
        stagePanel.SetActive(false);
        eventPanel.SetActive(false);
        turretPanel.SetActive(false);

        SetAllButtonsNormal();
    }

    void SetAllButtonsNormal()
    {
        storeButtonImage.color = normalColor;
        upgradeButtonImage.color = normalColor;
        stageButtonImage.color = normalColor;
        eventButtonImage.color = normalColor;
        turretButtonImage.color = normalColor;
    }

    public void ShowStore()
    {
        HideAll();
        turretDetailPanel.Hide();
        storePanel.SetActive(true);
        storeButtonImage.color = selectedColor;
    }

    public void ShowUpgrade()
    {
        HideAll();
        turretDetailPanel.Hide();
        upgradePanel.SetActive(true);
        upgradeButtonImage.color = selectedColor;
    }

    public void ShowStage()
    {
        HideAll();
        turretDetailPanel.Hide();
        stagePanel.SetActive(true);
        stageButtonImage.color = selectedColor;
    }

    public void ShowEvent()
    {
        HideAll();
        turretDetailPanel.Hide();
        eventPanel.SetActive(true);
        eventButtonImage.color = selectedColor;
    }

    public void ShowTurret()
    {
        HideAll();
        turretDetailPanel.Hide();
        turretPanel.SetActive(true);
        turretButtonImage.color = selectedColor;
    }
}
