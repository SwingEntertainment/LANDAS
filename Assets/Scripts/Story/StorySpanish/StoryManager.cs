using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

[Serializable]
public class SlideData
{
    public int slideIndex;
    public string slideImg;
    public string[] slideText;
}

[Serializable]
public class QuizTryQuestion
{
    public string q;
    public string choiceA;
    public string choiceB;
    public string correct;
}

[Serializable]
public class QuizSegment
{
    public int triggerAfterSlide;
    public QuizTryQuestion[] questions;
}

[Serializable]
public class Subchapter
{
    public string subchapterID;
    public string title;
    public string thumbnail;
    public string imageTitle;
    public bool isRead;
    public SlideData[] slides;
    public QuizSegment[] quizSegments;
}

[Serializable]
public class SubchapterCollection
{
    public Subchapter[] subchapters;
}

public class StoryManager : MonoBehaviour
{
    [Header("JSON / File")]
    public string jsonFileName = "spanishChapters.json";
    public bool copyStreamingToPersistentIfMissing = true;

    // -------- LISTING UI (card view with prev/next) --------
    [Header("Listing UI")]
    public Image listingThumbnail;
    public TMP_Text listingTitle;
    public Image listingCheckmark;
    public Button prevButton;
    public Button nextButton;
    public Button readButton;
    public Toggle voiceToggle;
    public GameObject confirmReadModal;

    // -------- SLIDE UI --------
    [Header("Slide UI")]
    public GameObject slidePanel;
    public Image slideImage;
    public TMP_Text slideText;
    public CanvasGroup slideTextCanvasGroup;
    public Button slideNextButton;
    public Button slideCancelButton;
    public Button storyBackButton;
    public GameObject exitConfirmModal;
    public GameObject finishPanel;
    public Button finishBackToListButton;

    // -------- QUIZ UI (modal) --------
    [Header("Quiz UI")]
    public GameObject quizModal;
    public TMP_Text quizQuestionText;
    public Button quizChoiceAButton;
    public Button quizChoiceBButton;
    public TMP_Text quizChoiceAText;
    public TMP_Text quizChoiceBText;

    [Header("Editor fallback quiz (if JSON missing)")]
    [Tooltip("If JSON doesn't contain quizSegments, this editor list will be used as a fallback quiz after slide #10.")]
    public QuizTryQuestion[] editorQuiz;

    [Header("Misc")]
    public float textFadeDuration = 0.45f;
    public int quizTriggerSlideIndexDefault = 10;

    // ---------- Internal state ----------
    SubchapterCollection collection;
    List<Subchapter> subchapters = new List<Subchapter>();
    int currentListingIndex = 0;

    // slide playback state:
    Subchapter currentSubchapter;
    int currentSlideZeroIndex = 0;
    int currentTextChunkIndex = 0;
    string[] currentTextChunks;
    bool voiceEnabled = true;
    bool quizActive = false;

    // TTS objects (Android)
    AndroidJavaObject ttsObj = null;
    AndroidJavaObject unityActivity = null;

    // persistent JSON path
    string PersistentPath => Path.Combine(Application.persistentDataPath, jsonFileName);
    string StreamingPath => Path.Combine(Application.streamingAssetsPath, jsonFileName);

    void Awake()
    {
        prevButton.onClick.AddListener(OnPrevListing);
        nextButton.onClick.AddListener(OnNextListing);
        readButton.onClick.AddListener(OnReadButtonPressed);
        slideNextButton.onClick.AddListener(OnSlideNextClicked);
        slideCancelButton.onClick.AddListener(OnSlideCancelClicked);
        finishBackToListButton.onClick.AddListener(OnFinishBackToList);

        voiceToggle.onValueChanged.AddListener((v) => voiceEnabled = v);
        voiceEnabled = voiceToggle.isOn;
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(EnsureJsonCopiedIfNeeded());
        yield return StartCoroutine(LoadJson());
        UpdateListingUI();
        InitTTSIfAndroid();
    }

