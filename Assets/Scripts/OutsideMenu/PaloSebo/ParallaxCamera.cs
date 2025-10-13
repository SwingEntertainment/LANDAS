using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    private float oldYPosition;   // ✅ consistent name

    void Start()
    {
        oldYPosition = transform.position.y;
    }

    void Update()
    {
        // Temporary auto-scroll upwards
        transform.position += Vector3.up * 25f * Time.deltaTime;

        float newY = transform.position.y;
        if (!Mathf.Approximately(newY, oldYPosition))
        {
            float delta = oldYPosition - newY;
            if (onCameraTranslate != null)
                onCameraTranslate(delta);

            oldYPosition = newY;
        }
    }
}
