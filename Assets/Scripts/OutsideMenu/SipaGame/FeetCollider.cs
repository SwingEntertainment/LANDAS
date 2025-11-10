using UnityEngine;
using System.Collections;

public class FeetCollider : MonoBehaviour
{
    [Header("References")]
    public PlayerKickAnimation playerKick;
    public Ball ballManager;
    private ScoreManager scoreManager; 

    [Header("Bounce Settings")]
    public float bounceForce = 14f;    
    public float topRespawnY = 6f;     
    public float respawnDelay = 0.5f;  
    public float maxWaitTime = 2f;  

    private bool hasKicked = false;
    private bool isGameOver = false;

    private void Awake()
    {
        scoreManager = ScoreManager.Instance;
    }

    private void OnEnable()
    {
        hasKicked = false;
        isGameOver = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGameOver) return;

        if (!hasKicked && collision.gameObject.CompareTag("Ball"))
        {
            StartCoroutine(KickBall(collision.gameObject));
        }
    }

    private IEnumerator KickBall(GameObject ball)
    {
        if (ball == null) yield break;
        hasKicked = true;

        if (playerKick != null)
            yield return StartCoroutine(playerKick.PlayKickAnimation());

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(1);

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            float randomX = Random.Range(-1.5f, 1.5f);
            rb.AddForce(new Vector2(randomX, bounceForce), ForceMode2D.Impulse);
        }

        float startTime = Time.time;
        yield return new WaitUntil(() =>
            ball == null || 
            ball.transform == null || 
            ball.transform.position.y > topRespawnY ||
            (Time.time - startTime > maxWaitTime)
        );

        if (!isGameOver && ball != null && ballManager != null)
        {
            Destroy(ball);
            yield return new WaitForSeconds(respawnDelay);
            ballManager.SpawnBall();
        }

        hasKicked = false;
    }

    public void SetGameOver(bool state)
    {
        isGameOver = state;
    }
}
