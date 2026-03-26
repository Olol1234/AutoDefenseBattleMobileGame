using UnityEngine;
using UnityEngine.UI;

public class AudioSync : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        // Sync the Slider's visual position to the saved data
        if (PlayerProfile.Instance != null)
        {
            slider.value = PlayerProfile.Instance.masterVolume;
        }

        // Ensure the AudioManager is actually playing at this volume
        AudioManager.Instance.SetVolume(slider.value);

        slider.onValueChanged.AddListener(HandleSliderChange);
    }

    void HandleSliderChange(float value)
    {
        AudioManager.Instance.SetVolume(value);
        PlayerProfile.Instance.masterVolume = value;
    }
}