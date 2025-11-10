using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KitchenMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject tutorialUI;

    [Header("Scenes")]
    public string gameMenuScene = "GameMenu";
    public string RecipeHunt = "RecipeHunt";

    [Header("Audio Clips")]
    public AudioClip defaultTheme;


    private const string KitchenTutorialPlayedKey = "kitchenMenuTutorial";

    void Start()
    {
        if (AudioManager.Instance != null && defaultTheme != null)
            if (!AudioManager.Instance.IsMusicPlaying(defaultTheme))
            {
                AudioManager.Instance.PlayMusic(defaultTheme, loop: true);
            }

        bool tutorialPlayed = PlayerPrefs.GetInt(KitchenTutorialPlayedKey, 0) == 1;

        if (!tutorialPlayed)
        {
            if (tutorialUI != null)
            {
                tutorialUI.SetActive(true);
                CanvasGroup cg = tutorialUI.GetComponent<CanvasGroup>() ?? tutorialUI.AddComponent<CanvasGroup>();
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
        }
        else
        {
            if (tutorialUI != null)
                tutorialUI.SetActive(false);
        }
    }

    public void TutorialFinished()
    {
        PlayerPrefs.SetInt(KitchenTutorialPlayedKey, 1);
        PlayerPrefs.Save();

        if (tutorialUI != null)
            tutorialUI.SetActive(false);
    }
    
    public void GoToGameMenu()
    {
        LoadingScene.LoadSceneWithLoading(gameMenuScene);
    }

    public void GoToRecipeHunt()
    {
        SceneManager.LoadScene(RecipeHunt);
    }
}
