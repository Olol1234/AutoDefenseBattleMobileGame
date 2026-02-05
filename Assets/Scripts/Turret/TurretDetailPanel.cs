using UnityEngine;
using TMPro;

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
            // damageText.text = "Damage: " + profile.homingMissileTurretBaseDamage;
            damageText.text = profile.homingMissileTurretBaseDamage.ToString("F0");
            // nextDamageText.text = "Next: " + TurretUpgradeHelper.GetNextDamageValue(type);
            nextDamageText.text = TurretUpgradeHelper.GetNextDamageValue(type).ToString("F0");
            // cooldownText.text = "Cooldown: " + profile.homingMissileTurretCooldown;
            cooldownText.text = profile.homingMissileTurretCooldown.ToString("F0");
            // nextCooldownText.text = "Next: " + TurretUpgradeHelper.GetNextCooldownValue(type);
            nextCooldownText.text = TurretUpgradeHelper.GetNextCooldownValue(type).ToString("F0");
            int cost = TurretUpgradeHelper.GetDamageUpgradeCost(type);
            upgradeDamageCostText.text = "Coin: " + cost;
            int cooldownCost = TurretUpgradeHelper.GetCooldownUpgradeCost(type);
            upgradeCooldownCostText.text = "Coin: " + cooldownCost;
        }
            // Future turret types can be handled here

        // int cost = TurretUpgradeHelper.GetDamageUpgradeCost(type);
        // upgradeCostText.text = "Cost: " + cost;

    }

    public void Hide()
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
        bool success = TurretUpgradeHelper.UpgradeDamage(currentTurretType);

        if (success)
            Show(currentTurretType); // refresh UI

        PlayerProfile.OnProfileLoaded?.Invoke();
    }

    public void OnUpgradeCooldownClicked()
    {
        bool success = TurretUpgradeHelper.UpgradeCooldown(currentTurretType);

        if (success)
            Show(currentTurretType);

        PlayerProfile.OnProfileLoaded?.Invoke();
    }

}
