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

    // Immunity variables
    private bool isImmune = false;
    private float immunityDuration = 2f; // 2 seconds of immunity
    private float immunityTimer = 0f;
    
    // 💡 New variables for flickering
    private SpriteRenderer playerSprite; // Reference to the player's SpriteRenderer
    private float flickerInterval = 0.1f; // How fast the player flashes (e.g., 10 times per second)
    private float flickerTimer = 0f;

    void Start()
    {
        playerMove = GetComponent<PlayerMovement>();
        // 💡 Get the SpriteRenderer component
        playerSprite = GetComponent<SpriteRenderer>(); 
        if (playerSprite == null)
        {
            Debug.LogError("SpriteRenderer component not found on the player object!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 🪲 Langgam Hit
        if (other.CompareTag("Langgam"))
        {
            // Only process the hit if the player is NOT immune
            if (!isImmune)
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
                
                // ⭐ Apply 2 seconds of immunity after getting hit
                isImmune = true;
                immunityTimer = immunityDuration;
                
                // 💡 Reset flicker timer immediately to start flashing
                flickerTimer = 0f; 
                // Ensure the sprite is visible at the start of immunity
                if (playerSprite != null)
                {
                    playerSprite.enabled = true;
                }
            }
            
            // 🔥 Remove the collided ant (Langgam) regardless of immunity
            Destroy(other.gameObject);
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
        
        // 🛡️ Immunity timer and Flickering Logic
        if (isImmune)
        {
            immunityTimer -= Time.deltaTime;
            
            // Handle flickering
            if (playerSprite != null)
            {
                flickerTimer -= Time.deltaTime;
                if (flickerTimer <= 0f)
                {
                    // Toggle the visibility of the sprite
                    playerSprite.enabled = !playerSprite.enabled;
                    flickerTimer = flickerInterval;
                }
            }
            
            // Check if immunity has ended
            if (immunityTimer <= 0f)
            {
                isImmune = false;
                // 💡 Ensure the sprite is visible when immunity ends
                if (playerSprite != null)
                {
                    playerSprite.enabled = true;
                }
                Debug.Log("Immunity ended.");
            }
        }
    }
}