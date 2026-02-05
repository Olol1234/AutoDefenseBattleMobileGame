using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UpgradePanel : MonoBehaviour
{
    public GameObject upgradePanel;
    public TMP_Text[] upgradeTexts;
    private List<UpgradeData> currentUpgrades;
    private float previousTimeScale;

    private void OnEnable()
    {
        PlayerExp.OnLevelUpEvent += ShowPanel;
    }

    private void OnDisable()
    {
        PlayerExp.OnLevelUpEvent -= ShowPanel;
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
