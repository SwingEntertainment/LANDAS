using UnityEngine;
using UnityEngine.UI;

public class AmericanQuizOpen : MonoBehaviour
{
    public Button nextChapterButton;

    private void Start()
    {
        int unlocked = PlayerPrefs.GetInt("isAmericaQuizUnlocked", 0);

        nextChapterButton.interactable = (unlocked == 1);
    }
}
