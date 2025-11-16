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
        gameOverPanel.SetActive(false); // Hide at start
    }

    public void ShowGameOver(int finalScore)
    {
        // Show Game Over panel
        gameOverPanel.SetActive(true);

        // Display Final Score
        if (finalScoreText != null)
            finalScoreText.text = "Score: " + finalScore;

        // ⭐ High Score System
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        if (finalScore > savedHighScore)
        {
            savedHighScore = finalScore;
            PlayerPrefs.SetInt("HighScore", savedHighScore);
            PlayerPrefs.Save();
        }

        // Show High Score in UI
        if (highScoreText != null)
            highScoreText.text = "High Score: " + savedHighScore;
    }

    public void OnPlayAgain()
    {
        // Reset score
        PaloSeboScoreManager.Instance.ResetScore();

        // Reload the PaloSebo scene to reset enemies, flags, and player
        Time.timeScale = 1f; // Make sure game time is running
        SceneManager.LoadScene("PaloSebo");
    }

    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("OutsideMenu"); // Assuming this is main menu
    }
}
