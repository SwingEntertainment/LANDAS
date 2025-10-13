using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public GameObject[] pages;       
    public Button nextButton;

    private int currentPage = 0;
    private GameMenu gameMenu;

    void Start()
    {
        gameMenu = FindObjectOfType<GameMenu>();

        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(i == 0);

        nextButton.onClick.AddListener(NextPage);
        UpdateButtons();
    }

    void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            pages[currentPage].SetActive(false);
            currentPage++;
            pages[currentPage].SetActive(true);

            if (currentPage == pages.Length - 1)
            {
                if (gameMenu != null)
                    gameMenu.TutorialFinished();
            }
        }
        UpdateButtons();
    }

    void UpdateButtons()
    {
        nextButton.interactable = true;
    }
}
