using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 2f;

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // Optional: destroy after going off-screen
        if (transform.position.y < -100f)
        {
            Destroy(gameObject);
        }
    }
}
