using UnityEngine;
using TMPro;

public class PaloSeboScoreManager : MonoBehaviour
{
    public static PaloSeboScoreManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;

    [Header("Game Over Settings")]
    public int maxScore = 999;  // optional, if you want a limit

    private int score = 0;
    private bool isGameOver = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    /// <summary>
    /// Add points to score
    /// </summary>
    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;

        // Optional: clamp maximum score
        if (score > maxScore)
            score = maxScore;

        UpdateScoreUI();
    }

    /// <summary>
    /// Subtract points from score
    /// </summary>
    public void SubtractScore(int amount)
    {
        if (isGameOver) return;

        score -= amount;
        if (score < 0) score = 0;

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    /// <summary>
    /// Call this to get the current score
    /// </summary>
    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
        isGameOver = false;
        UpdateScoreUI();
    }


    /// <summary>
    /// Trigger Game Over
    /// </summary>
    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Show Game Over UI with final score
        GameOverUI.Instance.ShowGameOver(score);

        // Optional: pause the game
        Time.timeScale = 0f;
    }
}
