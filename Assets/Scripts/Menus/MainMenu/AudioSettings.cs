using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonsAndSlidersSettings : MonoBehaviour
{
    [Header("Button + Label References")]
    public Button musicButton;
    public TMP_Text musicLabel;

    public Button sfxButton;
    public TMP_Text sfxLabel;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (AudioManager.Instance == null) return;

        UpdateMusicLabel();
        UpdateSFXLabel();

        if (musicSlider != null)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.wholeNumbers = false;
            musicSlider.value = AudioManager.Instance.musicVolume;
            musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.wholeNumbers = false;
            sfxSlider.value = AudioManager.Instance.sfxVolume;
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        }

        // Hook up button clicks
        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSFX);
    }

    void ToggleMusic()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.ToggleMusic();
        UpdateMusicLabel();
    }

    void ToggleSFX()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.ToggleSFX();
        UpdateSFXLabel();
    }

    void UpdateMusicLabel()
    {
        if (AudioManager.Instance == null) return;
        musicLabel.text = AudioManager.Instance.musicOn ? "Music: ON" : "Music: OFF";

        if (musicSlider != null)
            musicSlider.interactable = AudioManager.Instance.musicOn;
    }

    void UpdateSFXLabel()
    {
        if (AudioManager.Instance == null) return;
        sfxLabel.text = AudioManager.Instance.sfxOn ? "Sound: ON" : "Sound: OFF";

        if (sfxSlider != null)
            sfxSlider.interactable = AudioManager.Instance.sfxOn;
    }
}
