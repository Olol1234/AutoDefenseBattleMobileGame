using NUnit.Framework;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public static bool IsPaused = false;
    public static bool IsSystemPaused = false;

    public GameObject pausePanel;
    public GameObject settingPanel;

    private float previousGameSpeed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void TogglePause()
    {
        if (IsSystemPaused) return;

        if (IsPaused)
        {
            Resume();
            if (pausePanel != null)
            pausePanel.SetActive(false);
        }
        else
        {
            Pause();
            if (pausePanel != null)
            pausePanel.SetActive(true);
        }
    }

    public void Pause()
    {
        IsPaused = true;
        previousGameSpeed = Time.timeScale;
        // Time.timeScale = 0f;
        Rigidbody2D[] allBodies = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
        foreach (var rb in allBodies) {
            rb.simulated = false;
        }
        // if (pausePanel != null)
        //     pausePanel.SetActive(true);
        GameInput.GameplayEnabled = false;
        // if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void OpenSettings()
    {
        if (settingPanel != null)
            settingPanel.SetActive(true);
    }

    public void Resume()
    {
        if (IsSystemPaused) return;
        IsPaused = false;
        Time.timeScale = previousGameSpeed;
        Rigidbody2D[] allBodies = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
        foreach (var rb in allBodies) {
            rb.simulated = true;
        }
        if (pausePanel != null)
            pausePanel.SetActive(false);
        GameInput.GameplayEnabled = true;
    }

    public void SystemPause()
    {
        IsSystemPaused = true;
        Pause();
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void SystemResume()
    {
        IsSystemPaused = false;
        Resume();
    }

    public void QuitToMainMenu()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        IsPaused = false;
        Time.timeScale = 0f;
        LevelManager.Instance.EndStageLose();
        // if (LevelManager.Instance != null)
        // {
        //     LevelManager.Instance.ForceEndStageLose();
        // }
    }
}
