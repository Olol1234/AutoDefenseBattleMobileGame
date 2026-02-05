using TMPro;
using UnityEngine;

public class StageTimer : MonoBehaviour
{
    public TMP_Text timerText;

    void Update()
    {
        if (LevelManager.Instance == null) return;

        float t = LevelManager.Instance.ElapsedTime;

        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
