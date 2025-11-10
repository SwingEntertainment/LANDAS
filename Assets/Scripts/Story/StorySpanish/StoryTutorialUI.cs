using UnityEngine;
using UnityEngine.UI;

public class StoryTutorialUI : MonoBehaviour
{
    public GameObject[] pages;
    public Button nextButton;

    private int currentPage = 0;
    private SpanishMenu spanishManager;

    void Start()
    {
        spanishManager = FindObjectOfType<SpanishMenu>();
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
        }
        else 
        {
            if (spanishManager != null)
            {
                spanishManager.TutorialStoryFinished(); 
            }        
        }
        UpdateButtons();
    }

    void UpdateButtons()
    {
        nextButton.interactable = true;
    }
}