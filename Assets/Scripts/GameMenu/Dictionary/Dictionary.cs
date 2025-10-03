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

    [Header("Scenes")]
    public string gameMenuScene = "GameMenu";

    private const string TutorialPlayedKey = "dictionaryTutorial";

    private List<WordEntry> dictionaryWords;
    private int selectedWordIndex = -1;

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
            nextPageButton.onClick.AddListener(() => ShowPage(currentPage + 1));

        if (prevPageButton != null)
            prevPageButton.onClick.AddListener(() => ShowPage(currentPage - 1));

        if (voiceButton != null)
            voiceButton.onClick.AddListener(PlayVoice);

        if (detailNextButton != null)
            detailNextButton.onClick.AddListener(() => ShowDetail(selectedWordIndex + 1));

        if (detailPrevButton != null)
            detailPrevButton.onClick.AddListener(() => ShowDetail(selectedWordIndex - 1));

        if (detailPanel != null) detailPanel.SetActive(false);
    }

    // ===== JSON Loading =====
    void LoadDictionary()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("tagalog_words");
        if (jsonFile != null)
        {
            DictionaryData data = JsonUtility.FromJson<DictionaryData>(jsonFile.text);
            dictionaryWords = new List<WordEntry>(data.words);

            // --- FUTURE-PROOF: remove duplicates based on Tagalog word ---
            Dictionary<string, WordEntry> uniqueWords = new Dictionary<string, WordEntry>();
            foreach (WordEntry word in dictionaryWords)
            {
                if (!uniqueWords.ContainsKey(word.tagalog))
                    uniqueWords.Add(word.tagalog, word);
            }
            dictionaryWords = new List<WordEntry>(uniqueWords.Values);

            // --- FUTURE-PROOF: sort alphabetically by Tagalog word ---
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

    void PlayVoice()
    {
        if (selectedWordIndex >= 0 && selectedWordIndex < dictionaryWords.Count)
        {
            WordEntry word = dictionaryWords[selectedWordIndex];
            // Placeholder: replace with actual TTS or pre-recorded clip
            Debug.Log($"Play Tagalog voice for: {word.tagalog}");
            // Example if pre-recorded audio is linked:
            // AudioManager.Instance.PlaySFX(word.audioClip);
        }
    }

    // ===== Navigation =====
    public void GoBackToGameMenu()
    {
        SceneManager.LoadScene(gameMenuScene);
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
}

[System.Serializable]
public class WordEntry
{
    public string tagalog;
    public string english;
    public string partOfSpeech;
    public string tagalogMeaning;
    public string englishMeaning;
    // Optionally add public string audioClipPath;
}

[System.Serializable]
public class DictionaryData
{
    public WordEntry[] words;
}
