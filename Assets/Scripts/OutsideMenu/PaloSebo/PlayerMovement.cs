using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public ButtonHold leftButton;
    public ButtonHold rightButton;

    private bool moveLeft = false;
    private bool moveRight = false;
    private SpriteRenderer spriteRenderer;

    [Header("Player Sprites")]
    public Sprite idleRight;
    public Sprite idleLeft;
    public Sprite climbRight;
    public Sprite climbLeft;

    private bool facingRight = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Button listeners
        leftButton.onHoldStart.AddListener(OnLeftHoldStart);
        leftButton.onHoldEnd.AddListener(OnLeftHoldEnd);
        rightButton.onHoldStart.AddListener(OnRightHoldStart);
        rightButton.onHoldEnd.AddListener(OnRightHoldEnd);

        // Start idle facing right
        spriteRenderer.sprite = idleRight;
    }

    void Update()
    {
        if (moveLeft)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            spriteRenderer.sprite = climbLeft;
            facingRight = false;
        }
        else if (moveRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            spriteRenderer.sprite = climbRight;
            facingRight = true;
        }
        else
        {
            // Idle
            spriteRenderer.sprite = facingRight ? idleRight : idleLeft;
        }
    }

    void OnLeftHoldStart() => moveLeft = true;
    void OnLeftHoldEnd() => moveLeft = false;
    void OnRightHoldStart() => moveRight = true;
    void OnRightHoldEnd() => moveRight = false;
}
