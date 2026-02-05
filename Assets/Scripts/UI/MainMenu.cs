using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class StageData
{
    public int stageNumber;
    public Sprite stageSprite;
    public string description;
}

public class MainMenu : MonoBehaviour
{
    public int selectedStage = 1;
    public TMP_Text stageText;
    public UnityEngine.UI.Image stageImage;
    // later if needed
    // public TMP_Text stageDescriptionText;

    public UnityEngine.UI.Button prevButton;
    public UnityEngine.UI.Button nextButton;

    public int maxUnlockedStage = 1;   // Later load from save
    public StageData[] stages;


    void Start()
    {
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
            nextButton.interactable = selectedStage < maxUnlockedStage;
        }

    public void PlayStage()
    {
        // Save selected stage
        PlayerPrefs.SetInt("SelectedStage", selectedStage);

        // Load gameplay scene
        SceneManager.LoadScene("MainScene");
    }

    public void NextStage()
    {
        if (selectedStage < maxUnlockedStage)
        {
            selectedStage++;
            UpdateUI();
        }
    }

    public void PrevStage()
    {
        if (selectedStage > 1)
        {
            selectedStage--;
            UpdateUI();
        }
    }
}
