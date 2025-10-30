using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

[System.Serializable]
public class QuizQuestion
{
    public int questionID;
    public string question;
    public string correctAnswer;
    public string choiceA;
    public string choiceB;
    public string choiceC;
    public string choiceD;
    public bool isAnswered;
    public bool isEncountered;
}

[System.Serializable]
public class QuizData
{
    public List<QuizQuestion> questions;
}

public class QuizSpanish : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject startPanel;
    public GameObject quizPanel;
    public GameObject countdownPanel;
    public GameObject feedbackPanel;
    public GameObject gameOverPanel;
    public GameObject quitModalPanel;
    public GameObject unlockPopupPanel;

    [Header("UI Elements")]
    public TMP_Text countdownText;
    public TMP_Text questionText;
    public TMP_Text scoreText;
    public Button[] choiceButtons;
    public TMP_Text feedbackText;
    public TMP_Text questionNumberText;
    public TMP_Text finalScoreText;
    public TMP_Text motivationalText;
    public TMP_Text highscoreText;
    public TMP_Text unlockPopupText;

    [Header("Audio")]
    public AudioClip quizTheme;

    [Header("Audio Clips")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;


    [Header("Confetti Effect")]
    public ConfettiAnimation confettiEffect;

    [Header("Navigation")]
    public Button backButton;
    public string gameMenuScene = "GameMenu";

    [Header("JSON Settings")]
    public string jsonFileName = "QuizSpanish.json";

    [Header("Feedback messages (editable in Inspector)")]
    public List<string> correctResponses = new List<string> {
        "Correct!", "Nice!", "Great!", "You got it!", "Right answer!"
    };
    public List<string> wrongResponses = new List<string> {
        "Wrong!", "Not quite.", "Oops!", "That's incorrect.", "Try the next one!"
    };

    [Header("End-of-quiz motivational messages")]
    public List<string> successMessages = new List<string> {
        "Fantastic job! You unlocked the QuizAmerica scene!",
        "Excellent work — new quiz unlocked!",
        "Bravo! QuizAmerica is now available!"
    };
    public List<string> retryMessages = new List<string> {
        "Keep trying — you'll get it next time!",
        "Don't give up! Practice makes perfect.",
        "Good effort! Try again and beat your score."
    };

    [Header("Gameplay settings")]
    public int sessionQuestionLimit = 20;
    public float feedbackDisplaySeconds = 3f;
    public int unlockScoreThreshold = 15;

    private List<QuizQuestion> questions = new List<QuizQuestion>();
    private List<QuizQuestion> sessionQuestions = new List<QuizQuestion>();
    private QuizQuestion currentQuestion;
    private int sessionQuestionIndex = 0;
    private int currentScore = 0;

    private const string PREF_CURRENT_SCORE = "currentscore";
    private const string PREF_HIGH_SCORE = "highscore";
    private const string PREF_ENCOUNTERED_COUNT = "EncounteredCount";
    private const string PREF_TOTAL_QUESTIONS = "TotalQuestions";
    private const string PREF_IS_AMERICA_UNLOCKED = "isAmericaQuizUnlocked";

    private void Start()
    {
        EnsureMessageDefaults();

        if (AudioManager.Instance != null && quizTheme != null)
        {
            AudioManager.Instance.PlayMusic(quizTheme, loop: true);
        }

        StartCoroutine(LoadQuizData());

        quizPanel.SetActive(false);
        countdownPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        quitModalPanel.SetActive(false);
        if (unlockPopupPanel != null) unlockPopupPanel.SetActive(false);
    }

    private void EnsureMessageDefaults()
    {
        if (correctResponses == null || correctResponses.Count == 0)
            correctResponses = new List<string> { "Correct!" };
        if (wrongResponses == null || wrongResponses.Count == 0)
            wrongResponses = new List<string> { "Wrong!" };
        if (successMessages == null || successMessages.Count == 0)
            successMessages = new List<string> { "Great job!" };
        if (retryMessages == null || retryMessages.Count == 0)
            retryMessages = new List<string> { "Keep trying!" };
    }

    public void GoToDictionary()
    {
        SceneManager.LoadScene(gameMenuScene);
    }

    #region Start Game
    public void StartQuiz()
    {
        foreach (var q in questions) q.isAnswered = false;

        currentScore = 0;
        PlayerPrefs.SetInt(PREF_CURRENT_SCORE, 0);
        PlayerPrefs.Save();

        BuildSessionPool();

        startPanel.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    private void BuildSessionPool()
    {
        var unanswered = questions.Where(q => !q.isAnswered).ToList();

        if (unanswered.Count < sessionQuestionLimit)
        {
            int needed = sessionQuestionLimit - unanswered.Count;
            var answered = questions.Where(q => q.isAnswered)
                                    .OrderBy(x => Random.value)
                                    .Take(needed)
                                    .ToList();
            unanswered.AddRange(answered);
        }

        sessionQuestions = unanswered.OrderBy(x => Random.value)
                                     .Take(sessionQuestionLimit)
                                     .ToList();

        sessionQuestionIndex = 0;
    }

    private IEnumerator StartCountdown()
    {
        countdownPanel.SetActive(true);
        int countdown = 3;
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownPanel.SetActive(false);
        quizPanel.SetActive(true);
        ShowNextQuestion();
    }
    #endregion

    #region Question Logic
    private IEnumerator LoadQuizData()
    {
        string persistentPath = Path.Combine(Application.persistentDataPath, jsonFileName);

        if (File.Exists(persistentPath))
        {
            string savedJson = File.ReadAllText(persistentPath);
            QuizData data = JsonUtility.FromJson<QuizData>(savedJson);
            questions = data.questions ?? new List<QuizQuestion>();

            if (PlayerPrefs.GetInt(PREF_ENCOUNTERED_COUNT, -1) <= 0)
            {
                Debug.Log("PlayerPrefs cleared — resetting question flags (answered + encountered).");
                ResetAllQuestionFlags();
            }

            yield break;
        }


        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                string jsonString = request.downloadHandler.text;
                QuizData data = JsonUtility.FromJson<QuizData>(jsonString);
                questions = data.questions ?? new List<QuizQuestion>();

                File.WriteAllText(persistentPath, jsonString);

                ResetAllQuestionFlags();
            }
            else
            {
                Debug.LogError("Failed to load JSON: " + request.error);
            }
        }
#else
        if (File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);
            QuizData data = JsonUtility.FromJson<QuizData>(jsonString);
            questions = data.questions ?? new List<QuizQuestion>();

            File.WriteAllText(persistentPath, jsonString);

            ResetAllQuestionFlags();
        }
        else
        {
            Debug.LogError("JSON file not found at " + path);
        }
