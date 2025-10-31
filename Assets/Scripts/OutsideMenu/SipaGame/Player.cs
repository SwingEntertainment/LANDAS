using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DragPlayerLeftRight : MonoBehaviour
{
    public float screenMargin = 0.5f; 

    private bool isDragging = false;
    private Vector3 offset;
    private float fixedY;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        fixedY = transform.position.y;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseDrag();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchDrag();
#endif
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z)
            );
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld);
            if (hit != null && hit.transform == transform)
            {
                isDragging = true;
                offset = transform.position - new Vector3(mouseWorld.x, 0, 0);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z)
            );
            MovePlayer(mouseWorld.x + offset.x);
        }
    }

    private void HandleTouchDrag()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchWorld = mainCamera.ScreenToWorldPoint(
                new Vector3(touch.position.x, touch.position.y, -mainCamera.transform.position.z)
            );

            if (touch.phase == TouchPhase.Began)
            {
                Collider2D hit = Physics2D.OverlapPoint(touchWorld);
                if (hit != null && hit.transform == transform)
                {
                    isDragging = true;
                    offset = transform.position - new Vector3(touchWorld.x, 0, 0);
                }
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                MovePlayer(touchWorld.x + offset.x);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }
    }

    private void MovePlayer(float targetX)
    {
        float halfScreenWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float minX = -halfScreenWidth + screenMargin;
        float maxX = halfScreenWidth - screenMargin;
        targetX = Mathf.Clamp(targetX, minX, maxX);

        transform.position = new Vector3(targetX, fixedY, transform.position.z);
    }
}
