using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryPagesFunction : MonoBehaviour
{
    [SerializeField] private GameObject[] storyPages;
    [SerializeField] private Button nextButton;
    [SerializeField] private string nextScene = "GameMenu";

    private int currentPage = 0;

    private void Start()
    {
        ShowPage(0);
    }

    private void OnEnable()
    {
        currentPage = 0;
        ShowPage(currentPage);
    }

    public void NextPage()
    {
        if (currentPage < storyPages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
        else
        {
            MainMenu.SetStoryPlayed();
            LoadingScene.LoadSceneWithLoading(nextScene);
        }
    }

    private void ShowPage(int index)
    {
        for (int i = 0; i < storyPages.Length; i++)
            storyPages[i].SetActive(i == index);

        nextButton.gameObject.SetActive(true);
    }
}
