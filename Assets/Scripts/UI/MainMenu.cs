using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

[System.Serializable]
public class StageData
{
    public int stageNumber;
    public Sprite stageSprite;
    public string description;
}

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public int selectedStage = 1;
    public TMP_Text stageText;
    public UnityEngine.UI.Image stageImage;
    public GameObject errorMessagePanel;

    public UnityEngine.UI.Button prevButton;
    public UnityEngine.UI.Button nextButton;

    public int maxUnlockedStage;   // Later load from save
    public StageData[] stages;


    void Start()
    {
        AudioManager.Instance.PlayMenuMusic();
        selectedStage = PlayerPrefs.GetInt("SelectedStage", 1);
        maxUnlockedStage = PlayerPrefs.GetInt("HighestStageUnlocked", 1);
        UpdateUI();
    }

    void UpdateUI()
    {
        selectedStage = Mathf.Clamp(selectedStage, 1, stages.Length);

        if (stageText != null)
            stageText.text = $"Stage {selectedStage}";

        // Stage data
        StageData data = stages[selectedStage - 1];

        if (stageImage != null)
            stageImage.sprite = data.stageSprite;

        // if (stageDescriptionText != null)
        //     stageDescriptionText.text = data.description;

        // Prev button
        if (prevButton != null)
            prevButton.interactable = selectedStage > 1;

        // Next button (locked logic)
        if (nextButton != null)
            nextButton.interactable = selectedStage < 10;
        }

    public void PlayStage()
    {
        AudioManager.Instance.PlayClick();
        // Save selected stage
        PlayerPrefs.SetInt("SelectedStage", selectedStage);
        if (selectedStage > maxUnlockedStage+1)
        {
            // Debug.LogWarning("Selected stage is locked! This should not happen.");
            StartCoroutine(ShowClearPreviousStageErrorPanelRoutine());
            return;
        }

        // Load gameplay scene
        SceneManager.LoadScene("MainScene");
    }

    public void NextStage()
    {
        if (selectedStage < 11)
        {
            selectedStage++;
            UpdateUI();
        }
        AudioManager.Instance.PlayClick();
    }

    public void PrevStage()
    {
        if (selectedStage > 1)
        {
            selectedStage--;
            UpdateUI();
        }
        AudioManager.Instance.PlayClick();
    }

    private IEnumerator ShowClearPreviousStageErrorPanelRoutine()
    {
        errorMessagePanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        errorMessagePanel.SetActive(false);
    }
}
