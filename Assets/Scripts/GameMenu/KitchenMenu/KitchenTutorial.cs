using UnityEngine;
using UnityEngine.UI;

public class KitchenTutorialUI : MonoBehaviour
{
    public GameObject[] pages; 
    public Button nextButton;

    private int currentPage = 0;
    private KitchenMenu kitchenManager; 

    void Start()
    {
        kitchenManager = FindObjectOfType<KitchenMenu>();
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == 0);
        }
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
                if (kitchenManager != null)
                {
                    kitchenManager.TutorialFinished(); 
                }
            }
        }
        UpdateButtons();
    }

    void UpdateButtons()
    {
        nextButton.interactable = true;
    }
}