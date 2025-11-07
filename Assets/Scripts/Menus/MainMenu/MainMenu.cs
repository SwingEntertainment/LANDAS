using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip mainMenuBGM;
    public AudioClip buttonClick;

    [Header("UI")]
    public GameObject replayModal;
    public GameObject mainMenuPanel;

    [Header("Scenes")]
    public string storyScene = "StorySequence";
    public string gameMenuScene = "GameMenu";

    private const string IsStoryPlayedKey = "isStoryPlayed";

    void Start()
    {
        if (AudioManager.Instance != null && mainMenuBGM != null)
        {
            AudioManager.Instance.PlayMusic(mainMenuBGM, loop: true);
        }

        if (replayModal != null) replayModal.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(buttonClick);
        Application.Quit();
    }

    public void CheckForUpdates()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(buttonClick);
        Application.OpenURL("https://landas2D.github.io");
    }

    public void OnStartButton()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(buttonClick);

        if (PlayerPrefs.GetInt(IsStoryPlayedKey, 0) == 0)
        {
            LoadingScene.LoadSceneWithLoading(storyScene);
        }
        else
        {
            ShowReplayModal(true);
        }
    }

    public void ReplayStory()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(buttonClick);
        ShowReplayModal(false);
        LoadingScene.LoadSceneWithLoading(storyScene);
    }

    public void SkipStory()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(buttonClick);
        ShowReplayModal(false);
        LoadingScene.LoadSceneWithLoading(gameMenuScene);
    }

    private void ShowReplayModal(bool show)
    {
        if (replayModal != null) replayModal.SetActive(show);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(!show);
    }

    public static void SetStoryPlayed()
    {
        PlayerPrefs.SetInt(IsStoryPlayedKey, 1);
        PlayerPrefs.Save();
    }
}
