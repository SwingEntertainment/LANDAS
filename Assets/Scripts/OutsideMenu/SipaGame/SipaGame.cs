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
    public SpriteRenderer playerSprite;   
    public Sprite defaultPlayerSprite;    

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("OutsideMenu");
    }

    public void Restart()
    {
        Debug.Log("Restart function called!");

        if (player != null)
        {
            player.SetActive(true);

            if (playerSprite != null && defaultPlayerSprite != null)
            {
                playerSprite.sprite = defaultPlayerSprite;
            }

            var kickAnim = player.GetComponent<PlayerKickAnimation>();
            if (kickAnim != null)
                kickAnim.ResetAnimation();

            Animator playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.Rebind();  
                playerAnimator.Update(0f); 
            }
        }

        FeetCollider feet = FindObjectOfType<FeetCollider>();
        if (feet != null)
            feet.SetGameOver(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (bgAnimator != null)
            bgAnimator.enabled = true;

        if (scoreManager != null)
            scoreManager.ResetScore();

        if (ballManager != null && ballManager.currentBall != null)
            Destroy(ballManager.currentBall);

        if (ballManager != null)
            ballManager.SpawnBall();

        Time.timeScale = 1f;

        Debug.Log("✅ Game restarted successfully!");
    }
}
