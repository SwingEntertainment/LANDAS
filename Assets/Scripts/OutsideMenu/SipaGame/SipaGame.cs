using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SipaGame : MonoBehaviour
{
    // Button function to go back to main menu
    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("OutsideMenu");
    }
}


public class Ball : MonoBehaviour
{
    private float fallSpeed;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetRandomSpeed();
    }

    void FixedUpdate()
    {
        // Move ball downward at chosen speed
        rb.linearVelocity = new Vector2(0, -fallSpeed);
    }

    void SetRandomSpeed()
    {
        // Pick between 3 speeds
        int rand = Random.Range(0, 3); // 0, 1, or 2

        if (rand == 0) fallSpeed = 3f;   // slow
        else if (rand == 1) fallSpeed = 5f; // medium
        else fallSpeed = 7f; // fast
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            // Respawn ball at top of the screen
            float topY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y + 1f;
            float randomX = Random.Range(-7f, 7f);

            transform.position = new Vector2(randomX, topY);
            SetRandomSpeed();
        }
    }
}
