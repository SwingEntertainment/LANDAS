using UnityEngine;
using UnityEngine.SceneManagement;

public class SpanishMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject tutorialUI;

    [Header("Scenes")]
    public string gameMenuScene = "GameMenu";

    [Header("Audio Clips")]
    public AudioClip defaultTheme;
    private const string LolaMenuActiveKey = "LolaMenuActive";
    private const string StoryTutorialPlayedKey = "StoryMenuTutorial";

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(LolaMenuActiveKey, 0);
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            PlayerPrefs.SetInt(LolaMenuActiveKey, 0);
            PlayerPrefs.Save();
        }
    }

    void Start()
    {
        if (AudioManager.Instance != null && defaultTheme != null)
            AudioManager.Instance.PlayMusic(defaultTheme, loop: true);

        bool tutorialPlayed = PlayerPrefs.GetInt(StoryTutorialPlayedKey, 0) == 1;

        if (!tutorialPlayed)
        {
            if (tutorialUI != null)
            {
                tutorialUI.SetActive(true);
                CanvasGroup cg = tutorialUI.GetComponent<CanvasGroup>() ?? tutorialUI.AddComponent<CanvasGroup>();
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
        }
        else
        {
            if (tutorialUI != null)
                tutorialUI.SetActive(false);
        }
    }

    public void TutorialStoryFinished()
    {
        PlayerPrefs.SetInt(StoryTutorialPlayedKey, 1);
        PlayerPrefs.Save();

        if (tutorialUI != null)
            tutorialUI.SetActive(false);
    }

    public void GoToGameMenu()
    {
        LoadingScene.LoadSceneWithLoading(gameMenuScene);
    }
}
