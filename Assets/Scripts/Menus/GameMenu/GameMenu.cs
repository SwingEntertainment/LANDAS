using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject tutorialUI;

    [Header("Interactive Buttons")]
    public Button kitchenButton;
    public Button dictionaryButton;
    public Button outsideButton;
    public Button lolaButton;

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


    private const string TutorialPlayedKey = "gameMenuTutorial";

    void Start()
    {
        Time.timeScale = 1f;

        // Play default theme
        if (AudioManager.Instance != null && defaultTheme != null)
            AudioManager.Instance.PlayMusic(defaultTheme, loop: true);

        // Show tutorial only if not read before
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
    }

    // Called by TutorialUI when last page is reached
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
        ChangeTheme(quizTheme);
        SceneManager.LoadScene(quizSpanishScene);
    }

    // ===== Audio =====
    private void ChangeTheme(AudioClip newTheme)
    {
        if (AudioManager.Instance != null && newTheme != null)
            AudioManager.Instance.PlayMusic(newTheme, loop: true);
    }
}
