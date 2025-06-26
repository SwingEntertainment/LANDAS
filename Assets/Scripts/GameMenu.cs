using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Header("---------AUDIO SOURCE--------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("---------AUDIO CLIP--------")]
    public AudioClip gameMenuBGM;
    public AudioClip buttonClick;

    private void Start()
    {
        musicSource.clip = gameMenuBGM;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void playSFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void GoToOutsideMenu()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void GoToLolaMenu()
    {
        SceneManager.LoadSceneAsync(4);
    }

    public void GoToDictionaryMenu()
    {
        SceneManager.LoadSceneAsync(5);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
