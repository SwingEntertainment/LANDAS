using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public AudioClip PaloSeboTheme;

    void Awake()
    {
        if (AudioManager.Instance != null && PaloSeboTheme != null)
        AudioManager.Instance.PlayMusic(PaloSeboTheme, loop: true);

        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

}
