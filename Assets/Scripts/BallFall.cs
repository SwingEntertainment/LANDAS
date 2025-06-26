using UnityEngine;

public class BallFall : MonoBehaviour
{
    public SipaGame gameManager;
    private float fallSpeed = 5f;
    private bool hasScored = false;

    public void SetSpeed(float speed)
    {
        fallSpeed = speed;
        Debug.Log("Speed set to: " + fallSpeed);
    }

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -6f && !hasScored)
        {
            gameManager.GameOver(); 
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Foot"))
        {
            hasScored = true;

            gameManager.AddScore();

            transform.position = new Vector3(Random.Range(-3f, 3f), 7f, 0f);


            SetSpeed(Random.Range(gameManager.minSpeed, gameManager.maxSpeed));
        }
    }
}
