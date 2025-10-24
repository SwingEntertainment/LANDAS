using UnityEngine;

public class MovingLeftAnimation : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 1f;             
    public float resetOffset = 20f;      
    public float extraBuffer = 20f;      
    private float cloudWidth;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            cloudWidth = sr.bounds.size.x;
        else
            cloudWidth = 5f; 
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        Vector3 screenLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, Mathf.Abs(mainCamera.transform.position.z - transform.position.z)));
        Vector3 screenRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, Mathf.Abs(mainCamera.transform.position.z - transform.position.z)));

        if (transform.position.x - (cloudWidth / 2) < screenLeft.x - extraBuffer)
        {
            transform.position = new Vector3(screenRight.x + (cloudWidth / 2) + resetOffset, transform.position.y, transform.position.z);
        }
    }
}
