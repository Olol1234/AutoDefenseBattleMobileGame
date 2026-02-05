using TMPro;
using UnityEngine;

public class StageResult : MonoBehaviour
{
    public TMP_Text resultText;
    public FortressHealth fortress;

    public void ShowFailResults()
    {
        Debug.Log("ShowFailResults() CALLED");
        float progress = LevelManager.Instance.GetClearProgress01() * 100f;
        int coinGained = LevelManager.Instance.CalculateCoins(false);
        resultText.text = $"Stage Progress: {progress:0}%\nCoins Earned: {coinGained}";
    }

    public void ShowClearResults()
    {
        float hpPercent = fortress.GetHPPercent01() * 100f;
        int coinGained = LevelManager.Instance.CalculateCoins(true);
        resultText.text = $"Fortress Integrity: {hpPercent:0}%\nCoins Earned: {coinGained}";
    }
}
