// MainMenu.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip mainMenuBGM;
    public AudioClip buttonClick;

    void Start()
    {
        if (AudioManager.Instance != null && mainMenuBGM != null)
        {
            AudioManager.Instance.PlayMusic(mainMenuBGM, loop: true);
        }
    }

    public void PlayGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(buttonClick);
        SceneManager.LoadSceneAsync(3);
    }

    public void QuitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(buttonClick);
        Application.Quit();
    }

    public void CheckForUpdates()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(buttonClick);
        Application.OpenURL("https://rzregio.github.io");
    }
}
