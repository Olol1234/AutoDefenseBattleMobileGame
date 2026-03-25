using UnityEngine;
using TMPro;
using System.Collections;

public class CharacterUpgradePanel : MonoBehaviour
{
    [Header("Damage UI")]
    public TextMeshProUGUI damageValueText;
    public TextMeshProUGUI damageNextText;
    public TextMeshProUGUI damageCostText;
    public UnityEngine.UI.Button damageUpgradeButton;

    [Header("Player Element")]
    public TMP_Dropdown elementDropdown;

    [Header("HP UI")]
    public TextMeshProUGUI hpValueText;
    public TextMeshProUGUI hpNextText;
    public TextMeshProUGUI hpCostText;
    public UnityEngine.UI.Button hpUpgradeButton;

    [Header("Fire Rate UI")]
    public TextMeshProUGUI fireRateValueText;
    public TextMeshProUGUI fireRateNextText;
    public TextMeshProUGUI fireRateCostText;
    public UnityEngine.UI.Button fireRateUpgradeButton;

    [Header("Max Upgrade Panel")]
    public GameObject maxUpgradePanel;

    void OnEnable()
    {
        PlayerProfile.OnProfileLoaded += HandleProfileLoaded;
        if (PlayerProfile.Instance != null && PlayerProfile.Instance.IsLoaded)
        {
            elementDropdown.SetValueWithoutNotify((int)PlayerProfile.Instance.elementalType);
        }
        TryRefreshUI();
    }

    void OnDisable()
    {
        PlayerProfile.OnProfileLoaded -= HandleProfileLoaded;
    }

    void HandleProfileLoaded()
    {
        if (elementDropdown != null && PlayerProfile.Instance != null)
        {
            int savedIndex = (int)PlayerProfile.Instance.elementalType;
            elementDropdown.SetValueWithoutNotify(savedIndex);
        }
        RefreshUI();
    }

    void TryRefreshUI()
    {
        if (PlayerProfile.Instance == null)
            return;

        if (!PlayerProfile.Instance.IsLoaded)
            return;

        RefreshUI();
    }

    public void RefreshUI()
    {
        var profile = PlayerProfile.Instance;

        // DAMAGE
        float dmg = profile.baseDamage;
        float dmgInc = 10f;
        int dmgCost = GetDamageUpgradeCost();

        damageValueText.text = dmg.ToString("0");
        damageNextText.text = (dmg + dmgInc).ToString("0");
        damageCostText.text = dmgCost.ToString();

        damageUpgradeButton.interactable = profile.coins >= dmgCost;

        // HP
        float hp = profile.baseFortressMaxHP;
        float hpInc = 500f;
        int hpCost = GetHPUpgradeCost();

        hpValueText.text = hp.ToString("0");
        hpNextText.text = (hp + hpInc).ToString("0");
        hpCostText.text = hpCost.ToString();

        hpUpgradeButton.interactable = profile.coins >= hpCost;

        // FIRE RATE
        float fr = profile.baseFireRate;
        float frInc = 0.1f;
        int frCost = GetFireRateUpgradeCost();

        fireRateValueText.text = fr.ToString("0.0");
        fireRateNextText.text = (fr - frInc).ToString("0.0");
        fireRateCostText.text = frCost.ToString();

        fireRateUpgradeButton.interactable = profile.coins >= frCost;
    }

    // ===== Element Dropdown Hook =====

    public void ChangeElement(int index)
    {
        AudioManager.Instance.PlayClick();

        var profile = PlayerProfile.Instance;

        // Converts the 0, 1, 2 index directly to Physical, Fire, Energy
        profile.elementalType = (ElementalType)index; 

        // Save immediately so the choice persists
        profile.SaveToDisk(); 

        Debug.Log("Element updated to: " + profile.elementalType);
    }

    // ===== Button Hooks =====

    public void UpgradeDamage()
    {
        AudioManager.Instance.PlayClick();

        int cost = GetDamageUpgradeCost();
        var profile = PlayerProfile.Instance;

        if (profile.coins < cost) return;

        profile.coins -= cost;
        profile.baseDamage += 10f;
        profile.SaveToDisk();

        PlayerProfile.OnProfileLoaded?.Invoke();

        RefreshUI();
    }

    public void UpgradeHP()
    {
        AudioManager.Instance.PlayClick();

        int cost = GetHPUpgradeCost();
        var profile = PlayerProfile.Instance;

        if (profile.coins < cost) return;

        profile.coins -= cost;
        profile.baseFortressMaxHP += 500f;
        profile.SaveToDisk();

        PlayerProfile.OnProfileLoaded?.Invoke();

        RefreshUI();
    }

    public void UpgradeFireRate()
    {
        if (PlayerProfile.Instance.fireRateLevel >= 41)
        {
            StartCoroutine(ShowMaxUpgradePanelRoutine());
            return;
        }

        AudioManager.Instance.PlayClick();
        
        int cost = GetFireRateUpgradeCost();
        var profile = PlayerProfile.Instance;

        if (profile.coins < cost) return;

        profile.coins -= cost;
        profile.baseFireRate -= 0.05f;
        profile.fireRateLevel++;
        profile.SaveToDisk();

        PlayerProfile.OnProfileLoaded?.Invoke();

        RefreshUI();
    }

    // ===== Cost Scaling =====

    int GetDamageUpgradeCost()
    {
        float dmg = PlayerProfile.Instance.baseDamage;
        return Mathf.RoundToInt(100 + dmg * 5f);
    }

    int GetHPUpgradeCost()
    {
        float hp = PlayerProfile.Instance.baseFortressMaxHP;
        return Mathf.RoundToInt(80 + hp * 1f);
    }

    int GetFireRateUpgradeCost()
    {
        float fr = PlayerProfile.Instance.baseFireRate;
        // return Mathf.RoundToInt(120 + fr * 100f);
        return Mathf.RoundToInt(120 + 1 * Mathf.Pow(1.3f, PlayerProfile.Instance.fireRateLevel - 1));
    }

    private IEnumerator ShowMaxUpgradePanelRoutine()
    {
        maxUpgradePanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        maxUpgradePanel.SetActive(false);
    }
}
