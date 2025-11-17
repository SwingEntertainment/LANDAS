using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 2f;

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -50f)
        {
            if (CompareTag("Langgam"))
            {
                if (PaloSeboScoreManager.Instance != null)
                    PaloSeboScoreManager.Instance.AddScore(2);
            }
            Destroy(gameObject);
        }
    }
}
