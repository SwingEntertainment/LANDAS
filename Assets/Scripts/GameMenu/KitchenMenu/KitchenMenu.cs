using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string gameMenuScene = "GameMenu";

    public void GoToGameMenu()
    {
        SceneManager.LoadScene(gameMenuScene);
    }
}
