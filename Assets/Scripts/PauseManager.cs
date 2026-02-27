using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public static bool IsPaused = false;

    public GameObject pausePanel;

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
    }

    public void Resume()
    {
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
