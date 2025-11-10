using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AudioSave : MonoBehaviour
{
    [Header("Sound Effect")]
    public AudioClip clickSound;

    private AudioSource lastPlayedSource;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    public void PlayClickSound()
    {
        if (AudioManager.Instance != null && clickSound != null)
        {
            var sfxSource = AudioManager.Instance.sfxSource;

            if (sfxSource.isPlaying && sfxSource.clip == clickSound)
                sfxSource.Stop();

            sfxSource.clip = clickSound;
            sfxSource.Play();
        }
    }
}
