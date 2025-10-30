using UnityEngine;
using UnityEngine.SceneManagement;

public class SpanishMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string gameMenuScene = "GameMenu";

    [Header("Audio Clips")]
    public AudioClip defaultTheme;

    void Start()
    {
        if (AudioManager.Instance != null && defaultTheme != null)
            AudioManager.Instance.PlayMusic(defaultTheme, loop: true);
    }
    
    public void GoToGameMenu()
    {
        SceneManager.LoadScene(gameMenuScene);
    }
}
