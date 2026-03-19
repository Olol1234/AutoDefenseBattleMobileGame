using UnityEngine;
using UnityEngine.UI;

public class MainSettingsPanel : MonoBehaviour
{
    // [SerializeField] private Slider volumeSlider;

    // void OnEnable()
    // {
    //     // Load existing volume or default to 1
    //     volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
    // }

    // public void OnVolumeChanged()
    // {
    //     float value = volumeSlider.value;
    //     AudioManager.Instance.SetVolume(value); 
    //     // PlayerPrefs.SetFloat("MusicVolume", value);
    // }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f; // Unpause the game
        // PauseManager.IsPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit(); // Note: This only works in a built .exe or .apk, not in Unity Editor
    }
}