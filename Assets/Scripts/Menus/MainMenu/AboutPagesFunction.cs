using UnityEngine;
using UnityEngine.UI;

public class AboutPagesFunction : MonoBehaviour
{
    [SerializeField] private GameObject[] aboutPages;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

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
        if (currentPage < aboutPages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    private void ShowPage(int index)
    {
        for (int i = 0; i < aboutPages.Length; i++)
            aboutPages[i].SetActive(i == index);

        prevButton.gameObject.SetActive(index > 0);

        nextButton.gameObject.SetActive(index < aboutPages.Length - 1);
    }
}
