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

    [Header("UI Elements")]
    public TMP_Text countdownText;
    public TMP_Text questionText;
    public Button[] choiceButtons;
    public TMP_Text feedbackText;
    public TMP_Text questionNumberText;
    public TMP_Text finalScoreText;
    public TMP_Text motivationalText;
    public TMP_Text highscoreText;

    [Header("Audio")]
    public AudioClip quizTheme;

    [Header("Navigation")]
    public Button backButton;
    public string gameMenuScene = "GameMenu";

    [Header("JSON Settings")]
    public string jsonFileName = "QuizSpanish.json";

    private List<QuizQuestion> questions = new List<QuizQuestion>();
    private List<QuizQuestion> sessionQuestions = new List<QuizQuestion>();
    private QuizQuestion currentQuestion;
    private int sessionQuestionIndex = 0;
    private int currentScore = 0;
    private const int SESSION_QUESTION_LIMIT = 30;

    private void Start()
    {
        // Play background music
        if (AudioManager.Instance != null && quizTheme != null)
        {
            AudioManager.Instance.PlayMusic(quizTheme, loop: true);
        }

        LoadQuizData();
        UpdateEncounteredProgress();
        quizPanel.SetActive(false);
        countdownPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        gameOverPanel.SetActive(false);
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
        PlayerPrefs.SetInt("currentscore", 0);

        BuildSessionPool();

        startPanel.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    private void BuildSessionPool()
    {
        // Get unanswered first
        var unanswered = questions.Where(q => !q.isAnswered).ToList();

        // Fill with answered if needed
        if (unanswered.Count < SESSION_QUESTION_LIMIT)
        {
            int needed = SESSION_QUESTION_LIMIT - unanswered.Count;
            var answered = questions.Where(q => q.isAnswered)
                                    .OrderBy(x => Random.value)
                                    .Take(needed)
                                    .ToList();
            unanswered.AddRange(answered);
        }

        // Shuffle and take exactly 30
        sessionQuestions = unanswered.OrderBy(x => Random.value)
                                     .Take(SESSION_QUESTION_LIMIT)
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
    private void LoadQuizData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);
            QuizData data = JsonUtility.FromJson<QuizData>(jsonString);
            questions = data.questions;
        }
        else
        {
            Debug.LogError("JSON file not found at " + path);
        }
    }

    private void ShowNextQuestion()
    {
        if (sessionQuestionIndex >= SESSION_QUESTION_LIMIT)
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
        questionNumberText.text = $"{sessionQuestionIndex + 1}/{SESSION_QUESTION_LIMIT}";

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
            int index = i;
            choiceButtons[i].GetComponentInChildren<TMP_Text>().text = choices[i];
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choices[index]));
        }
    }

    private void OnChoiceSelected(string selectedChoice)
    {
        StartCoroutine(HandleFeedback(selectedChoice));
    }

    private IEnumerator HandleFeedback(string selectedChoice)
    {
        feedbackPanel.SetActive(true);

        if (selectedChoice == currentQuestion.correctAnswer)
        {
            feedbackText.text = "Correct!";
            currentQuestion.isAnswered = true;
            currentQuestion.isEncountered = true;
            currentScore++;
            PlayerPrefs.SetInt("currentscore", currentScore);
            UpdateEncounteredProgress();
        }
        else
        {
            feedbackText.text = "Wrong!";
            currentQuestion.isAnswered = true;
        }

        SaveQuizData();
        yield return new WaitForSeconds(1.5f);
        feedbackPanel.SetActive(false);

        sessionQuestionIndex++; // go to next question
        ShowNextQuestion();
    }
    #endregion

    #region End Quiz
    private void EndQuiz()
    {
        quizPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        UpdateEncounteredProgress();

        int encounteredCount = sessionQuestions.Count;
        PlayerPrefs.SetInt("SpanishProgress", encounteredCount);

        int highscore = PlayerPrefs.GetInt("highscore", 0);
        if (currentScore > highscore)
        {
            PlayerPrefs.SetInt("highscore", currentScore);
        }

        if (currentScore >= 27)
        {
            PlayerPrefs.SetInt("isAmericaQuizUnlocked", 1);
            motivationalText.text = "Great job! You unlocked the QuizAmerica scene!";
        }
        else
        {
            motivationalText.text = "Keep trying! You can do better!";
        }

        finalScoreText.text = $"Score: {currentScore}/{SESSION_QUESTION_LIMIT}";
        highscoreText.text = $"Highscore: {PlayerPrefs.GetInt("highscore", 0)}";
        PlayerPrefs.Save();
    }

    public void NewQuizSession()
    {
        foreach (var q in questions) q.isAnswered = false;
        currentScore = 0;
        PlayerPrefs.SetInt("currentscore", 0);
        PlayerPrefs.Save();

        BuildSessionPool();

        gameOverPanel.SetActive(false);
        StartCoroutine(StartCountdown());
    }
    #endregion

    #region JSON Save
    private void SaveQuizData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        QuizData data = new QuizData { questions = questions };
        string jsonString = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, jsonString);
    }

    private void UpdateEncounteredProgress()
    {
        int encounteredCount = questions.Count(q => q.isEncountered);
        PlayerPrefs.SetInt("EncounteredCount", encounteredCount);
        PlayerPrefs.SetInt("TotalQuestions", questions.Count);
        PlayerPrefs.Save();
    }
    #endregion
}
