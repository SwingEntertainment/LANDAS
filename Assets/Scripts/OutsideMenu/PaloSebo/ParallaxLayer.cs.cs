using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("Controls how much this layer moves relative to the camera. Lower = slower (farther), higher = faster (closer).")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    private float spriteHeight;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;

        // Get the sprite height in world units
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            spriteHeight = sr.bounds.size.y;
    }

    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.y -= delta * parallaxFactor;
        transform.localPosition = newPos;

        // Infinite tiling upwards
        if (cam != null)
        {
            float camY = cam.position.y;
            float diff = camY - transform.position.y;

            // If camera has gone too far up, reposition the sprite one "tile" higher
            if (diff > spriteHeight)
            {
                transform.position += new Vector3(0f, spriteHeight * 2f, 0f);
            }
        }
    }
}
