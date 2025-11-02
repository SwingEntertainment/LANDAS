using UnityEngine;
using UnityEngine.SceneManagement;

public class SpanishMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string gameMenuScene = "GameMenu";

    [Header("Audio Clips")]
    public AudioClip defaultTheme;
    private const string LolaMenuActiveKey = "LolaMenuActive";

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(LolaMenuActiveKey, 0);
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            PlayerPrefs.SetInt(LolaMenuActiveKey, 0);
            PlayerPrefs.Save();
        }
    }
    
    void Start()
    {
        if (AudioManager.Instance != null && defaultTheme != null)
            AudioManager.Instance.PlayMusic(defaultTheme, loop: true);
    }

    public void GoToGameMenu()
    {
        LoadingScene.LoadSceneWithLoading(gameMenuScene);
    }
}
