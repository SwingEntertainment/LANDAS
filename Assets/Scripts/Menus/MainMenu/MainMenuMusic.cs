using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioClip sceneMusic;
    public bool loop = true;

    void Start()
    {
        if (AudioManager.Instance != null && sceneMusic != null)
        {
            AudioManager.Instance.PlayMusic(sceneMusic, loop);
        }
    }
}
