using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    public float climbSpeed = 2f;      // auto upward speed
    public float moveSpeed = 5f;       // left/right movement speed

    private int moveDirection = 0;     // -1 = left, 0 = none, 1 = right

    void Update()
    {
        // Auto climb upwards
        transform.Translate(Vector3.up * climbSpeed * Time.deltaTime);

        // Move left or right if a button is pressed
        if (moveDirection != 0)
        {
            transform.Translate(Vector3.right * moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    // Called by UI button
    public void MoveLeftDown() => moveDirection = -1;
    public void MoveRightDown() => moveDirection = 1;

    // Called when button released
    public void StopMove() => moveDirection = 0;
}
