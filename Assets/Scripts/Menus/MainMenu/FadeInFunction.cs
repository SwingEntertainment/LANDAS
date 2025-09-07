using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInFunction : MonoBehaviour
{
    public float fadeDuration = 1f; 
    public Image[] targetImages;

    private void OnEnable()
    {
        StartCoroutine(FadeInImages());
    }

    private IEnumerator FadeInImages()
    {
        foreach (Image img in targetImages)
        {
            if (img != null)
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);

            foreach (Image img in targetImages)
            {
                if (img != null)
                {
                    Color c = img.color;
                    c.a = alpha;
                    img.color = c;
                }
            }

            yield return null;
        }
    }
}
