using UnityEngine;
using UnityEngine.SceneManagement;

public class SkippingLogic : MonoBehaviour
{
    public string nextScene = "GameMenu";

    public void OnStoryFinished()
    {
        MainMenu.SetStoryPlayed();
        SceneManager.LoadScene(nextScene);
    }
}
