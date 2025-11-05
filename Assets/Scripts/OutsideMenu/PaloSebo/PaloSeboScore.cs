using UnityEngine;
using TMPro;

public class PaloSeboScoreManager : MonoBehaviour
{
    public static PaloSeboScoreManager Instance;

    public TextMeshProUGUI scoreText;
    private int score = 0;

    void Awake()
    {
        // Singleton pattern (so you can easily access PaloSeboScoreManager.Instance)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    public void SubtractScore(int amount)
    {
        score -= amount;
        if (score < 0) score = 0; // prevent negative
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    public int GetScore()
    {
        return score;
    }
}
