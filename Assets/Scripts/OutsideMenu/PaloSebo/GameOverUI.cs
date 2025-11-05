using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    [Header("🎯 UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    void Awake()
    {
        Instance = this;
        gameOverPanel.SetActive(false); // Hide at start
    }

    public void ShowGameOver(int finalScore)
    {
        gameOverPanel.SetActive(true);
        if (finalScoreText != null)
            finalScoreText.text = "Score: " + finalScore;
    }

    public void OnPlayAgain()
    {
        // Reload the PaloSebo scene (replace "PaloSebo" with your actual scene name)
        SceneManager.LoadScene("PaloSebo");
    }

    public void OnMainMenu()
    {
        // Load the OutsideMenu scene
        SceneManager.LoadScene("OutsideMenu");
    }
}
