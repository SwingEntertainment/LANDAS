using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject tutorialUI;
    public GameObject lolaMenuPanel;
    public GameObject lockedPanel;

    [Header("Hidden Dictionary Panel")]
    public GameObject hiddenDictionaryPanel;
    public TMP_Text hiddenDictionaryMessageText;
    public Button readFirstDictionaryButton;
    public Button closeHiddenDictionaryPanelButton;

    [Header("Hidden Kitchen Panel")]
    public GameObject hiddenKitchenPanel;
    public TMP_Text hiddenKitchenMessageText;
    public Button readSecondButton;
    public Button closeHiddenKitchenPanelButton;

    [Header("Hidden Outside Panel")]
    public GameObject hiddenOutsidePanel;
    public TMP_Text hiddenOutsideMessageText;
    public Button goToQuizSpanishButton;
    public Button closeHiddenOutsidePanelButton;

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
    private const string RecipeJsonFile = "RecipeList.json";


    void Start()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null && defaultTheme != null)
            if (!AudioManager.Instance.IsMusicPlaying(defaultTheme))
            {
                AudioManager.Instance.PlayMusic(defaultTheme, loop: true);
            }

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
        StartCoroutine(CheckKitchenAccess());
    }

    private IEnumerator CheckKitchenAccess()
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
        }
        else
        {
            yield break;
        }
#else
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
            }
            else
            {
                yield break;
            }
#endif
        }

        try
        {
            json = File.ReadAllText(persistentPath);
        }
        catch (Exception)
        {
            yield break;
        }

        SpanishChaptersData_GameMenu data = JsonUtility.FromJson<SpanishChaptersData_GameMenu>(json);

        if (data == null || data.subchapters == null || data.subchapters.Length == 0)
            yield break;

        Subchapter_GameMenu secondChapter = Array.Find(data.subchapters, s => s.subchapterID == "ch1-02");

        if (secondChapter != null && !secondChapter.isRead)
        {
            if (hiddenKitchenPanel != null)
            {
                hiddenKitchenPanel.SetActive(true);

                if (hiddenKitchenMessageText != null)
                {
                    hiddenKitchenMessageText.text =
                        "Saliksikin muna ang kwento ng panahon ng Espanyol sa subchapter 2";
                }

                if (readSecondButton != null)
                {
                    readSecondButton.onClick.RemoveAllListeners();
                    readSecondButton.onClick.AddListener(GoToSpanishStory);
                }

                if (closeHiddenKitchenPanelButton != null)
                {
                    closeHiddenKitchenPanelButton.onClick.RemoveAllListeners();
                    closeHiddenKitchenPanelButton.onClick.AddListener(() => hiddenKitchenPanel.SetActive(false));
                }
            }
        }
        else
        {
            ChangeTheme(kitchenTheme);
            LoadingScene.LoadSceneWithLoading(kitchenScene);
        }
    }


    public void GoToDictionary()
    {
        StartCoroutine(CheckDictionaryAccess());
    }

    private IEnumerator CheckDictionaryAccess()
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
        }
        else
        {
            yield break;
        }
#else
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
            }
            else
            {
                yield break;
            }
#endif
        }

        try
        {
            json = File.ReadAllText(persistentPath);
        }
        catch (Exception)
        {
            yield break;
        }

        SpanishChaptersData_GameMenu data = JsonUtility.FromJson<SpanishChaptersData_GameMenu>(json);

        if (data == null || data.subchapters == null || data.subchapters.Length == 0)
            yield break;

        Subchapter_GameMenu firstChapter = Array.Find(data.subchapters, s => s.subchapterID == "ch1-01");

        if (firstChapter != null && !firstChapter.isRead)
        {
            if (hiddenDictionaryPanel != null)
            {
                hiddenDictionaryPanel.SetActive(true);

                if (hiddenDictionaryMessageText != null)
                {
                    hiddenDictionaryMessageText.text =
                        "Saliksikin muna ang kwento ng panahon ng Espanyol sa subchapter 1";
                }

                if (readFirstDictionaryButton != null)
                    readFirstDictionaryButton.onClick.AddListener(GoToSpanishStory);

                if (closeHiddenDictionaryPanelButton != null)
                    closeHiddenDictionaryPanelButton.onClick.AddListener(() => hiddenDictionaryPanel.SetActive(false));
            }
        }
        else
        {
            ChangeTheme(dictionaryTheme);
            LoadingScene.LoadSceneWithLoading(dictionaryScene);
        }
    }


    public void GoToOutside()
    {
        StartCoroutine(CheckOutsideAccess());
    }

    private IEnumerator CheckOutsideAccess()
    {
        string persistentPath = Path.Combine(Application.persistentDataPath, RecipeJsonFile);
        string streamingPath = Path.Combine(Application.streamingAssetsPath, RecipeJsonFile);
        string json = "";

        if (!File.Exists(persistentPath))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest request = UnityWebRequest.Get(streamingPath);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllText(persistentPath, request.downloadHandler.text);
        }
        else
        {
            yield break;
        }
#else
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
            }
            else
            {
                yield break;
            }
#endif
        }

        try
        {
            json = File.ReadAllText(persistentPath);
        }
        catch (Exception)
        {
            yield break;
        }

        DishList data = JsonUtility.FromJson<DishList>(json);
        if (data == null || data.dishes == null || data.dishes.Count == 0)
            yield break;

        int cookedCount = 0;
        foreach (var dish in data.dishes)
        {
            if (dish.isCooked)
                cookedCount++;
        }

        if (cookedCount < 3)
        {
            if (hiddenOutsidePanel != null)
            {
                hiddenOutsidePanel.SetActive(true);

                if (hiddenOutsideMessageText != null)
                {
                    hiddenOutsideMessageText.text =
                        "Magsaliksik muna ng tatlong (3) putahe bago makapagsaya sa labas ng bahay.";
                }

                if (goToQuizSpanishButton != null)
                {
                    goToQuizSpanishButton.onClick.RemoveAllListeners();
                }

                if (closeHiddenOutsidePanelButton != null)
                {
                    closeHiddenOutsidePanelButton.onClick.RemoveAllListeners();
                    closeHiddenOutsidePanelButton.onClick.AddListener(() => hiddenOutsidePanel.SetActive(false));
                }
            }
        }
        else
        {
            ChangeTheme(outsideTheme);
            LoadingScene.LoadSceneWithLoading(outsideScene);
        }
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
            }
            else
            {
                yield break;
            }
#else
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath);
            }
            else
            {
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
            yield break;
        }

        SpanishChaptersData_GameMenu data = JsonUtility.FromJson<SpanishChaptersData_GameMenu>(json);

        if (data == null || data.subchapters == null || data.subchapters.Length == 0)
        {
            yield break;
        }

        bool allRead = true;
        foreach (var sub in data.subchapters)
        {
            if (!sub.isRead)
            {
                allRead = false;
                break;
            }
        }

        if (allRead)
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

[Serializable]
public class DishData
{
    public int foodID;
    public bool isCooked;
    public string dishName;
    public string dishImg;
    public List<string> recipeList;
    public List<int> ingredientIDs;
    public string dishDescription;
}

[Serializable]
public class DishList
{
    public List<DishData> dishes;
}
