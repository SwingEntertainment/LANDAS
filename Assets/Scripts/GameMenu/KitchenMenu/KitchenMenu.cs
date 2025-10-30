using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string gameMenuScene = "GameMenu";
    public string RecipeHunt = "RecipeHunt";

    [Header("Audio Clips")]
    public AudioClip defaultTheme;

    void Start()
    {
        if (AudioManager.Instance != null && defaultTheme != null)
            AudioManager.Instance.PlayMusic(defaultTheme, loop: true);
    }

    public void GoToGameMenu()
    {
        SceneManager.LoadScene(gameMenuScene);
    }
    
     public void GoToRecipeHunt()
    {
        SceneManager.LoadScene(RecipeHunt);
    }
}
