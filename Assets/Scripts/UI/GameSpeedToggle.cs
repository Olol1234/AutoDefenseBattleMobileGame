using TMPro;
using UnityEngine;

public class GameSpeedToggle : MonoBehaviour
{
    public TMP_Text speedText;

    private float[] speeds = { 1f, 1.5f, 2f };
    private int currentIndex = 0;

    void Start()
    {
        ApplySpeed();
    }

    public void ToggleSpeed()
    {
        currentIndex++;

        if (currentIndex >= speeds.Length)
            currentIndex = 0;

        ApplySpeed();
    }

    void ApplySpeed()
    {
        float speed = speeds[currentIndex];

        LevelManager.Instance.SetGameSpeed(speed);
        Debug.Log("Speed set to: " + speed);

        if (speedText != null)
            speedText.text = speed.ToString("0.#") + "x";
    }
}
