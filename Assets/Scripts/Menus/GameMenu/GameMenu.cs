using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject tutorialUI;
    public GameObject lolaMenuPanel;

    [Header("Interactive Buttons")]
    public Button kitchenButton;
    public Button dictionaryButton;
    public Button outsideButton;
    public Button lolaButton;
    public Button menuButton;


    [Header("Audio Clips")]
    public AudioClip defaultTheme;
    public AudioClip kitchenTheme;
    public AudioClip dictionaryTheme;
    public AudioClip outsideTheme;
    public AudioClip quizTheme;


    [Header("Scenes")]
    public string kitchenScene = "KitchenMenu";
    public string dictionaryScene = "Dictionary";
    public string outsideScene = "OutsideMenu";
    public string quizSpanishScene = "QuizSPanish";

    private const string LolaMenuActiveKey = "LolaMenuActive";

    private const string TutorialPlayedKey = "gameMenuTutorial";

    void Start()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null && defaultTheme != null)
            AudioManager.Instance.PlayMusic(defaultTheme, loop: true);

        if (PlayerPrefs.GetInt(TutorialPlayedKey, 0) == 0)
        {
            if (tutorialUI != null) tutorialUI.SetActive(true);
            SetInteractiveButtons(false);
        }
        else
        {
            if (tutorialUI != null) tutorialUI.SetActive(false);
            SetInteractiveButtons(true);
        }
        if (lolaMenuPanel != null)
        {
            bool wasActive = PlayerPrefs.GetInt(LolaMenuActiveKey, 0) == 1;
            lolaMenuPanel.SetActive(wasActive);
            SetInteractiveButtons(!wasActive);
        }
    }

    public void TutorialFinished()
    {
        PlayerPrefs.SetInt(TutorialPlayedKey, 1);
        PlayerPrefs.Save();

        if (tutorialUI != null) tutorialUI.SetActive(false);
        SetInteractiveButtons(true);
    }

    private void SetInteractiveButtons(bool enable)
    {
        if (kitchenButton != null) kitchenButton.interactable = enable;
        if (dictionaryButton != null) dictionaryButton.interactable = enable;
        if (outsideButton != null) outsideButton.interactable = enable;
        if (lolaButton != null) lolaButton.interactable = enable;
        if (menuButton != null)
        {
            menuButton.interactable = enable;

            TMP_Text buttonText = menuButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
                buttonText.enabled = enable;
        }
    }

    // ===== Navigation =====
    public void GoToKitchen()
    {
        ChangeTheme(kitchenTheme);
        SceneManager.LoadScene(kitchenScene);
    }

    public void GoToDictionary()
    {
        ChangeTheme(dictionaryTheme);
        SceneManager.LoadScene(dictionaryScene);
    }

    public void GoToOutside()
    {
        ChangeTheme(outsideTheme);
        SceneManager.LoadScene(outsideScene);
    }

    public void GoToSpanishQiuz()
    {
        if (lolaMenuPanel != null)
        {
            PlayerPrefs.SetInt(LolaMenuActiveKey, lolaMenuPanel.activeSelf ? 1 : 0);
            PlayerPrefs.Save();
        }

        ChangeTheme(quizTheme);
        SceneManager.LoadScene(quizSpanishScene);
    }

    // ===== Audio =====
    private void ChangeTheme(AudioClip newTheme)
    {
        if (AudioManager.Instance != null && newTheme != null)
            AudioManager.Instance.PlayMusic(newTheme, loop: true);
    }

    public void ToggleLolaMenu(bool show)
{
    if (lolaMenuPanel != null)
    {
        lolaMenuPanel.SetActive(show);
        SetInteractiveButtons(!show); 
    }

    PlayerPrefs.SetInt(LolaMenuActiveKey, show ? 1 : 0);
    PlayerPrefs.Save();
}

}
