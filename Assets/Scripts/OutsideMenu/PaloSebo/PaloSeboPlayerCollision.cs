using UnityEngine;

public class PaloSeboPlayerCollision : MonoBehaviour
{
    private float bambooTimer = 0f;
    private float bambooInterval = 10f;
    private bool isTouchingBamboo = false;

    private PlayerMovement playerMove;
    private bool isSlowed = false;
    private float slowDuration = 2f;
    private float slowTimer = 0f;

    void Start()
    {
        playerMove = GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 🪲 Langgam Hit
        if (other.CompareTag("Langgam"))
        {
            if (PaloSeboScoreManager.Instance != null)
                PaloSeboScoreManager.Instance.SubtractScore(2);

            Debug.Log("❌ Hit Langgam (-2)");

            // Lose life when hit
            if (PaloSeboLivesManager.Instance != null)
                PaloSeboLivesManager.Instance.LoseLife();

            // 🐢 Slow effect
            if (!isSlowed)
            {
                isSlowed = true;
                slowTimer = slowDuration;
                playerMove.SetSlow(true);
            }
        }


        // 🚩 Flag Hit
        if (other.CompareTag("Flag"))
        {
            if (PaloSeboScoreManager.Instance != null)
                PaloSeboScoreManager.Instance.AddScore(50);

            Debug.Log("✅ Got Flag (+50)");
            Destroy(other.gameObject);
        }

        // 🪵 Bamboo Enter
        if (other.CompareTag("Bamboo"))
        {
            isTouchingBamboo = true;
            bambooTimer = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bamboo"))
            isTouchingBamboo = false;
    }

    void Update()
    {
        if (isTouchingBamboo)
        {
            bambooTimer += Time.deltaTime;
            if (bambooTimer >= bambooInterval)
            {
                bambooTimer = 0f;
                PaloSeboScoreManager.Instance.AddScore(2);
            }
        }

        // Slow effect timer
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                isSlowed = false;
                playerMove.SetSlow(false);
            }
        }
    }
}
