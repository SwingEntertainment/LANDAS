using UnityEngine;
using UnityEngine.UI;

public class OutsideTutorialUI : MonoBehaviour
{
    public GameObject[] pages;       
    public Button nextButton;

    private int currentPage = 0;
    private OutsideMenu outsideMenu;

    void Start()
    {
        outsideMenu = FindObjectOfType<OutsideMenu>();

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
                if (outsideMenu != null)
                    outsideMenu.TutorialFinished();
            }
        }
        UpdateButtons();
    }

    void UpdateButtons()
    {
        nextButton.interactable = true;
    }
}
