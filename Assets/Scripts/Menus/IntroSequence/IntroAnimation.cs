using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAnimation : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
