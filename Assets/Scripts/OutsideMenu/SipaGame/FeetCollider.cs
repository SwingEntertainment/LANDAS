using UnityEngine;
using System.Collections;

public class FeetCollider : MonoBehaviour
{
    [Header("References")]
    public PlayerKickAnimation playerKick;
    public Ball ballManager;
    public ScoreManager scoreManager;

    [Header("Bounce Settings")]
    public float bounceForce = 14f;    // how strong the upward kick is
    public float topRespawnY = 6f;     // Y position to trigger respawn
    public float respawnDelay = 0.5f;  // delay before respawn

    private bool hasKicked = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasKicked && collision.gameObject.CompareTag("Ball"))
        {
            StartCoroutine(KickBall(collision.gameObject));
        }
    }

    private IEnumerator KickBall(GameObject ball)
    {
        hasKicked = true;

        // Play kick animation if available
        if (playerKick != null)
            yield return StartCoroutine(playerKick.PlayKickAnimation());

        // Add +1 score
        if (scoreManager != null)
            scoreManager.AddScore(1);

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // reset velocity
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse); // apply upward impulse
        }

        // Wait until ball goes above topRespawnY
        yield return new WaitUntil(() => ball == null || ball.transform.position.y > topRespawnY);

        // Respawn new ball
        if (ball != null && ballManager != null)
        {
            Destroy(ball);
            yield return new WaitForSeconds(respawnDelay);
            ballManager.SpawnBall();
        }

        hasKicked = false;
    }
}
