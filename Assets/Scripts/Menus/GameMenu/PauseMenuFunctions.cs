using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuUI;

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";

    [Header("Buttons to Disable During Pause")]
    public Button kitchenButton;
    public Button lolaButton;
    public Button doorButton;
    public Button bookButton;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;  
        isPaused = true;

        if (kitchenButton) kitchenButton.interactable = false;
        if (lolaButton) lolaButton.interactable = false;
        if (doorButton) doorButton.interactable = false;
        if (bookButton) bookButton.interactable = false;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;  
        isPaused = false;

        if (kitchenButton) kitchenButton.interactable = true;
        if (lolaButton) lolaButton.interactable = true;
        if (doorButton) doorButton.interactable = true;
        if (bookButton) bookButton.interactable = true;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;  
        LoadingScene.LoadSceneWithLoading(mainMenuScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
