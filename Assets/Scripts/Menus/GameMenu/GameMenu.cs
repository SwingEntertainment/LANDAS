using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;
using UnityEngine.Networking;

public class GameMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject tutorialUI;
    public GameObject lolaMenuPanel;
    public GameObject lockedPanel;

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
    public string quizSpanishScene = "QuizSpanish";
    public string storySpanishScene = "StorySpanish";

    private const string LolaMenuActiveKey = "LolaMenuActive";
    private const string TutorialPlayedKey = "gameMenuTutorial";
    private const string SpanishJsonFile = "spanishChapters.json";

    void Start()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null && defaultTheme != null)
            AudioManager.Instance.PlayMusic(defaultTheme, loop: true);

        bool tutorialPlayed = PlayerPrefs.GetInt(TutorialPlayedKey, 0) == 1;

        if (!tutorialPlayed)
        {
            if (tutorialUI != null)
            {
                tutorialUI.SetActive(true);
                CanvasGroup cg = tutorialUI.GetComponent<CanvasGroup>() ?? tutorialUI.AddComponent<CanvasGroup>();
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
            SetInteractiveButtons(false);
        }
        else
        {
            if (tutorialUI != null)
                tutorialUI.SetActive(false);
            SetInteractiveButtons(true);
        }

        bool lolaActive = PlayerPrefs.GetInt(LolaMenuActiveKey, 0) == 1;
        if (lolaMenuPanel != null)
        {
            lolaMenuPanel.SetActive(lolaActive);
            SetInteractiveButtons(!lolaActive);
        }

        if (lockedPanel != null)
            lockedPanel.SetActive(false);
    }

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

    public void TutorialFinished()
    {
        PlayerPrefs.SetInt(TutorialPlayedKey, 1);
        PlayerPrefs.Save();

        if (tutorialUI != null)
            tutorialUI.SetActive(false);

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
        LoadingScene.LoadSceneWithLoading(kitchenScene);
    }

    public void GoToDictionary()
    {
        ChangeTheme(dictionaryTheme);
        LoadingScene.LoadSceneWithLoading(dictionaryScene);
    }

    public void GoToOutside()
    {
        ChangeTheme(outsideTheme);
        LoadingScene.LoadSceneWithLoading(outsideScene);
    }

    public void GoToSpanishQuizIfUnlocked()
    {
        StartCoroutine(CheckSpanishChaptersAndProceed());
    }

    private IEnumerator CheckSpanishChaptersAndProceed()
    {
        string persistentPath = Path.Combine(Application.persistentDataPath, SpanishJsonFile);
        string streamingPath = Path.Combine(Application.streamingAssetsPath, SpanishJsonFile);
        string json = "";

        if (!File.Exists(persistentPath))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            UnityWebRequest request = UnityWebRequest.Get(streamingPath);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllText(persistentPath, request.downloadHandler.text);
                Debug.Log("Copied spanishChapters.json to persistentDataPath.");
            }
            else
            {
                Debug.LogWarning("Failed to copy spanishChapters.json on Android: " + request.error);
                yield break;
            }
#else
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("Copied spanishChapters.json to persistentDataPath.");
            }
            else
            {
                Debug.LogWarning("spanishChapters.json not found at " + streamingPath);
                yield break;
            }
#endif
        }

        try
        {
            json = File.ReadAllText(persistentPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to read spanishChapters.json: " + e.Message);
            yield break;
        }

        SpanishChaptersData_GameMenu data = JsonUtility.FromJson<SpanishChaptersData_GameMenu>(json);

        if (data == null || data.subchapters == null || data.subchapters.Length == 0)
        {
            Debug.LogWarning("Invalid or empty spanishChapters.json structure.");
            yield break;
        }

        bool anyRead = false;
        foreach (var sub in data.subchapters)
        {
            if (sub.isRead)
            {
                anyRead = true;
                break;
            }
        }

        if (anyRead)
        {
            PlayerPrefs.SetInt(LolaMenuActiveKey, 1);
            PlayerPrefs.Save();

            ChangeTheme(quizTheme);
            LoadingScene.LoadSceneWithLoading(quizSpanishScene);
        }
        else
        {
            if (lockedPanel != null)
                lockedPanel.SetActive(true);
        }
    }

    public void GoToSpanishStory()
    {
        PlayerPrefs.SetInt(LolaMenuActiveKey, 1);
        PlayerPrefs.Save();

        ChangeTheme(quizTheme);
        LoadingScene.LoadSceneWithLoading(storySpanishScene);
    }

    // ===== Audio =====
    private void ChangeTheme(AudioClip newTheme)
    {
        if (AudioManager.Instance != null && newTheme != null)
            AudioManager.Instance.PlayMusic(newTheme, loop: true);
    }

    // ===== Lola Menu =====
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

[Serializable]
public class SpanishChaptersData_GameMenu
{
    public Subchapter_GameMenu[] subchapters;
}

[Serializable]
public class Subchapter_GameMenu
{
    public string subchapterID;
    public string title;
    public string thumbnail;
    public string imageTitle;
    public bool isRead;
}
