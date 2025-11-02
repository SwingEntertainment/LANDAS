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
    public float firstBallSpeed = 25f; 

    [Header("Game Over Settings")]
    public GameObject gameOverPanel;
    public Animator bgAnimator;

    [HideInInspector] public GameObject currentBall;

    private Camera cam;
    private bool isFirstBall = true;

    private void Start()
    {
        cam = Camera.main;

        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();

        if (backgroundPanel == null)
            backgroundPanel = FindObjectOfType<RectTransform>();

        SpawnBall();
    }

    public void ResetFirstBallFlag()
    {
        isFirstBall = true;
    }

    public void SpawnBall()
    {
        if (!backgroundPanel || !ballPrefab) return;

        if (currentBall != null)
        {
            Destroy(currentBall);
            currentBall = null;
        }

        Vector3[] corners = new Vector3[4];
        backgroundPanel.GetWorldCorners(corners);

        float minX = corners[0].x + horizontalMargin;
        float maxX = corners[2].x - horizontalMargin;
        float spawnY = corners[1].y + 1f;

        Vector3 spawnPos = new Vector3(Random.Range(minX, maxX), spawnY, 0f);

        float fallSpeed = isFirstBall ? firstBallSpeed : Random.Range(minFallSpeed, maxFallSpeed);
        isFirstBall = false;

        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb == null) rb = currentBall.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(0f, -fallSpeed);

        Collider2D col = currentBall.GetComponent<Collider2D>();
        if (col == null)
        {
            col = currentBall.AddComponent<CircleCollider2D>();
            col.isTrigger = false;
        }

        Debug.Log($"🟢 Spawned ball with speed: {fallSpeed}");
        StartCoroutine(CheckForRespawnOrGameOver(currentBall));
    }

    private IEnumerator CheckForRespawnOrGameOver(GameObject ball)
    {
        if (cam == null) cam = Camera.main;

        while (ball != null)
        {
            Vector3 viewportPos = cam.WorldToViewportPoint(ball.transform.position);

            if (viewportPos.x <= 0f || viewportPos.x >= 1f)
            {
                Destroy(ball);
                yield return new WaitForSeconds(0.3f);
                SpawnBall();
                yield break;
            }

            if (viewportPos.y <= 0f)
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

        FeetCollider feet = FindObjectOfType<FeetCollider>();
        if (feet != null)
            feet.SetGameOver(true);

        GameObject playerObj = GameObject.Find("player");
        if (playerObj != null)
            playerObj.SetActive(false);

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

    public void ResetBall()
    {
        StopAllCoroutines();

        if (currentBall != null)
            Destroy(currentBall);

        Time.timeScale = 1f;

        isFirstBall = true;

        SpawnBall();
    }
}
