using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources (assign in inspector)")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Optional default music")]
    public AudioClip defaultMusic;

    [Header("Settings (defaults)")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public bool musicOn = true;
    public bool sfxOn = true;

    private const string PREF_MUSIC_ON = "MusicOn";
    private const string PREF_SFX_ON = "SFXOn";
    private const string PREF_MUSIC_VOL = "MusicVolume";
    private const string PREF_SFX_VOL = "SFXVolume";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPrefs();
        ApplySettings();

        if (defaultMusic != null && musicOn)
        {
            musicSource.clip = defaultMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    void LoadPrefs()
    {
        bool hasPrefs = PlayerPrefs.HasKey(PREF_MUSIC_ON) ||
                        PlayerPrefs.HasKey(PREF_SFX_ON) ||
                        PlayerPrefs.HasKey(PREF_MUSIC_VOL) ||
                        PlayerPrefs.HasKey(PREF_SFX_VOL);

        if (!hasPrefs)
        {
            musicOn = true;
            sfxOn = true;
            musicVolume = 1f;
            sfxVolume = 1f;

            PlayerPrefs.SetInt(PREF_MUSIC_ON, 1);
            PlayerPrefs.SetInt(PREF_SFX_ON, 1);
            PlayerPrefs.SetFloat(PREF_MUSIC_VOL, 1f);
            PlayerPrefs.SetFloat(PREF_SFX_VOL, 1f);
            PlayerPrefs.Save();
        }
        else
        {
            musicOn = PlayerPrefs.GetInt(PREF_MUSIC_ON, 1) == 1;
            sfxOn = PlayerPrefs.GetInt(PREF_SFX_ON, 1) == 1;
            musicVolume = PlayerPrefs.GetFloat(PREF_MUSIC_VOL, 1f);
            sfxVolume = PlayerPrefs.GetFloat(PREF_SFX_VOL, 1f);
        }
    }

    void ApplySettings()
    {
        if (musicSource != null)
            musicSource.volume = musicOn ? musicVolume : 0f;
        if (sfxSource != null)
            sfxSource.volume = sfxOn ? sfxVolume : 0f;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        if (musicOn)
            musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        if (!sfxOn) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetMusicOn(bool on)
    {
        musicOn = on;
        PlayerPrefs.SetInt(PREF_MUSIC_ON, on ? 1 : 0);
        ApplySettings();

        if (musicOn && musicSource.clip != null && !musicSource.isPlaying)
            musicSource.Play();

        PlayerPrefs.Save();
    }

    public void SetSFXOn(bool on)
    {
        sfxOn = on;
        PlayerPrefs.SetInt(PREF_SFX_ON, on ? 1 : 0);
        ApplySettings();
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float vol)
    {
        musicVolume = Mathf.Clamp01(vol);
        PlayerPrefs.SetFloat(PREF_MUSIC_VOL, musicVolume);
        ApplySettings();
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float vol)
    {
        sfxVolume = Mathf.Clamp01(vol);
        PlayerPrefs.SetFloat(PREF_SFX_VOL, sfxVolume);
        ApplySettings();
        PlayerPrefs.Save();
    }

    public void ToggleMusic() => SetMusicOn(!musicOn);
    public void ToggleSFX() => SetSFXOn(!sfxOn);
}
