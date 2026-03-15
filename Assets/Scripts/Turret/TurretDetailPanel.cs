using UnityEngine;
using TMPro;
using System.Collections;

public class TurretDetailPanel : MonoBehaviour
{
    // public static TurretDetailPanel Instance;
    // public TopNavManager topNavManager;

    public GameObject damageRow;
    public GameObject cooldownRow;

    public TMP_Text turretNameText;
    public TMP_Text damageText;
    public TMP_Text nextDamageText;
    public TMP_Text cooldownText;
    public TMP_Text nextCooldownText;
    public TMP_Text upgradeDamageCostText;
    public TMP_Text upgradeCooldownCostText;

    public GameObject maxUpgradePanel;
    public GameObject panelRoot;

    private TurretType currentTurretType;

    public void Show(TurretType type)
    {
        currentTurretType = type;
        panelRoot.SetActive(true);
        HideAllRows();

        var profile = PlayerProfile.Instance;

        if (type == TurretType.HomingMissileTurret)
        {
            damageRow.SetActive(true);
            cooldownRow.SetActive(true);

            turretNameText.text = "Homing Missile Turret";
            damageText.text = profile.homingMissileTurretBaseDamage.ToString("F0");
            nextDamageText.text = TurretUpgradeHelper.GetNextDamageValue(type).ToString("F0");
            cooldownText.text = profile.homingMissileTurretCooldown.ToString("F2");
            nextCooldownText.text = TurretUpgradeHelper.GetNextCooldownValue(type).ToString("F2");
            int cost = TurretUpgradeHelper.GetDamageUpgradeCost(type);
            upgradeDamageCostText.text = "Coin: " + cost;
            int cooldownCost = TurretUpgradeHelper.GetCooldownUpgradeCost(type);
            upgradeCooldownCostText.text = "Coin: " + cooldownCost;
        }

        if (type == TurretType.LaserTurret)
        {
            damageRow.SetActive(true);
            cooldownRow.SetActive(true);

            turretNameText.text = "Laser Turret";
            damageText.text = profile.laserTurretBaseDPS.ToString("F0");
            nextDamageText.text = TurretUpgradeHelper.GetNextDamageValue(type).ToString("F0");
            cooldownText.text = profile.laserTurretCooldown.ToString("F2");
            nextCooldownText.text = TurretUpgradeHelper.GetNextCooldownValue(type).ToString("F2");
            int cost = TurretUpgradeHelper.GetDamageUpgradeCost(type);
            upgradeDamageCostText.text = "Coin: " + cost;
            int cooldownCost = TurretUpgradeHelper.GetCooldownUpgradeCost(type);
            upgradeCooldownCostText.text = "Coin: " + cooldownCost;
        }

        if (type == TurretType.ShotgunTurret)
        {
            damageRow.SetActive(true);
            cooldownRow.SetActive(true);

            turretNameText.text = "Shotgun Turret";
            damageText.text = profile.shotgunTurretBaseDamage.ToString("F0");
            nextDamageText.text = TurretUpgradeHelper.GetNextDamageValue(type).ToString("F0");
            cooldownText.text = profile.shotgunTurretCooldown.ToString("F2");
            nextCooldownText.text = TurretUpgradeHelper.GetNextCooldownValue(type).ToString("F2");
            int cost = TurretUpgradeHelper.GetDamageUpgradeCost(type);
            upgradeDamageCostText.text = "Coin: " + cost;
            int cooldownCost = TurretUpgradeHelper.GetCooldownUpgradeCost(type);
            upgradeCooldownCostText.text = "Coin: " + cooldownCost;
        }

    }

    public void Hide()
    {
        AudioManager.Instance.PlayCancel();
        panelRoot.SetActive(false);
    }

    public void ClosePanelRoot()
    {
        panelRoot.SetActive(false);
    }

    void HideAllRows()
    {
        damageRow.SetActive(false);
        cooldownRow.SetActive(false);
    }

    public void OnUpgradeDamageClicked()
    {
        AudioManager.Instance.PlayClick();

        bool success = TurretUpgradeHelper.UpgradeDamage(currentTurretType);

        if (success)
        {
            Show(currentTurretType); // refresh UI
        }
        else
        {
            StartCoroutine(ShowMaxUpgradePanelRoutine());
        }

        PlayerProfile.OnProfileLoaded?.Invoke();
    }

    public void OnUpgradeCooldownClicked()
    {
        AudioManager.Instance.PlayClick();

        bool success = TurretUpgradeHelper.UpgradeCooldown(currentTurretType);

        if (success)
        {
            Show(currentTurretType); // refresh UI
        }
        else
        {
            StartCoroutine(ShowMaxUpgradePanelRoutine());
        }

        PlayerProfile.OnProfileLoaded?.Invoke();
    }

    private IEnumerator ShowMaxUpgradePanelRoutine()
    {
        maxUpgradePanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        maxUpgradePanel.SetActive(false);
    }

}
