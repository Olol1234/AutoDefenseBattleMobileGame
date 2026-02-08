using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    public GameObject upgradePanel;
    public TMP_Text[] upgradeTexts;
    private List<UpgradeData> currentUpgrades;
    private float previousTimeScale;

    // COLOR FOR BORDER BASED ON RARITY
    // public Button[] upgradeButtons;
    public Image[] upgradeBorders;
    public Color commonColor = Color.white;
    public Color uncommonColor = Color.green;
    public Color rareColor = Color.blue;
    public Color epicColor = new Color(0.6f, 0f, 1f);
    public Color legendaryColor = Color.yellow;

    private void OnEnable()
    {
        PlayerExp.OnLevelUpEvent += ShowPanel;
    }

    private void OnDisable()
    {
        PlayerExp.OnLevelUpEvent -= ShowPanel;
    }

    Color GetRarityColor(UpgradeRarity rarity)
    {
        switch(rarity)
        {
            case UpgradeRarity.Common: return commonColor;
            case UpgradeRarity.Uncommon: return uncommonColor;
            case UpgradeRarity.Rare: return rareColor;
            case UpgradeRarity.Epic: return epicColor;
            case UpgradeRarity.Legendary: return legendaryColor;
        }

        return commonColor;
    }

    void ShowPanel()
    {
        GameInput.GameplayEnabled = false;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);

        currentUpgrades = UpgradeManager.Instance.GetRandomUpgrades(4);

        for (int i = 0; i < upgradeTexts.Length; i++)
        {
            upgradeTexts[i].text =
                currentUpgrades[i].name + "\n" +
                currentUpgrades[i].description;

            var borderImage = upgradeBorders[i];
            borderImage.color = GetRarityColor(currentUpgrades[i].rarity);
        }
    }

    // Hook this to button OnClick for now
    public void OnUpgradeSelected(int index)
    {
        currentUpgrades[index].Apply();

        GameInput.GameplayEnabled = true;
        Time.timeScale = previousTimeScale;
        upgradePanel.SetActive(false);
    }
}
