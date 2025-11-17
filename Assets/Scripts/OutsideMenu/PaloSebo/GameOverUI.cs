using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    [Header("🎯 UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    void Awake()
    {
        Instance = this;
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(int finalScore)
    {
        gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + finalScore;

        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        if (finalScore > savedHighScore)
        {
            savedHighScore = finalScore;
            PlayerPrefs.SetInt("HighScore", savedHighScore);
            PlayerPrefs.Save();
        }

        if (highScoreText != null)
            highScoreText.text = "High Score: " + savedHighScore;
    }

    public void OnBack()
    {
        PaloSeboGameManager.StopGame();
        if (PaloSeboScoreManager.Instance != null)
            PaloSeboScoreManager.Instance.ResetScore();

        if (PaloSeboLivesManager.Instance != null)
            PaloSeboLivesManager.Instance.ResetLives();
        SceneManager.LoadScene("PaloSebo");
    }

    public void OnRestart()
    {
        PaloSeboGameManager.StartGame();
        Time.timeScale = 1f;
        if (PaloSeboScoreManager.Instance != null)
            PaloSeboScoreManager.Instance.ResetScore();

        if (PaloSeboLivesManager.Instance != null)
            PaloSeboLivesManager.Instance.ResetLives();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

    }
}