    #region JSON Loading / Saving
    IEnumerator EnsureJsonCopiedIfNeeded()
    {
        if (!copyStreamingToPersistentIfMissing) yield break;

        if (!File.Exists(PersistentPath))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (UnityWebRequest uwr = UnityWebRequest.Get(StreamingPath))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllText(PersistentPath, uwr.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("Failed to copy JSON from StreamingAssets: " + uwr.error);
                }
            }
#else
            if (File.Exists(StreamingPath))
            {
                File.Copy(StreamingPath, PersistentPath);
            }
            else
            {
                Debug.LogError("Streaming JSON not found at " + StreamingPath);
            }
            yield return null;
#endif
        }
        else yield return null;
    }

    IEnumerator LoadJson()
    {
        if (!File.Exists(PersistentPath))
        {
            Debug.LogError("Persistent JSON not found at " + PersistentPath);
            yield break;
        }

        string json = File.ReadAllText(PersistentPath);
        if (json.TrimStart().StartsWith("["))
        {
            json = "{\"subchapters\":" + json + "}";
        }

        collection = JsonUtility.FromJson<SubchapterCollection>(json);
        if (collection == null || collection.subchapters == null)
        {
            Debug.LogError("Failed to parse JSON or no subchapters found.");
            yield break;
        }

        subchapters = new List<Subchapter>(collection.subchapters);
        Debug.Log($"Loaded {subchapters.Count} subchapters.");
        yield return null;
    }

    void SaveJson()
    {
        if (collection == null) collection = new SubchapterCollection();
        collection.subchapters = subchapters.ToArray();
        string json = JsonUtility.ToJson(collection, true);
        try
        {
            File.WriteAllText(PersistentPath, json);
            Debug.Log("Saved subchapters to " + PersistentPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save JSON: " + e);
        }
    }
    #endregion

    #region Listing UI
    void UpdateListingUI()
    {
        if (subchapters == null || subchapters.Count == 0) return;
        currentListingIndex = Mathf.Clamp(currentListingIndex, 0, subchapters.Count - 1);
        var s = subchapters[currentListingIndex];

        listingTitle.text = s.title;
        listingCheckmark.gameObject.SetActive(s.isRead);

        Debug.Log($"[StoryManager] Loading thumbnail: {s.thumbnail}");

        if (!string.IsNullOrEmpty(s.thumbnail))
        {
            StartCoroutine(LoadSpriteFromStreamingAssets(s.thumbnail, (sp) =>
            {
                if (sp != null)
                {
                    listingThumbnail.sprite = sp;
                    listingThumbnail.color = Color.white; // 🔹 ensures it's visible
                    Debug.Log("[StoryManager] Thumbnail loaded successfully!");
                }
                else
                {
                    Debug.LogWarning("[StoryManager] Thumbnail sprite was null!");
                }
            }));
        }
        else
        {
            Debug.LogWarning("[StoryManager] No thumbnail path in JSON!");
            listingThumbnail.sprite = null;
        }

        prevButton.interactable = currentListingIndex > 0;
        nextButton.interactable = currentListingIndex < subchapters.Count - 1;
    }


    void OnPrevListing()
    {
        if (currentListingIndex > 0)
        {
            currentListingIndex--;
            UpdateListingUI();
        }
    }

    void OnNextListing()
    {
        if (currentListingIndex < subchapters.Count - 1)
        {
            currentListingIndex++;
            UpdateListingUI();
        }
    }

    void OnReadButtonPressed()
    {
        confirmReadModal.SetActive(true);
    }

    public void ConfirmReadYes()
    {
        confirmReadModal.SetActive(false);
        OpenSlidePanelForIndex(currentListingIndex);
    }

    public void ConfirmReadNo()
    {
        confirmReadModal.SetActive(false);
    }
    #endregion

    #region Slide Playback
    void OpenSlidePanelForIndex(int idx)
    {
        if (idx < 0 || idx >= subchapters.Count) return;
        currentSubchapter = subchapters[idx];
        currentSlideZeroIndex = 0;
        currentTextChunkIndex = 0;
        quizActive = false;
        slidePanel.SetActive(true);
        exitConfirmModal.SetActive(false);
        finishPanel.SetActive(false);
        quizModal.SetActive(false);
        ShowSlide(currentSlideZeroIndex);
    }

    void ShowSlide(int zeroBasedIndex)
    {
        if (currentSubchapter == null) return;
        if (zeroBasedIndex < 0 || zeroBasedIndex >= currentSubchapter.slides.Length)
        {
            OnSlidesFinished();
            return;
        }

        SlideData slide = currentSubchapter.slides[zeroBasedIndex];
        currentTextChunks = slide.slideText ?? new string[0];
        currentTextChunkIndex = 0;
        slideText.text = "";
        if (slideTextCanvasGroup != null)
            slideTextCanvasGroup.alpha = 0f;

        // ✅ SAFELY LOAD IMAGE
        if (slideImage != null && !string.IsNullOrEmpty(slide.slideImg))
        {
            string slidePath = Path.Combine(Application.streamingAssetsPath, slide.slideImg);

            if (File.Exists(slidePath))
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(slidePath);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(bytes);
                    slideImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load image at {slidePath}: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"❌ Slide image not found: {slidePath}");
                slideImage.sprite = null;
            }
        }
        else
        {
            Debug.LogWarning("slideImage is null or slide.slideImg is empty!");
            if (slideImage != null)
                slideImage.sprite = null;
        }
    }


    void OnSlideNextClicked()
    {
        if (quizActive) return;

        if (currentTextChunks != null && currentTextChunkIndex < currentTextChunks.Length)
        {
            string chunk = currentTextChunks[currentTextChunkIndex];
            StartCoroutine(FadeInTextChunk(chunk));
            if (voiceEnabled) Speak(chunk);
            currentTextChunkIndex++;
            return;
        }

        if (currentSubchapter == null)
        {
            Debug.LogError("Current subchapter is NULL in OnSlideNextClicked!");
            return;
        }

        if (currentSubchapter.slides == null || currentSubchapter.slides.Length == 0)
        {
            Debug.LogError("Slides array is NULL or EMPTY in currentSubchapter!");
            return;
        }

        if (currentSlideZeroIndex < 0 || currentSlideZeroIndex >= currentSubchapter.slides.Length)
        {
            Debug.LogError($"Invalid slide index: {currentSlideZeroIndex}. Slides length: {currentSubchapter.slides.Length}");
            return;
        }

        int currentSlideIndex1Based = currentSubchapter.slides[currentSlideZeroIndex].slideIndex;
        QuizSegment triggeredSegment = null;
        if (currentSubchapter.quizSegments != null)
        {
            foreach (var seg in currentSubchapter.quizSegments)
            {
                if (seg.triggerAfterSlide == currentSlideIndex1Based)
                {
                    triggeredSegment = seg;
                    break;
                }
            }
        }

        if (triggeredSegment != null)
        {
            ShowQuiz(triggeredSegment.questions);
            return;
        }
        else
        {
            if (editorQuiz != null && editorQuiz.Length > 0 && currentSlideIndex1Based >= quizTriggerSlideIndexDefault)
            {
                if (!quizActive)
                {
                    ShowQuiz(editorQuiz);
                    return;
                }
            }
        }

        currentSlideZeroIndex++;
        if (currentSlideZeroIndex >= currentSubchapter.slides.Length)
        {
            OnSlidesFinished();
            return;
        }
        ShowSlide(currentSlideZeroIndex);
    }

    IEnumerator FadeInTextChunk(string text)
    {
        if (slideTextCanvasGroup != null) slideTextCanvasGroup.alpha = 0f;
        slideText.text = text;
        float elapsed = 0f;
        while (elapsed < textFadeDuration)
        {
            elapsed += Time.deltaTime;
            if (slideTextCanvasGroup != null) slideTextCanvasGroup.alpha = Mathf.Clamp01(elapsed / textFadeDuration);
            yield return null;
        }
        if (slideTextCanvasGroup != null) slideTextCanvasGroup.alpha = 1f;
    }

    void OnSlideCancelClicked()
    {
        exitConfirmModal.SetActive(true);
    }

    public void ExitConfirmYes()
    {
        exitConfirmModal.SetActive(false);
        currentSlideZeroIndex = 0;
        currentTextChunkIndex = 0;
        slidePanel.SetActive(false);
        UpdateListingUI();
    }

    public void ExitConfirmNo()
    {
        exitConfirmModal.SetActive(false);
    }

    void OnSlidesFinished()
    {
        currentSubchapter.isRead = true;
        SaveJson();
        finishPanel.SetActive(true);
    }

    void OnFinishBackToList()
    {
        finishPanel.SetActive(false);
        slidePanel.SetActive(false);
        UpdateListingUI();
    }
    #endregion

    #region Quiz
    void ShowQuiz(QuizTryQuestion[] questions)
    {
        if (questions == null || questions.Length == 0) return;
        quizActive = true;

        if (slideTextCanvasGroup != null) slideTextCanvasGroup.gameObject.SetActive(false);
        if (storyBackButton != null) storyBackButton.gameObject.SetActive(false);
        quizModal.SetActive(true);
        StartCoroutine(RunQuizSequence(questions));
    }

    IEnumerator RunQuizSequence(QuizTryQuestion[] questions)
    {
        int idx = 0;
        while (idx < questions.Length)
        {
            var q = questions[idx];
            quizQuestionText.text = q.q;
            quizChoiceAText.text = q.choiceA;
            quizChoiceBText.text = q.choiceB;

            quizChoiceAButton.gameObject.SetActive(true);
            quizChoiceBButton.gameObject.SetActive(true);
            bool answered = false;
            bool answeredCorrect = false;

            quizChoiceAButton.onClick.RemoveAllListeners();
            quizChoiceBButton.onClick.RemoveAllListeners();

            quizChoiceAButton.onClick.AddListener(() =>
            {
                if (q.correct == "A" || q.correct == "a")
                {
                    answeredCorrect = true;
                    answered = true;
                }
                else
                {
                    quizChoiceAButton.gameObject.SetActive(false);
                }
            });

            quizChoiceBButton.onClick.AddListener(() =>
            {
                if (q.correct == "B" || q.correct == "b")
                {
                    answeredCorrect = true;
                    answered = true;
                }
                else
                {
                    quizChoiceBButton.gameObject.SetActive(false);
                }
            });

            while (!answered)
                yield return null;

            if (answeredCorrect)
            {
                idx++;
                yield return new WaitForSeconds(0.25f);
            }
        }

        quizModal.SetActive(false);
        if (slideTextCanvasGroup != null) slideTextCanvasGroup.gameObject.SetActive(true);
        if (storyBackButton != null) storyBackButton.gameObject.SetActive(true);
        quizActive = false;
        currentSlideZeroIndex++;
        if (currentSlideZeroIndex >= currentSubchapter.slides.Length)
        {
            OnSlidesFinished();
        }
        else ShowSlide(currentSlideZeroIndex);

        yield break;
    }

    #endregion

    #region Image Loading Helpers
    IEnumerator LoadTextureAsSpriteFromPath(string fullPath, Action<Sprite> onComplete)
    {
        if (string.IsNullOrEmpty(fullPath))
        {
            onComplete?.Invoke(null);
            yield break;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(fullPath))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var tex = DownloadHandlerTexture.GetContent(uwr);
                var sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
                onComplete?.Invoke(sp);
            }
            else
            {
                Debug.LogWarning("Image load failed: " + uwr.error + " path:" + fullPath);
                onComplete?.Invoke(null);
            }
        }
