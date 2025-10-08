using UnityEngine;
using TMPro;

public class PulsingText : MonoBehaviour
{
    [Header("Pulse Settings")]
    public TMP_Text targetText;           
    public float pulseSpeed = 2f;        
    public float minScale = 0.9f;         
    public float maxScale = 1.1f;         

    private Vector3 originalScale;

    void Start()
    {
        if (targetText != null)
        {
            originalScale = targetText.rectTransform.localScale;
        }
    }

    void Update()
    {
        if (targetText != null)
        {
            float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            targetText.rectTransform.localScale = originalScale * scale;
        }
    }
}
