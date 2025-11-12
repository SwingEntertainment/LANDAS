using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Dictionary : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject tutorialUI;
    public Button backButton;
    public Transform wordListParent;
    public GameObject wordButtonPrefab;
    public GameObject detailPanel;

    [Header("Pagination")]
    public Button nextPageButton;
    public Button prevPageButton;
    private int currentPage = 0;
    private int wordsPerPage = 6;

    [Header("Detail Panel UI")]
    public TMP_Text tagalogWordText;
    public TMP_Text englishWordText;
    public TMP_Text partOfSpeechText;
    public TMP_Text tagalogMeaningText;
    public TMP_Text englishMeaningText;
    public Button voiceButton;
    public Button detailNextButton;
    public Button detailPrevButton;

    [Header("Audio Clips")]
    public AudioClip dictionaryTheme;
    public AudioClip[] switchSFXList;


    [Header("Scenes")]
    public string gameMenuScene = "GameMenu";

    [Header("Voice Preparation UI")]
    public GameObject preparingVoicePanel;
    public GameObject loadingSpinner;

    private const string TutorialPlayedKey = "dictionaryTutorial";

    private List<WordEntry> dictionaryWords;
    private int selectedWordIndex = -1;

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject ttsObject;
    private bool ttsInitialized = false;
    private bool isPreparingVoice = false; 
#endif

    void Start()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null && dictionaryTheme != null)
            AudioManager.Instance.PlayMusic(dictionaryTheme, loop: true);

        LoadDictionary();
        ShowPage(0);

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

        if (backButton != null)
            backButton.onClick.AddListener(GoBackToGameMenu);

        if (nextPageButton != null)
        {
            nextPageButton.onClick.AddListener(() =>
            {
                ShowPage(currentPage + 1);

                if (AudioManager.Instance != null && switchSFXList != null && switchSFXList.Length > 0)
                {
                    int randomIndex = Random.Range(0, switchSFXList.Length);
                    AudioManager.Instance.PlaySFX(switchSFXList[randomIndex]);
                }
            });
        }

        if (prevPageButton != null)
        {
            prevPageButton.onClick.AddListener(() =>
            {
                ShowPage(currentPage - 1);

                if (AudioManager.Instance != null && switchSFXList != null && switchSFXList.Length > 0)
                {
                    int randomIndex = Random.Range(0, switchSFXList.Length);
                    AudioManager.Instance.PlaySFX(switchSFXList[randomIndex]);
                }
            });
        }


        if (voiceButton != null)
            voiceButton.onClick.AddListener(OnVoiceButtonClicked);

        if (detailNextButton != null)
        {
            detailNextButton.onClick.AddListener(() =>
            {
                ShowDetail(selectedWordIndex + 1);

                if (AudioManager.Instance != null && switchSFXList != null && switchSFXList.Length > 0)
                {
                    int randomIndex = Random.Range(0, switchSFXList.Length);
                    AudioManager.Instance.PlaySFX(switchSFXList[randomIndex]);
                }
            });
        }


        if (detailPrevButton != null)
        {
            detailPrevButton.onClick.AddListener(() =>
            {
                ShowDetail(selectedWordIndex - 1);

                if (AudioManager.Instance != null && switchSFXList != null && switchSFXList.Length > 0)
                {
                    int randomIndex = Random.Range(0, switchSFXList.Length);
                    AudioManager.Instance.PlaySFX(switchSFXList[randomIndex]);
                }
            });
        }


        if (detailPanel != null) detailPanel.SetActive(false);
        if (preparingVoicePanel != null) preparingVoicePanel.SetActive(false);
    }

    // ===== Voice Button Click =====
    private void OnVoiceButtonClicked()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    if (!ttsInitialized)
    {
        if (!isPreparingVoice)
        {
            if (voiceButton != null) voiceButton.gameObject.SetActive(false);

            if (loadingSpinner != null) loadingSpinner.SetActive(true);

            ShowPreparingVoiceIndicator();
            InitializeAndroidTTS();
        }
        else
        {
            Debug.Log("Still preparing voice...");
        }
    }
    else
    {
        PlayVoice();
    }
#else
        PlayVoice();
#endif
    }



#if UNITY_ANDROID && !UNITY_EDITOR
    private void InitializeAndroidTTS()
    {
        isPreparingVoice = true; 
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            ttsObject = new AndroidJavaObject("android.speech.tts.TextToSpeech", context, new TTSInitListener(this, context));
        }
    }

    private class TTSInitListener : AndroidJavaProxy
    {
        private readonly Dictionary parent;
        private readonly AndroidJavaObject context;

        public TTSInitListener(Dictionary parent, AndroidJavaObject context)
            : base("android.speech.tts.TextToSpeech$OnInitListener")
        {
            this.parent = parent;
            this.context = context;
        }

        void onInit(int status)
        {
            parent.isPreparingVoice = false; 

            parent.HidePreparingVoiceIndicator(); 

            if (parent.loadingSpinner != null)
                parent.loadingSpinner.SetActive(false);

            if (parent.voiceButton != null)
                parent.voiceButton.gameObject.SetActive(true);

            if (status == 0)
            {
                var locale = new AndroidJavaObject("java.util.Locale", "fil", "PH");
                int result = parent.ttsObject.Call<int>("setLanguage", locale);

                if (result == -1 || result == -2) 
                {
                    Debug.LogWarning("Tagalog voice data not installed. Opening installer...");
                    parent.ttsInitialized = false;

                    AndroidJavaObject installIntent = new AndroidJavaObject(
                        "android.content.Intent", "android.speech.tts.engine.INSTALL_TTS_DATA");
                    context.Call("startActivity", installIntent);
                }
                else 
                {
                    Debug.Log("TTS initialized successfully in Tagalog.");
                    parent.ttsInitialized = true;
                    parent.PlayVoice();
                }
            }
            else 
            {
                Debug.LogWarning("TTS failed to initialize.");
                parent.ttsInitialized = false;
            }
        }
    }
