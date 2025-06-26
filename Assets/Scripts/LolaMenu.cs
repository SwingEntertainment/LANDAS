using UnityEngine;
using UnityEngine.SceneManagement;

public class LolaMenu : MonoBehaviour
{
    [Header("---------AUDIO SOURCE--------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("---------AUDIO CLIP--------")]
    public AudioClip LolaMenuBGM;
    public AudioClip buttonClick;

    private void Start()
    {
        musicSource.clip = LolaMenuBGM;
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
    
}
