using UnityEngine;
using TMPro;

public class SpanishProgress : MonoBehaviour
{
    public TMP_Text progressText;

    private void Start()
    {
        UpdateProgressText();
    }

    private void OnEnable()
    {
        UpdateProgressText();
    }

    private void UpdateProgressText()
    {
        int encountered = PlayerPrefs.GetInt("EncounteredCount", 0);
        int total = PlayerPrefs.GetInt("TotalQuestions", 90);

        if (progressText != null)
        {
            progressText.text = $"{encountered} out of {total}";
        }
    }
}
