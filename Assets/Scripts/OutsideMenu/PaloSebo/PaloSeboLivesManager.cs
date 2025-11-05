using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaloSeboLivesManager : MonoBehaviour
{
    public static PaloSeboLivesManager Instance;

    [Header("❤️ UI References")]
    public Image lifeIcon;         // your head icon
    public TextMeshProUGUI lifeText; // the "×3" text (or regular Text if not TMP)

    private int currentLives = 3;

    void Awake()
    {
        Instance = this;
        UpdateLivesUI();
    }

    public void LoseLife()
    {
        if (currentLives <= 0) return;

        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            Debug.Log("💀 Game Over!");
            
            int finalScore = 0;
            if (PaloSeboScoreManager.Instance != null)
                finalScore = PaloSeboScoreManager.Instance.GetScore();

            if (GameOverUI.Instance != null)
                GameOverUI.Instance.ShowGameOver(finalScore);
        }

    }

    private void UpdateLivesUI()
    {
        if (lifeText != null)
            lifeText.text = "×" + currentLives;
    }

    public void ResetLives()
    {
        currentLives = 3;
        UpdateLivesUI();
    }
}
