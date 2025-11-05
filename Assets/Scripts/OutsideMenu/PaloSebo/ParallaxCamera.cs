using UnityEngine;

public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float delta);
    public event ParallaxCameraDelegate onCameraTranslate;

    [Header("Camera Movement")]
    public float scrollSpeed = 2f;

    private Vector3 previousPos;

    void Start()
    {
        previousPos = transform.position;
    }

    void Update()
    {
        transform.position += Vector3.up * scrollSpeed * Time.deltaTime;

        float delta = transform.position.y - previousPos.y;
        if (Mathf.Abs(delta) > Mathf.Epsilon)
        {
            if (onCameraTranslate != null)
                onCameraTranslate(delta);
        }

        previousPos = transform.position;
    }
}
