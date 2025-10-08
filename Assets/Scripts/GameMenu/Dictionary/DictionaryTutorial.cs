using UnityEngine;
using UnityEngine.UI;

public class DictionaryTutorialUI : MonoBehaviour
{
    public GameObject[] pages;       
    public Button nextButton;

    private int currentPage = 0;
    private Dictionary dictionaryMenu;

    void Start()
    {
        dictionaryMenu = FindObjectOfType<Dictionary>();

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
                if (dictionaryMenu != null)
                    dictionaryMenu.TutorialFinished();
            }
        }
        UpdateButtons();
    }

    void UpdateButtons()
    {
        nextButton.interactable = true;
    }
}
