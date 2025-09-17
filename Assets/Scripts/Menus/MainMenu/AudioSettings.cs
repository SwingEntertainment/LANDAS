using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettings : MonoBehaviour
{
    [Header("Button + Label References")]
    public Button musicButton;
    public TMP_Text musicLabel;

    public Button sfxButton;
    public TMP_Text sfxLabel;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Defaults")]
    [Range(0.01f, 1f)] public float defaultVolume = 0.1f; 

    private float lastMusicVolume = 0.5f; 
    private float lastSFXVolume = 0.5f;

    void Start()
    {
        if (AudioManager.Instance == null) return;

        lastMusicVolume = Mathf.Max(AudioManager.Instance.musicVolume, defaultVolume);
        lastSFXVolume = Mathf.Max(AudioManager.Instance.sfxVolume, defaultVolume);

        UpdateMusicLabel();
        UpdateSFXLabel();

        if (musicSlider != null)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.wholeNumbers = false;
            musicSlider.value = AudioManager.Instance.musicVolume;
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.wholeNumbers = false;
            sfxSlider.value = AudioManager.Instance.sfxVolume;
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }

        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSFX);
    }

    void ToggleMusic()
    {
        if (AudioManager.Instance == null) return;

        if (!AudioManager.Instance.musicOn || AudioManager.Instance.musicVolume <= 0f)
        {
            AudioManager.Instance.musicOn = true;

            float restoreVolume = (lastMusicVolume > 0f) ? lastMusicVolume : defaultVolume;
            AudioManager.Instance.SetMusicVolume(restoreVolume);

            if (musicSlider != null)
                musicSlider.value = restoreVolume;
        }
        else
        {
            if (AudioManager.Instance.musicVolume > 0f)
                lastMusicVolume = AudioManager.Instance.musicVolume;

            AudioManager.Instance.ToggleMusic();
        }

        UpdateMusicLabel();
    }

    void ToggleSFX()
    {
        if (AudioManager.Instance == null) return;

        if (!AudioManager.Instance.sfxOn || AudioManager.Instance.sfxVolume <= 0f)
        {
            AudioManager.Instance.sfxOn = true;

            float restoreVolume = (lastSFXVolume > 0f) ? lastSFXVolume : defaultVolume;
            AudioManager.Instance.SetSFXVolume(restoreVolume);

            if (sfxSlider != null)
                sfxSlider.value = restoreVolume;
        }
        else
        {
            if (AudioManager.Instance.sfxVolume > 0f)
                lastSFXVolume = AudioManager.Instance.sfxVolume;

            AudioManager.Instance.ToggleSFX();
        }

        UpdateSFXLabel();
    }

    void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.SetMusicVolume(value);

        if (value > 0f)
            lastMusicVolume = value;

        UpdateMusicLabel();
    }

    void OnSFXSliderChanged(float value)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.SetSFXVolume(value);

        if (value > 0f)
            lastSFXVolume = value;

        UpdateSFXLabel();
    }

    void UpdateMusicLabel()
    {
        if (AudioManager.Instance == null) return;

        if (AudioManager.Instance.musicVolume <= 0f)
            AudioManager.Instance.musicOn = false;

        musicLabel.text = AudioManager.Instance.musicOn ? "Music: ON" : "Music: OFF";

        if (musicSlider != null)
            musicSlider.interactable = (AudioManager.Instance.musicOn && AudioManager.Instance.musicVolume > 0f);
    }

    void UpdateSFXLabel()
    {
        if (AudioManager.Instance == null) return;

        if (AudioManager.Instance.sfxVolume <= 0f)
            AudioManager.Instance.sfxOn = false;

        sfxLabel.text = AudioManager.Instance.sfxOn ? "Sound: ON" : "Sound: OFF";

        if (sfxSlider != null)
            sfxSlider.interactable = (AudioManager.Instance.sfxOn && AudioManager.Instance.sfxVolume > 0f);
    }
}
