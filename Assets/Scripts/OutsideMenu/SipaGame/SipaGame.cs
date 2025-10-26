using UnityEngine;
using UnityEngine.SceneManagement;

public class SipaGame : MonoBehaviour
{
    [Header("Restart References")]
    public GameObject player;           
    public GameObject gameOverPanel;    
    public Animator bgAnimator;         
    public Ball ballManager;           
    public ScoreManager scoreManager;   

    // Button function to go back to main menu
    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("OutsideMenu");
    }

    // Function to restart the game
    public void Restart()
    {
        Debug.Log("Restart function called!"); 

        // Reactivate player
        if (player != null)
            player.SetActive(true);

        // Hide Game Over panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Restart background animation
        if (bgAnimator != null)
            bgAnimator.enabled = true;

        // Reset score
        if (scoreManager != null)
            scoreManager.ResetScore();

        // Destroy current ball if exists
        if (ballManager != null && ballManager.currentBall != null)
            Destroy(ballManager.currentBall);

        // Spawn a new ball
        if (ballManager != null)
            ballManager.SpawnBall();

        // Resume the game if it was paused
        Time.timeScale = 1f;
    }
}