#endif
    }

    private void ShowNextQuestion()
    {
        if (sessionQuestionIndex >= sessionQuestionLimit)
        {
            EndQuiz();
            return;
        }

        currentQuestion = sessionQuestions[sessionQuestionIndex];
        UpdateQuestionUI();
    }

    private void UpdateQuestionUI()
    {
        questionText.text = currentQuestion.question;
        questionNumberText.text = $"{sessionQuestionIndex + 1}/{sessionQuestionLimit}";
        scoreText.text = $"{currentScore}";

        List<string> choices = new List<string>
        {
            currentQuestion.choiceA,
            currentQuestion.choiceB,
            currentQuestion.choiceC,
            currentQuestion.choiceD
        };

        choices = choices.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i >= choices.Count) break;

            int index = i;
            var btn = choiceButtons[i];
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = choices[i];

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnChoiceSelected(choices[index]));
            btn.interactable = true;
        }
    }

    private void OnChoiceSelected(string selectedChoice)
    {
        StartCoroutine(HandleFeedback(selectedChoice));
    }

    private IEnumerator HandleFeedback(string selectedChoice)
    {
        SetChoicesInteractable(false);

        Button selectedButton = null;

        foreach (var btn in choiceButtons)
        {
            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null && label.text == selectedChoice)
            {
                selectedButton = btn;
                break;
            }
        }

        feedbackPanel.SetActive(true);
        bool isCorrect = selectedChoice == currentQuestion.correctAnswer;

        if (isCorrect)
        {
            feedbackText.text = correctResponses[Random.Range(0, correctResponses.Count)];
            currentQuestion.isAnswered = true;

            if (!currentQuestion.isEncountered)
            {
                currentQuestion.isEncountered = true;
                UpdateEncounteredProgress();
            }

            if (selectedButton != null)
                selectedButton.image.color = new Color32(50, 207, 56, 255); 
            currentScore++;
            PlayerPrefs.SetInt(PREF_CURRENT_SCORE, currentScore);
            scoreText.text = $"{currentScore}";

            if (AudioManager.Instance != null && correctSFX != null)
                AudioManager.Instance.PlaySFX(correctSFX);
        }
        else
        {
            feedbackText.text = wrongResponses[Random.Range(0, wrongResponses.Count)];
            currentQuestion.isAnswered = true;

            if (selectedButton != null)
                selectedButton.image.color = new Color32(219, 70, 70, 255);

            if (AudioManager.Instance != null && wrongSFX != null)
                AudioManager.Instance.PlaySFX(wrongSFX);
        }

        SaveQuizData();

        StartCoroutine(ShowFloatingIcon(isCorrect));

        yield return new WaitForSeconds(feedbackDisplaySeconds);

        foreach (var btn in choiceButtons)
        {
            if (btn != null && btn.image != null)
                btn.image.color = Color.white;
        }

        feedbackPanel.SetActive(false);

        sessionQuestionIndex++;
        SetChoicesInteractable(true);
        ShowNextQuestion();
    }

    [SerializeField] private TMP_Text checkIcon;
    [SerializeField] private TMP_Text xIcon;

    private IEnumerator ShowFloatingIcon(bool isCorrect)
    {
        TMP_Text targetIcon = isCorrect ? checkIcon : xIcon;
        if (targetIcon == null) yield break;

        targetIcon.gameObject.SetActive(true);

        Color startColor = targetIcon.color;
        startColor.a = 0;
        targetIcon.color = startColor;

        Vector3 startPos = targetIcon.rectTransform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 50f, 0);
        float duration = 2f;
        float half = duration / 2f;
        float elapsed = 0f;


        while (elapsed < half)
        {
            float t = elapsed / half;
            targetIcon.color = new Color(targetIcon.color.r, targetIcon.color.g, targetIcon.color.b, t);
            targetIcon.rectTransform.localPosition = Vector3.Lerp(startPos, endPos, t * 0.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        while (elapsed < duration)
        {
            float t = (elapsed - half) / half;
            targetIcon.color = new Color(targetIcon.color.r, targetIcon.color.g, targetIcon.color.b, 1f - t);
            targetIcon.rectTransform.localPosition = Vector3.Lerp(startPos + (Vector3.up * 25f), endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetIcon.gameObject.SetActive(false);
        targetIcon.rectTransform.localPosition = startPos;
    }


    private void SetChoicesInteractable(bool state)
    {
        if (choiceButtons == null) return;
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].interactable = state;
        }
    }
    #endregion

    #region End Quiz
    private void EndQuiz()
    {
        quizPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        UpdateEncounteredProgress();

        int highscore = PlayerPrefs.GetInt(PREF_HIGH_SCORE, 0);
        if (currentScore > highscore)
        {
            PlayerPrefs.SetInt(PREF_HIGH_SCORE, currentScore);
        }

        string chosenMessage;

        if (currentScore >= unlockScoreThreshold && successMessages.Count > 0)
        {
            PlayerPrefs.SetInt(PREF_IS_AMERICA_UNLOCKED, 1);
            chosenMessage = successMessages[Random.Range(0, successMessages.Count)];
            if (unlockPopupPanel != null && unlockPopupText != null)
                StartCoroutine(ShowUnlockPopup("Story and Quiz for American Chapter Unlocked!"));
        }
        else
        {
            chosenMessage = (retryMessages.Count > 0)
                ? retryMessages[Random.Range(0, retryMessages.Count)]
                : "Keep trying!";
        }

        if (currentScore >= 15)
        {
            if (confettiEffect != null)
            {
                confettiEffect.PlayConfetti();
            }
            else
            {
                Debug.LogWarning("ConfettiAnimation reference missing in Inspector!");
            }
        }

        motivationalText.text = chosenMessage;
        finalScoreText.text = $"Score: {currentScore}/{sessionQuestionLimit}";
        highscoreText.text = $"Highscore: {PlayerPrefs.GetInt(PREF_HIGH_SCORE, 0)}";

        PlayerPrefs.Save();
    }

    private IEnumerator ShowUnlockPopup(string message)
    {
        unlockPopupText.text = message;
        unlockPopupPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        unlockPopupPanel.SetActive(false);
    }

    public void NewQuizSession()
    {
        foreach (var q in questions) q.isAnswered = false;
        currentScore = 0;
        PlayerPrefs.SetInt(PREF_CURRENT_SCORE, 0);
        PlayerPrefs.Save();

        BuildSessionPool();

        gameOverPanel.SetActive(false);
        StartCoroutine(StartCountdown());
    }
    #endregion

    #region Quit Modal
    public void OpenQuitModal()
    {
        if (quitModalPanel != null) quitModalPanel.SetActive(true);
    }

    public void ConfirmQuit()
    {
        quizPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        countdownPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        if (unlockPopupPanel != null) unlockPopupPanel.SetActive(false);
        if (quitModalPanel != null) quitModalPanel.SetActive(false);

        currentScore = 0;
        sessionQuestionIndex = 0;
        foreach (var q in questions)
            q.isAnswered = false;

        startPanel.SetActive(true);
    }

    public void CancelQuit()
    {
        if (quitModalPanel != null) quitModalPanel.SetActive(false);
    }
    #endregion

    #region JSON Save / Reset
    private void SaveQuizData()
    {
        string persistentPath = Path.Combine(Application.persistentDataPath, jsonFileName);
        QuizData data = new QuizData { questions = questions };
        string jsonString = JsonUtility.ToJson(data, true);
        File.WriteAllText(persistentPath, jsonString);
    }

    private void UpdateEncounteredProgress()
    {
        int encounteredCount = questions.Count(q => q.isEncountered);
        PlayerPrefs.SetInt(PREF_ENCOUNTERED_COUNT, encounteredCount);
        PlayerPrefs.SetInt(PREF_TOTAL_QUESTIONS, questions.Count);
        PlayerPrefs.Save();
    }

    private void ResetAllQuestionFlags()
    {
        if (questions == null || questions.Count == 0) return;

        foreach (var q in questions)
        {
            q.isEncountered = false;
            q.isAnswered = false;
        }

        SaveQuizData();

        PlayerPrefs.SetInt(PREF_ENCOUNTERED_COUNT, 0);
        PlayerPrefs.SetInt(PREF_CURRENT_SCORE, 0);
        PlayerPrefs.Save();

        Debug.Log("Cache cleared or missing — all question flags reset.");
    }
    #endregion
}
