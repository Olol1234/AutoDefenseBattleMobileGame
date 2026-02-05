using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FortressHealthBar : MonoBehaviour
{
    public FortressHealth fortressHealth;
    public Image fillImage;
    public TMP_Text hpText;

    void Update()
    {
        if (fortressHealth == null || fillImage == null) return;

        float percent = (float)fortressHealth.CurrentHealth / fortressHealth.MaxHealth;
        fillImage.fillAmount = percent;

        if (hpText != null)
        {
            hpText.text = $"{fortressHealth.CurrentHealth} / {fortressHealth.MaxHealth}";
        }
    }
}