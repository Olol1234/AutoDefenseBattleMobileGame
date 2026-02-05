using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerExpBar : MonoBehaviour
{
    public Image expFillImage;
    public TMP_Text levelText;

    void Update()
    {
        if (PlayerExp.Instance == null) return;

        float fillAmount = (float)PlayerExp.Instance.currentExp 
                           / PlayerExp.Instance.expToNextLevel;

        expFillImage.fillAmount = fillAmount;

        if (levelText != null)
        {
            levelText.text = "Lv " + PlayerExp.Instance.level;
        }
    }

}
