using UnityEngine;
using TMPro;

public class TopNavManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private GameObject settingsPanel;

    void OnEnable()
    {
        PlayerProfile.OnProfileLoaded += RefreshCoins;

        if (PlayerProfile.Instance != null && PlayerProfile.Instance.IsLoaded)
            RefreshCoins();
    }

    void OnDisable()
    {
        PlayerProfile.OnProfileLoaded -= RefreshCoins;
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            AudioManager.Instance.PlayClick();
            Time.timeScale = 0f;
        }
    }

    void RefreshCoins()
    {
        coinsText.text = PlayerProfile.Instance.coins.ToString();
    }
}