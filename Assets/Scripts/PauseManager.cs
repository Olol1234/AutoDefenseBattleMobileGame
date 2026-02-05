using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused = false;

    public GameObject pausePanel;

    private float previousGameSpeed;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        IsPaused = true;
        previousGameSpeed = Time.timeScale;
        Time.timeScale = 0f;
        if (pausePanel != null)
            pausePanel.SetActive(true);
        GameInput.GameplayEnabled = false;
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = previousGameSpeed;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        GameInput.GameplayEnabled = true;
    }

    public void QuitToMainMenu()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        Time.timeScale = 0f;
        LevelManager.Instance.EndStageLose();
        // if (LevelManager.Instance != null)
        // {
        //     LevelManager.Instance.ForceEndStageLose();
        // }
    }
}
