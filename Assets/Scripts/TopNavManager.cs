using UnityEngine;
using TMPro;

public class TopNavManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;
    // private TMP_Text gemsText;

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

    void RefreshCoins()
    {
        coinsText.text = PlayerProfile.Instance.coins.ToString();
    }
}