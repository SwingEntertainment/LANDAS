using UnityEngine;
using TMPro;

public class SpanishProgress : MonoBehaviour
{
    public TMP_Text progressText;

    private void Start()
    {
        int encountered = PlayerPrefs.GetInt("EncounteredCount", 0);
        int total = PlayerPrefs.GetInt("TotalQuestions", 60);
        progressText.text = $"{encountered} out of {total}";
    }
}
