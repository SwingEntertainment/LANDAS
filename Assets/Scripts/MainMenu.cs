using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    [Header("---------AUDIO SOURCE--------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("---------AUDIO CLIP--------")]
    public AudioClip mainMenuBGM;
    public AudioClip buttonClick;

    private void Start()
    {
        musicSource.clip = mainMenuBGM;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void playSFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
