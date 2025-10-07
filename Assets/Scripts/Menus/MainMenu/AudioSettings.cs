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

    [Header("Button Images")]
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    [Header("Defaults")]
    [Range(0.01f, 1f)] public float defaultVolume = 0.5f;

    private float lastMusicVolume = 0.5f;
    private float lastSFXVolume = 0.5f;

    void Start()
    {
        if (AudioManager.Instance == null) return;

        float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        float savedSFXVol = PlayerPrefs.GetFloat("SFXVolume", defaultVolume);
        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        bool sfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;

        AudioManager.Instance.musicVolume = savedMusicVol;
        AudioManager.Instance.sfxVolume = savedSFXVol;
        AudioManager.Instance.musicOn = musicOn;
        AudioManager.Instance.sfxOn = sfxOn;

        lastMusicVolume = Mathf.Max(savedMusicVol, defaultVolume);
        lastSFXVolume = Mathf.Max(savedSFXVol, defaultVolume);

        // Setup sliders
        if (musicSlider != null)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.wholeNumbers = false;
            musicSlider.value = musicOn ? savedMusicVol : 0f;
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.wholeNumbers = false;
            sfxSlider.value = sfxOn ? savedSFXVol : 0f;
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }

        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSFX);

        UpdateMusicUI();
        UpdateSFXUI();
    }

    void ToggleMusic()
    {
        if (AudioManager.Instance == null) return;

        if (!AudioManager.Instance.musicOn)
        {
            AudioManager.Instance.musicOn = true;
            float restoreVolume = (lastMusicVolume > 0f) ? lastMusicVolume : defaultVolume;
            AudioManager.Instance.SetMusicVolume(restoreVolume);
            if (musicSlider != null) musicSlider.value = restoreVolume;
        }
        else
        {
            lastMusicVolume = AudioManager.Instance.musicVolume;
            AudioManager.Instance.SetMusicVolume(0f);
            AudioManager.Instance.musicOn = false;
            if (musicSlider != null) musicSlider.value = 0f;
        }

        SaveAudioSettings();
        UpdateMusicUI();
    }

    void ToggleSFX()
    {
        if (AudioManager.Instance == null) return;

        if (!AudioManager.Instance.sfxOn)
        {
            AudioManager.Instance.sfxOn = true;
            float restoreVolume = (lastSFXVolume > 0f) ? lastSFXVolume : defaultVolume;
            AudioManager.Instance.SetSFXVolume(restoreVolume);
            if (sfxSlider != null) sfxSlider.value = restoreVolume;
        }
        else
        {
            lastSFXVolume = AudioManager.Instance.sfxVolume;
            AudioManager.Instance.SetSFXVolume(0f);
            AudioManager.Instance.sfxOn = false;
            if (sfxSlider != null) sfxSlider.value = 0f;
        }

        SaveAudioSettings();
        UpdateSFXUI();
    }

    void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance == null) return;

        AudioManager.Instance.SetMusicVolume(value);
        AudioManager.Instance.musicOn = value > 0f;
        lastMusicVolume = Mathf.Max(value, defaultVolume);

        SaveAudioSettings();
        UpdateMusicUI();
    }

    void OnSFXSliderChanged(float value)
    {
        if (AudioManager.Instance == null) return;

        AudioManager.Instance.SetSFXVolume(value);
        AudioManager.Instance.sfxOn = value > 0f;
        lastSFXVolume = Mathf.Max(value, defaultVolume);

        SaveAudioSettings();
        UpdateSFXUI();
    }

    void UpdateMusicUI()
    {
        if (AudioManager.Instance == null) return;

        bool on = AudioManager.Instance.musicOn;
        musicLabel.text = on ? "Music: ON" : "Music: OFF";

        Image btnImage = musicButton.GetComponent<Image>();
        if (btnImage != null)
            btnImage.sprite = on ? musicOnSprite : musicOffSprite;
    }

    void UpdateSFXUI()
    {
        if (AudioManager.Instance == null) return;

        bool on = AudioManager.Instance.sfxOn;
        sfxLabel.text = on ? "Sound: ON" : "Sound: OFF";

        Image btnImage = sfxButton.GetComponent<Image>();
        if (btnImage != null)
            btnImage.sprite = on ? sfxOnSprite : sfxOffSprite;
    }

    void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", AudioManager.Instance.musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", AudioManager.Instance.sfxVolume);
        PlayerPrefs.SetInt("MusicOn", AudioManager.Instance.musicOn ? 1 : 0);
        PlayerPrefs.SetInt("SFXOn", AudioManager.Instance.sfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