#endif

    // ===== NEW FUNCTIONS =====
    private void ShowPreparingVoiceIndicator()
    {
        if (preparingVoicePanel != null)
        {
            preparingVoicePanel.SetActive(true);
            Debug.Log("Preparing Tagalog voice... Please wait.");
        }
    }

    private void HidePreparingVoiceIndicator()
    {
        if (preparingVoicePanel != null)
        {
            preparingVoicePanel.SetActive(false);
            Debug.Log("Tagalog voice ready!");
        }
    }

    // ===== JSON Loading =====
    void LoadDictionary()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("tagalog_words");
        if (jsonFile != null)
        {
            DictionaryData data = JsonUtility.FromJson<DictionaryData>(jsonFile.text);
            dictionaryWords = new List<WordEntry>(data.words);

            Dictionary<string, WordEntry> uniqueWords = new Dictionary<string, WordEntry>();
            foreach (WordEntry word in dictionaryWords)
            {
                if (!uniqueWords.ContainsKey(word.tagalog))
                    uniqueWords.Add(word.tagalog, word);
            }
            dictionaryWords = new List<WordEntry>(uniqueWords.Values);
            dictionaryWords.Sort((a, b) => string.Compare(a.tagalog, b.tagalog));
        }
        else
        {
            Debug.LogWarning("Dictionary JSON not found in Resources!");
            dictionaryWords = new List<WordEntry>();
        }
    }

    // ===== Pagination =====
    void ShowPage(int page)
    {
        foreach (Transform child in wordListParent)
            Destroy(child.gameObject);

        int totalPages = Mathf.CeilToInt((float)dictionaryWords.Count / wordsPerPage);
        currentPage = Mathf.Clamp(page, 0, totalPages - 1);

        int start = currentPage * wordsPerPage;
        int end = Mathf.Min(start + wordsPerPage, dictionaryWords.Count);

        for (int i = start; i < end; i++)
        {
            WordEntry word = dictionaryWords[i];
            GameObject btnObj = Instantiate(wordButtonPrefab, wordListParent);
            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();

            if (label != null) label.text = $"{word.tagalog} / {word.english}";

            int index = i;
            btnObj.GetComponent<Button>().onClick.AddListener(() => ShowDetail(index));
        }

        prevPageButton.interactable = (currentPage > 0);
        nextPageButton.interactable = (currentPage < totalPages - 1);
    }

    // ===== Detail Panel =====
    void ShowDetail(int index)
    {
        if (index < 0 || index >= dictionaryWords.Count) return;

        StopVoice();

        selectedWordIndex = index;
        WordEntry word = dictionaryWords[index];

        tagalogWordText.text = word.tagalog;
        englishWordText.text = word.english;
        partOfSpeechText.text = word.partOfSpeech;
        tagalogMeaningText.text = word.tagalogMeaning;
        englishMeaningText.text = word.englishMeaning;

        detailPanel.SetActive(true);

        detailPrevButton.interactable = (index > 0);
        detailNextButton.interactable = (index < dictionaryWords.Count - 1);
    }

    // ===== TTS Play Voice =====
    void PlayVoice()
    {
        if (selectedWordIndex < 0 || selectedWordIndex >= dictionaryWords.Count) return;
        string wordToSpeak = dictionaryWords[selectedWordIndex].tagalog;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (ttsObject != null && ttsInitialized)
        {
            ttsObject.Call<int>("speak", wordToSpeak, 0, null, null);
        }
        else
        {
            Debug.LogWarning("TTS not initialized yet. Initializing now...");
            InitializeAndroidTTS();
        }
#else
        Debug.Log($"[TTS] Would speak (Tagalog): {wordToSpeak}");
#endif
    }

    // ===== Navigation =====
    public void GoBackToGameMenu()
    {
        LoadingScene.LoadSceneWithLoading(gameMenuScene);
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
        if (backButton != null) backButton.interactable = enable;
        if (nextPageButton != null) nextPageButton.interactable = enable;
        if (prevPageButton != null) prevPageButton.interactable = enable;

        foreach (Transform child in wordListParent)
        {
            Button btn = child.GetComponent<Button>();
            if (btn != null) btn.interactable = enable;
        }
    }

    private void OnDisable()
    {
        StopVoice();
    }
    
    private void StopVoice()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    if (ttsObject != null)
    {
        ttsObject.Call("stop");
        Debug.Log("TTS stopped.");
    }
#else
        Debug.Log("[TTS] StopVoice() called (simulation in Editor).");
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnDestroy()
    {
        if (ttsObject != null)
        {
            ttsObject.Call("stop");
            ttsObject.Call("shutdown");
            ttsObject.Dispose();
            ttsObject = null;
        }
    }
#endif
}

[System.Serializable]
public class

WordEntry
{
    public string tagalog;
    public string english;
    public string partOfSpeech;
    public string tagalogMeaning;
    public string englishMeaning;
}

[System.Serializable]
public class DictionaryData
{
    public WordEntry[] words;
}
