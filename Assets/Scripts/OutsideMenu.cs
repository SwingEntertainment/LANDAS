using UnityEngine;
using UnityEngine.SceneManagement;

public class OutsideMenu : MonoBehaviour
{
    [Header("---------AUDIO SOURCE--------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("---------AUDIO CLIP--------")]
    public AudioClip outsideMenuBGM;
    public AudioClip buttonClick;

    private void Start()
    {
        musicSource.clip = outsideMenuBGM;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void playSFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }
    
    public void GoToSipaGame()
    {
        SceneManager.LoadSceneAsync(6);
    }
}
