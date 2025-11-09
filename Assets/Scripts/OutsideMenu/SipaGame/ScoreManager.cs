using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Elements")]
    public TMP_Text currentScoreText;
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;

    private int currentScore = 0;

    private const string PREF_HIGH_SCORE = "SipaHighScore";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetScore();    
        UpdateHighScoreUI();
        Debug.Log($"[ScoreManager] Initialized | Current Score: {currentScore} | High Score: {GetHighScore()}");
    }

    public void AddScore(int points)
    {
        currentScore += points;

        int highScore = GetHighScore();
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt(PREF_HIGH_SCORE, currentScore);
            PlayerPrefs.Save();
            Debug.Log($"🏆 New High Score: {currentScore}");
        }

        UpdateScoreUI();
        Debug.Log($"[ScoreManager] +{points} | New Score: {currentScore}");
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
        Debug.Log("[ScoreManager] Current score reset to 0");
    }

    private void UpdateScoreUI()
    {
        if (currentScoreText != null)
            currentScoreText.text = currentScore.ToString();

        if (finalScoreText != null)
            finalScoreText.text = currentScore.ToString();

        UpdateHighScoreUI();
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            int highScore = GetHighScore();
            highScoreText.text = highScore.ToString();
        }
    }

    public int GetCurrentScore() => currentScore;

    public int GetHighScore() => PlayerPrefs.GetInt(PREF_HIGH_SCORE, 0);

    private void OnEnable()
    {
        UpdateHighScoreUI();
    }

    private void OnDisable()
    {
        int highScore = GetHighScore();
        PlayerPrefs.SetInt(PREF_HIGH_SCORE, highScore);
        PlayerPrefs.Save();
    }
}
