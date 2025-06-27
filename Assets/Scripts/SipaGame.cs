using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SipaGame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gamePanel;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text highScoreText;
    public GameObject gameOverPanel;

    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public float minSpeed = 5f;
    public float maxSpeed = 9f;

    private int score = 0;
    private int highScore = 0;
    private float timer = 300f;
    private bool isGameRunning = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
        scoreText.text = "Score: 0";
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;
    }

    void Update()
    {
        if (!isGameRunning) return;

        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";

        if (timer <= 0f)
        {
            GameOver();
        }
    }

    public void StartGame()
    {
        if (gamePanel.activeSelf)
        {
            isGameRunning = true;
            score = 0;
            scoreText.text = "Score: 0";
            timer = 300f;
            CancelInvoke();
            InvokeRepeating("SpawnBall", 1f, spawnInterval);
        }
    }

    void SpawnBall()
    {
        if (!isGameRunning) return;

        Vector3 spawnPosition = new Vector3(Random.Range(-3f, 3f), spawnPoint.position.y, 0);
        GameObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        BallFall bf = ball.AddComponent<BallFall>();
        bf.SetSpeed(Random.Range(minSpeed, maxSpeed));
        bf.gameManager = this;
    }

    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        if (!isGameRunning) return;

        isGameRunning = false;
        CancelInvoke("SpawnBall");
        gameOverPanel.SetActive(true);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        highScoreText.text = "High Score: " + highScore;
    }

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("OutsideMenu");
    }
}
