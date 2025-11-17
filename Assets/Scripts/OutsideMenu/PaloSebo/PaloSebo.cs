using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PaloSebo : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public AudioClip PaloSeboTheme;
    public GameObject gamePanel;
    public GameObject startPanel;

    void Start()
    {
        PaloSeboGameManager.StopGame();

        if (PaloSeboLivesManager.Instance != null)
            PaloSeboLivesManager.Instance.ResetLives();

        if (PaloSeboScoreManager.Instance != null)
            PaloSeboScoreManager.Instance.ResetScore();

        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
        if (AudioManager.Instance != null && PaloSeboTheme != null)
        {
            AudioManager.Instance.PlayMusic(PaloSeboTheme, loop: true);
        }
    }

    public void OnPressStart()
    {
        PaloSeboGameManager.StartGame();

        LanggamAndFlagSpawner spawner = FindObjectOfType<LanggamAndFlagSpawner>();
        if (spawner != null)
            spawner.ResetSpawner();


        gamePanel.SetActive(true);
        startPanel.SetActive(false);
    }

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("OutsideMenu");
    }

}
