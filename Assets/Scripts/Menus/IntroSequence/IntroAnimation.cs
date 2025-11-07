using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAnimation : MonoBehaviour
{
    public void LoadNextScene()
    {
        LoadingScene.LoadSceneWithLoading("MainMenu");
    }
}
