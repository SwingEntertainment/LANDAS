using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AudioSave : MonoBehaviour
{
    [Header("Sound Effect")]
    public AudioClip clickSound;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (AudioManager.Instance != null && clickSound != null)
        {
            AudioManager.Instance.PlaySFX(clickSound);
        }
    }
}
