using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    public void Move(float delta)
    {
        // Move the layer slightly based on the camera movement
        transform.position = new Vector3(
            transform.position.x,
            startPosition.y + delta * parallaxFactor,
            transform.position.z
        );
    }
}
