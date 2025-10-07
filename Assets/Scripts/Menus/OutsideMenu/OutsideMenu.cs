using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutsideMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject OutsidetutorialUI;

    [Header("Interactive Buttons")]
    public Button PaloSeboButton;
    public Button SipaButton;
    public Button JolensButton;
    public Button BackButton;


    [Header("Audio Clips")]
    public AudioClip defaultTheme;
    public AudioClip SipaTheme;
    public AudioClip GameMenuTheme;
    public AudioClip PaloSeboTheme;


    [Header("Scenes")]
    public string PaloSeboScene = "PaloSebo";
    public string SipaScene = "SipaGame";
    public string GameMenuScene = "GameMenu";

    private const string TutorialPlayedKeyOutside = "outsideMenuTutorial";

    void Start()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null && defaultTheme != null)
            AudioManager.Instance.PlayMusic(defaultTheme, loop: true);

        if (PlayerPrefs.GetInt(TutorialPlayedKeyOutside, 0) == 0)
        {
            if (OutsidetutorialUI != null) OutsidetutorialUI.SetActive(true);
            SetInteractiveButtons(false);
        }
        else
        {
            if (OutsidetutorialUI != null) OutsidetutorialUI.SetActive(false);
            SetInteractiveButtons(true);
        }
    }

    public void TutorialFinished()
    {
        PlayerPrefs.SetInt(TutorialPlayedKeyOutside, 1);
        PlayerPrefs.Save();

        if (OutsidetutorialUI != null) OutsidetutorialUI.SetActive(false);
        SetInteractiveButtons(true);
    }

    private void SetInteractiveButtons(bool enable)
    {
        if (PaloSeboButton != null) PaloSeboButton.interactable = enable;
        if (SipaButton != null) SipaButton.interactable = enable;
        if (JolensButton != null) JolensButton.interactable = enable;
        if (BackButton != null)
        {
            BackButton.interactable = enable;

            TMP_Text buttonText = BackButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
                buttonText.enabled = enable;
        }
    }

    // ===== Navigation =====
    public void GoToGameMenu()
    {
        ChangeTheme(GameMenuTheme);
        SceneManager.LoadScene(GameMenuScene);
    }

    public void GoToPaloSebo()
    {
        ChangeTheme(PaloSeboTheme);
        SceneManager.LoadScene(PaloSeboScene);
    }

    public void GoToSipa()
    {
        ChangeTheme(SipaTheme);
        SceneManager.LoadScene(SipaScene);
    }

    // ===== Audio =====
    private void ChangeTheme(AudioClip newTheme)
    {
        if (AudioManager.Instance != null && newTheme != null)
            AudioManager.Instance.PlayMusic(newTheme, loop: true);
    }
}
