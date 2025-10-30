using System.Collections;
using UnityEngine;
using TMPro; 

public class Ball : MonoBehaviour
{
    [Header("Setup")]
    public GameObject ballPrefab;
    public RectTransform backgroundPanel;
    public ScoreManager scoreManager;
    public TMP_Text highScoreText; 

    [Header("Fall Settings")]
    public float minFallSpeed = 10f;
    public float maxFallSpeed = 15f;
    public float horizontalMargin = 0.5f;

    [Header("Game Over Settings")]
    public GameObject gameOverPanel;   
    public Animator bgAnimator;        

    [HideInInspector] public GameObject currentBall; 

    private void Start()
    {
        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager == null)
                Debug.LogError("ScoreManager not found in scene!");
        }

        if (backgroundPanel == null)
            backgroundPanel = FindObjectOfType<RectTransform>();

        SpawnBall();
    }

    public void SpawnBall()
    {
        if (!backgroundPanel || !ballPrefab) return;

        Vector3[] corners = new Vector3[4];
        backgroundPanel.GetWorldCorners(corners);

        float minX = corners[0].x + horizontalMargin;
        float maxX = corners[2].x - horizontalMargin;
        float spawnY = corners[1].y + 1f;

        Vector3 spawnPos = new Vector3(Random.Range(minX, maxX), spawnY, 0f);
        float fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);

        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb == null) rb = currentBall.AddComponent<Rigidbody2D>();

        rb.gravityScale = 1f;
        rb.linearVelocity = new Vector2(0f, -fallSpeed);

        Collider2D col = currentBall.GetComponent<Collider2D>();
        if (col == null)
        {
            col = currentBall.AddComponent<CircleCollider2D>();
            col.isTrigger = false;
        }

        StartCoroutine(CheckForGameOver(currentBall));
    }

    private IEnumerator CheckForGameOver(GameObject ball)
    {
        Camera cam = Camera.main;
        while (ball != null)
        {
            float bottomY = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

            if (ball.transform.position.y <= bottomY)
            {
                TriggerGameOver(ball);
                yield break;
            }

            yield return null;
        }
    }

    private void TriggerGameOver(GameObject ball)
    {
        Debug.Log("💀 Game Over triggered!");

        if (bgAnimator != null)
            bgAnimator.enabled = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling(); 
        }

        GameObject playerObj = GameObject.Find("player");
        if (playerObj != null)
        {
            playerObj.SetActive(false);
        }

        if (scoreManager != null)
        {
            int currentScore = scoreManager.GetCurrentScore();
            int highScore = scoreManager.GetHighScore();

            if (currentScore > highScore)
                PlayerPrefs.SetInt("SipaHighScore", currentScore);

            scoreManager.ResetScore();
        }

        if (highScoreText != null)
        {
            int savedHighScore = PlayerPrefs.GetInt("SipaHighScore", 0);
            highScoreText.text = "High Score: " + savedHighScore;
        }

        if (ball != null)
            Destroy(ball);

        Time.timeScale = 0f;
    }
}
