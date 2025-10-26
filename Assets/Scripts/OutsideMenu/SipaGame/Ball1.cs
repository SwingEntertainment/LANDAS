using UnityEngine;

public class DragPlayerLeftRight : MonoBehaviour
{
    public float screenMargin = 0.5f; // Distance from left/right edge in world units

    private bool isDragging = false;
    private Vector3 offset;
    private float fixedY;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        fixedY = transform.position.y;
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorld.x, 0, 0); // Only offset X
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (!isDragging) return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float targetX = mouseWorld.x + offset.x;

        // Clamp to screen bounds
        float halfScreenWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float minX = -halfScreenWidth + screenMargin;
        float maxX = halfScreenWidth - screenMargin;

        targetX = Mathf.Clamp(targetX, minX, maxX);

        // Set new position: only X moves, Y and Z stay fixed
        transform.position = new Vector3(targetX, fixedY, transform.position.z);
    }
}
