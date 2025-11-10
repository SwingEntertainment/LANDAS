using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Elements (assign in inspector if possible)")]
    public TMP_Text currentScoreText;          
    public TMP_Text gameOverHighScoreText;     
    public TMP_Text mainMenuHighScoreText;     

    private int currentScore = 0;
    private int highScore = 0;
    private string prefHighScoreKey = "Global_HighScore"; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        highScore = PlayerPrefs.GetInt(prefHighScoreKey, 0);
    }

    private void Start()
    {
        InitializeUI(true);
        UpdateUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeUI(true);
        UpdateUI();
    }

    private void InitializeUI(bool includeInactive = false)
    {
        if (currentScoreText == null)
            currentScoreText = FindTMPText("CurrentScoreText", includeInactive);

        if (gameOverHighScoreText == null)
            gameOverHighScoreText = FindTMPText("GameOverHighScoreText", includeInactive);

        if (mainMenuHighScoreText == null)
            mainMenuHighScoreText = FindTMPText("MainMenuHighScoreText", includeInactive);
    }

    private TMP_Text FindTMPText(string tag, bool includeInactive)
    {
        GameObject obj = null;

        if (includeInactive)
        {
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                Transform t = root.transform.FindDeepChildWithTag(tag);
                if (t != null)
                {
                    obj = t.gameObject;
                    break;
                }
            }
        }
        else
        {
            obj = GameObject.FindWithTag(tag);
        }

        if (obj != null)
            return obj.GetComponent<TMP_Text>();

        return null;
    }

    public void AddScore(int points)
    {
        currentScore += points;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(prefHighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateUI();
    }

    public int GetCurrentScore() => currentScore;
    public int GetHighScore() => highScore;

    public void UpdateUI()
    {
        InitializeUI(true); 

        if (currentScoreText != null)
            currentScoreText.text = currentScore.ToString();

        string highScoreText = "High Score: " + highScore;

        if (gameOverHighScoreText != null)
            gameOverHighScoreText.text = highScoreText;

        if (mainMenuHighScoreText != null)
            mainMenuHighScoreText.text = highScoreText;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(prefHighScoreKey, highScore);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

public static class TransformExtensions
{
    public static Transform FindDeepChildWithTag(this Transform parent, string tag)
    {
        if (parent.CompareTag(tag))
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = child.FindDeepChildWithTag(tag);
            if (result != null)
                return result;
        }
        return null;
    }
}
