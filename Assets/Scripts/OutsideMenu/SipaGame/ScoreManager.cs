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

    private const string PREF_CURRENT_SCORE = "SipaCurrentScore";
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
        LoadScore();
        UpdateScoreUI();
        Debug.Log($"[ScoreManager] Initialized | Current Score: {currentScore}");
    }

    private void LoadScore()
    {
        currentScore = PlayerPrefs.GetInt(PREF_CURRENT_SCORE, 0);
    }

    public void AddScore(int points)
    {
        currentScore += points;

        PlayerPrefs.SetInt(PREF_CURRENT_SCORE, currentScore);

        int highScore = PlayerPrefs.GetInt(PREF_HIGH_SCORE, 0);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt(PREF_HIGH_SCORE, currentScore);
            Debug.Log($"🏆 New High Score: {currentScore}");
        }

        PlayerPrefs.Save();
        UpdateScoreUI();

        Debug.Log($"[ScoreManager] +{points} | New Score: {currentScore}");
    }

    public void ResetScore()
    {
        currentScore = 0;
        PlayerPrefs.SetInt(PREF_CURRENT_SCORE, 0);
        PlayerPrefs.Save();
        UpdateScoreUI();
        Debug.Log("[ScoreManager] Score reset to 0");
    }

    private void UpdateScoreUI()
    {
        if (currentScoreText != null)
            currentScoreText.text = currentScore.ToString();

        if (finalScoreText != null)
            finalScoreText.text = currentScore.ToString();

        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt(PREF_HIGH_SCORE, 0);
            highScoreText.text = highScore.ToString();
        }
    }

    public int GetCurrentScore() => currentScore;
    public int GetHighScore() => PlayerPrefs.GetInt(PREF_HIGH_SCORE, 0);

    private void OnEnable()
    {
        LoadScore();
        UpdateScoreUI();
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt(PREF_CURRENT_SCORE, currentScore);
        PlayerPrefs.Save();
    }
}
