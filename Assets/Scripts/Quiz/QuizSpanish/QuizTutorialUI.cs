using UnityEngine;
using UnityEngine.UI;

public class QuizTutorialUI : MonoBehaviour
{
    public GameObject[] pages;
    public Button nextButton;

    private int currentPage = 0;
    private QuizSpanish quizManager;

    void Start()
    {
        quizManager = FindObjectOfType<QuizSpanish>();
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
            if (quizManager != null)
            {
                quizManager.TutorialQuizFinished(); 
            }        
        }
        UpdateButtons();
    }

    void UpdateButtons()
    {
        nextButton.interactable = true;
    }
}