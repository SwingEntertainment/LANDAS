using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using TMPro;
using System.Collections;

public class SipaGame : MonoBehaviour
{
    [Header("Restart References")]
    public GameObject player;
    public GameObject gameOverPanel;
    public Animator bgAnimator;
    public PlayableDirector BgTimeline;
    public Ball ballManager;
    public ScoreManager scoreManager;
    public SpriteRenderer playerSprite;
    public Sprite defaultPlayerSprite;

    [Header("Countdown UI")]
    public GameObject countdownPanel;
    public TMP_Text countdownText;

    [Header("Countdown Settings")]
    public int countdownStart = 3;

    [Header("Menu Panels (For Menu Music)")]
    public GameObject mainPanel;
    public GameObject tutorialPanel;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;
    public AudioClip countdownClip;

    [Header("Audio Source for SFX")]
    public AudioSource sfxSource;

    [Header("Static Game Background")]
    public GameObject staticGameBG;

    [Header("UI Sound Effects")]
    public AudioClip buttonClickClip;

    private bool isCountingDown = false;
    private bool isGameOver = false;
    private bool isGameStarted = false;
    private Vector3 playerStartPos;
    private bool menuMusicPlaying = false;

    private void Start()
    {
        if (player != null)
            playerStartPos = player.transform.position;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (AudioManager.Instance != null && menuMusic != null)
        {
            if ((mainPanel != null && mainPanel.activeSelf) ||
                (tutorialPanel != null && tutorialPanel.activeSelf))
            {
                AudioManager.Instance.StopMusic();
                AudioManager.Instance.PlayMusic(menuMusic, true);
                Debug.Log("Menu music started (Main/Tutorial panel active).");
            }
        }

        ShowStaticAndResetBG();
    }

    private void Update()
    {
        if (AudioManager.Instance != null && menuMusic != null)
        {
            bool isMenuActive = (mainPanel != null && mainPanel.activeSelf) ||
                                (tutorialPanel != null && tutorialPanel.activeSelf);

            if (isMenuActive && !isGameStarted && !isCountingDown && !menuMusicPlaying)
            {
                AudioManager.Instance.StopMusic();
                AudioManager.Instance.PlayMusic(menuMusic, true);
                menuMusicPlaying = true;
                Debug.Log("Menu music started in Update() without repeating.");
            }
        }
    }

    public void BackToMenu()
    {
        if (AudioManager.Instance != null && menuMusic != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMusic(menuMusic, true);
            Debug.Log("Menu music played from BackToMenu()");
        }

        isGameStarted = false;
        isGameOver = false;
        SceneManager.LoadSceneAsync("OutsideMenu");
    }

    public void Restart()
    {
        if (isCountingDown) return;
        StartCoroutine(RestartSequence());
    }

    private IEnumerator RestartSequence()
    {
        isGameOver = false;
        isGameStarted = true;
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();

        if (player != null)
            player.SetActive(false);

        if (ballManager != null && ballManager.currentBall != null)
            Destroy(ballManager.currentBall);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        yield return new WaitForEndOfFrame();
        RebindScoreManagerUI();

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
            ScoreManager.Instance.UpdateUI(); 
            Debug.Log("Score reset and UI updated at restart.");
        }

        if (countdownPanel != null)
            countdownPanel.SetActive(true);

        if (sfxSource != null && countdownClip != null)
            sfxSource.PlayOneShot(countdownClip);

        yield return StartCoroutine(CountdownRoutineRealtime());

        if (bgAnimator != null)
        {
            bgAnimator.enabled = false;
            yield return null;
            bgAnimator.enabled = true;
            bgAnimator.Rebind();
            bgAnimator.Update(0f);
            bgAnimator.Play("Bg", 0, 0f);
            Debug.Log("Background animator fully reset to start.");
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

        if (AudioManager.Instance != null && gameMusic != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMusic(gameMusic, true);
            Debug.Log("Game music started after countdown.");
        }
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

    public void OnGameOver()
    {
        if (!isGameStarted)
        {
            Debug.Log("Ignored Game Over — game hasn’t started yet.");
            return;
        }

        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Game Over triggered.");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            if (gameOverMusic != null)
            {
                AudioManager.Instance.PlayMusic(gameOverMusic, false);
                Debug.Log("Game Over music started.");
            }
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void PlayMainMenuMusic()
    {
        if (AudioManager.Instance != null && menuMusic != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMusic(menuMusic, true);
            Debug.Log("Main Menu music started via helper function.");
        }
    }

    public void ShowStaticAndResetBG()
    {
        if (staticGameBG != null)
            staticGameBG.SetActive(true);

        if (bgAnimator != null)
        {
            bgAnimator.enabled = false;
            bgAnimator.enabled = true;
            bgAnimator.Rebind();
            bgAnimator.Update(0f);
            bgAnimator.Play("Bg", 0, 0f);
        }

        if (BgTimeline != null)
        {
            BgTimeline.gameObject.SetActive(true);
            BgTimeline.time = 0;
            BgTimeline.Evaluate();
            BgTimeline.Play();
        }

        if (staticGameBG != null)
            staticGameBG.SetActive(false);

        Debug.Log("Static BG shown and BG Animator/Timeline reset in one function.");
    }

    public void ResetBG()
    {
        if (bgAnimator != null)
        {
            bgAnimator.enabled = false;
            bgAnimator.enabled = true;
            bgAnimator.Rebind();
            bgAnimator.Update(0f);
            bgAnimator.Play("Bg", 0, 0f);
            Debug.Log("BG Animator reset.");
        }

        if (BgTimeline != null)
        {
            BgTimeline.gameObject.SetActive(true);
            BgTimeline.time = 0;
            BgTimeline.Evaluate();
            BgTimeline.Play();
            Debug.Log("BG Timeline reset and playing.");
        }
    }

    public void PlayButtonClick()
    {
        if (sfxSource != null && buttonClickClip != null)
        {
            sfxSource.PlayOneShot(buttonClickClip);
            Debug.Log("Button click sound played.");
        }
    }

    private void RebindScoreManagerUI()
    {
        if (ScoreManager.Instance == null) return;

        ScoreManager.Instance.currentScoreText =
            GameObject.FindWithTag("CurrentScoreText")?.GetComponent<TMP_Text>();
        ScoreManager.Instance.gameOverHighScoreText =
            GameObject.FindWithTag("GameOverHighScoreText")?.GetComponent<TMP_Text>();
        ScoreManager.Instance.mainMenuHighScoreText =
            GameObject.FindWithTag("MainMenuHighScoreText")?.GetComponent<TMP_Text>();

        ScoreManager.Instance.UpdateUI(); 
    }
}
