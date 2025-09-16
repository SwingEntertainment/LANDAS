using UnityEngine;
using TMPro;
using System.Collections;

public class FadeInTexFunction : MonoBehaviour
{
    public float fadeDuration = 1f; 
public TextMeshProUGUI[] targetTexts;

    private void OnEnable()
    {
        StartCoroutine(FadeInTexts());
    }

    private IEnumerator FadeInTexts()
    {
        foreach (TextMeshProUGUI txt in targetTexts)
        {
            if (txt != null)
            {
                Color c = txt.color;
                c.a = 0f;
                txt.color = c;
            }
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);

            foreach (TextMeshProUGUI txt in targetTexts)
            {
                if (txt != null)
                {
                    Color c = txt.color;
                    c.a = alpha;
                    txt.color = c;
                }
            }

            yield return null;
        }
    }
}
