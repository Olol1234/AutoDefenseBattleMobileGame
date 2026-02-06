using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public GameObject gameOverPanel;
    public GameObject stageClearPanel;
    public StageResult winResultsUI;
    public StageResult loseResultsUI;
    public TMP_Text stageTitleText;
    public TurretSlotManager slotManager;
    private bool stageTimeFinished = false;

    [Header("Level Duration")]
    [SerializeField] private float levelDuration = 180f;

    [Header("Difficulty Scaling")]
    [SerializeField] private float stepInterval = 20f;
    [SerializeField] private float stepMultiplier = 1.25f;

    [Header("Speed")]
    [SerializeField] private float gameSpeed = 1f;

    [Header("Stage")]
    public int currentStage = 1;

    private float elapsedTime = 0f;
    private bool stageEnded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;            
        }
        currentStage = PlayerPrefs.GetInt("SelectedStage", 1);
    }

    // void Start()
    // {
    //     FortressHealth fortress = FindObjectOfType<FortressHealth>();
    //     if (fortress != null)
    //     {
    //         fortress.InitFromProfile();
    //     }
    //     if (stageTitleText != null)
    //         stageTitleText.text = $"Stage {currentStage}";
    // }
    IEnumerator Start()
    {
        while (!PlayerProfile.Instance.IsLoaded)
            yield return null;

        PlayerStats.Instance.InitFromProfile();

        FortressHealth fortress = FindObjectOfType<FortressHealth>();
        if (fortress != null)
            fortress.InitFromProfile();

        if (stageTitleText != null)
            stageTitleText.text = $"Stage {currentStage}";
    }


    void Update()
    {
        if (stageEnded) return;
        elapsedTime += Time.deltaTime * gameSpeed;
        if (ElapsedTime >= 60f)
        {
            ZoomOutCamera();
        }
        if (ElapsedTime >= levelDuration)
        {
            OnStageTimeFinished();
        }
    }

    void ZoomOutCamera()
    {
        CameraController camCtrl = FindObjectOfType<CameraController>();
        if (camCtrl != null)
        {
            camCtrl.SmoothZoomKeepingBottom(9.5f);
        }
    }

    // Elapsed game time (affected by timeScale)
    public float ElapsedTime
    {
        get { return elapsedTime; }
    }

    public bool IsLevelOver()
    {
        return ElapsedTime >= levelDuration;
    }

    public int GetDifficultyStep()
    {
        return Mathf.FloorToInt(ElapsedTime / stepInterval);
    }

    public float GetDifficultyMultiplier()
    {
        int step = GetDifficultyStep();
        return Mathf.Pow(stepMultiplier, step);
    }

    // Simple speed control
    public void SetGameSpeed(float speedMultiplier)
    {
        gameSpeed = speedMultiplier;
        Time.timeScale = speedMultiplier;
    }

    public float GetClearProgress01()
    {
        return Mathf.Clamp01(ElapsedTime / levelDuration);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnStageTimeFinished()
    {
        stageTimeFinished = true;
        EnemySpawner.Instance.StopSpawning();
        CheckForStageClear();
    }

    public void CheckForStageClear()
    {
        if (!stageTimeFinished)
            return;

        if (EnemySpawner.Instance.activeEnemyCount <= 0)
            EndStageWin();

        Debug.Log("All enemies cleared. Stage complete.");
    }

    public void ForceEndStageLose()
    {
        // if (stageEnded) return;

        stageEnded = true;
        // EndStageLose();
    }

    public int CalculateCoins(bool won)
    {
        int baseReward = won ? 50 : 20;
        int timeBonus = Mathf.FloorToInt(elapsedTime * 0.1f);

        int stage = currentStage;
        float stageMultiplier = 1f + (stage - 1) * 0.5f;

        return Mathf.RoundToInt((baseReward + timeBonus) * stageMultiplier);
    }

    void CheckUnlocks()
    {
        var profile = PlayerProfile.Instance;

        if (profile.highestStageCleared >= 1)
        {
            profile.laserTurretUnlocked = true;
            profile.homingMissileTurretUnlocked = true;
        }

    }

    public void EndStageLose()
    {
        if (stageEnded) return;
        stageEnded = true;

        Debug.Log("STAGE LOST!");
        Time.timeScale = 0f;
        int coinsEarned = CalculateCoins(false);
        PlayerProfile.Instance.coins += coinsEarned;
        PlayerProfile.Instance.SaveToDisk();

        gameOverPanel.SetActive(true);
        if (loseResultsUI != null)
            loseResultsUI.ShowFailResults();
    }

    public void EndStageWin()
    {
        if (stageEnded) return;
        stageEnded = true;

        int stage = currentStage;
        if (stage > PlayerProfile.Instance.highestStageCleared)
        {
            PlayerProfile.Instance.highestStageCleared = stage;
        }

        Debug.Log("STAGE CLEARED!");
        Time.timeScale = 0f;
        CheckUnlocks();
        int coinsEarned = CalculateCoins(true);
        PlayerProfile.Instance.coins += coinsEarned;
        PlayerProfile.Instance.SaveToDisk();

        stageClearPanel.SetActive(true);
        if (winResultsUI != null)
            winResultsUI.ShowClearResults();
    }
}