#else
        string usePath = fullPath;
        if (!usePath.StartsWith("file://")) usePath = "file://" + fullPath;
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(usePath))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var tex = DownloadHandlerTexture.GetContent(uwr);
                var sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
                onComplete?.Invoke(sp);
            }
            else
            {
                Debug.LogWarning("Image load failed: " + uwr.error + " path:" + usePath);
                onComplete?.Invoke(null);
            }
        }
#endif
    }

    IEnumerator LoadSpriteFromStreamingAssets(string relativePath, Action<Sprite> onComplete)
    {
        string persistentCandidate = Path.Combine(Application.persistentDataPath, relativePath);
        if (File.Exists(persistentCandidate))
        {
            yield return LoadTextureAsSpriteFromPath("file://" + persistentCandidate, onComplete);
            yield break;
        }

        string streamingCandidate;
#if UNITY_ANDROID && !UNITY_EDITOR
        streamingCandidate = Path.Combine(Application.streamingAssetsPath, relativePath);
#else
        streamingCandidate = Path.Combine(Application.streamingAssetsPath, relativePath);
#endif
        yield return LoadTextureAsSpriteFromPath(streamingCandidate, onComplete);
    }
    #endregion

    #region TTS (Android only, offline)
    void InitTTSIfAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            ttsObj = new AndroidJavaObject("android.speech.tts.TextToSpeech", unityActivity, new TTSListenerProxy());
            AndroidJavaObject locale = new AndroidJavaObject("java.util.Locale", "en", "US");
            int result = ttsObj.Call<int>("setLanguage", locale);
            Debug.Log("TTS init done, setLanguage result: " + result);
        }
        catch (Exception e)
        {
            Debug.LogWarning("TTS init failed: " + e.Message);
            ttsObj = null;
        }
#else
#endif
    }

    void Speak(string text)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (ttsObj == null) return;
        try
        {
            ttsObj.Call<int>("speak", text, 0, null as AndroidJavaObject, null);
        }
        catch (Exception e)
        {
            Debug.LogWarning("TTS speak error: " + e.Message);
        }
#else
        Debug.Log("[TTS] " + text);
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    class TTSListenerProxy : AndroidJavaProxy
    {
        public TTSListenerProxy() : base("android.speech.tts.TextToSpeech$OnInitListener") { }
        public void onInit(int status) { }
    }
#endif
    #endregion
}
