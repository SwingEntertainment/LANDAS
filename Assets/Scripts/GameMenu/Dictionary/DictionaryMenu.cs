using UnityEngine;
using UnityEngine.SceneManagement;

public class DictionaryMenu : MonoBehaviour
{
    [Header("---------AUDIO SOURCE--------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("---------AUDIO CLIP--------")]
    public AudioClip DictionaryMenuBGM;
    public AudioClip buttonClick;

    private void Start()
    {
        musicSource.clip = DictionaryMenuBGM;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void playSFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(3);
    }
}
