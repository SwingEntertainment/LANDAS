using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CloudLoopUI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 100f;           
    public float leftOffscreenMargin = 300f;   
    public float rightOffscreenMargin = -200f;

    private RectTransform rectTransform;
    private RectTransform parentRect;
    private float panelWidth;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRect = rectTransform.parent as RectTransform;
        UpdatePanelWidth();
    }

    void Update()
    {
        UpdatePanelWidth();

        rectTransform.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        if (rectTransform.anchoredPosition.x + rectTransform.rect.width / 2f < -leftOffscreenMargin)
        {
            float newX = panelWidth + rightOffscreenMargin + rectTransform.rect.width / 2f;
            rectTransform.anchoredPosition = new Vector2(newX, rectTransform.anchoredPosition.y);
        }
    }

    void UpdatePanelWidth()
    {
        if (parentRect != null)
            panelWidth = parentRect.rect.width;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (parentRect == null)
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null) parentRect = rt.parent as RectTransform;
        }
        if (parentRect == null) return;

        float width = parentRect.rect.width;
        Vector3 pos = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector3(parentRect.position.x - width / 2f - leftOffscreenMargin, pos.y - 50, 0),
            new Vector3(parentRect.position.x - width / 2f - leftOffscreenMargin, pos.y + 50, 0)
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            new Vector3(parentRect.position.x + width / 2f + rightOffscreenMargin, pos.y - 50, 0),
            new Vector3(parentRect.position.x + width / 2f + rightOffscreenMargin, pos.y + 50, 0)
        );
    }
#endif
}
