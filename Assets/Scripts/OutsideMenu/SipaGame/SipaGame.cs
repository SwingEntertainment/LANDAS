using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

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

    [Header("Countdown UI")]
    public GameObject countdownPanel;
    public TMP_Text countdownText;

    [Header("Countdown Settings")]
    public int countdownStart = 3;

    private bool isCountingDown = false;
    private Vector3 playerStartPos;

    private void Start()
    {
        if (player != null)
            playerStartPos = player.transform.position;
    }

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("OutsideMenu");
    }

    public void Restart()
    {
        Debug.Log("Restart function called!");
        StartCoroutine(RestartSequence());
    }

    private IEnumerator RestartSequence()
    {
        Time.timeScale = 1f;

        if (player != null)
            player.SetActive(false);

        if (ballManager != null && ballManager.currentBall != null)
            Destroy(ballManager.currentBall);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (scoreManager != null)
            scoreManager.ResetScore();

        if (countdownPanel != null)
            countdownPanel.SetActive(true);

        yield return StartCoroutine(CountdownRoutineRealtime());

        if (bgAnimator != null)
        {
            bgAnimator.enabled = false;
            yield return null; 
            bgAnimator.enabled = true;

            bgAnimator.Rebind();
            bgAnimator.Update(0f);
            bgAnimator.Play("Bg", 0, 0f); 
            Debug.Log("✅ Background animator fully reset to start.");
        }

        if (player != null)
        {
            player.SetActive(true);
            player.transform.position = playerStartPos; 

            if (playerSprite != null && defaultPlayerSprite != null)
                playerSprite.sprite = defaultPlayerSprite;

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

        if (ballManager != null)
        {
            ballManager.ResetFirstBallFlag();
            ballManager.SpawnBall();
        }

        Debug.Log("✅ Game fully restarted with background and player reset!");
    }

    private IEnumerator CountdownRoutineRealtime()
    {
        if (isCountingDown) yield break;
        isCountingDown = true;

        if (countdownPanel != null)
            countdownPanel.SetActive(true);

        int count = countdownStart;
        while (count > 0)
        {
            if (countdownText != null)
                countdownText.text = count.ToString();
            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        if (countdownText != null)
            countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);

        if (countdownPanel != null)
            countdownPanel.SetActive(false);

        isCountingDown = false;
    }
}
