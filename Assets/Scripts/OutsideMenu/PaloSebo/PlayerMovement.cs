using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public ButtonHold leftButton;
    public ButtonHold rightButton;

    private SpriteRenderer spriteRenderer;

    [Header("Player Sprites")]
    public Sprite idleRight;
    public Sprite idleLeft;
    public Sprite climbRight;
    public Sprite climbLeft;

    private bool facingRight = true;
    private bool isSlowed = false;

    // Fixed positions for left and right
    private Vector2 leftPosition = new Vector2(510.0657f, 95f);
    private Vector2 rightPosition = new Vector2(570.188f, 95f);

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        leftButton.onHoldStart.AddListener(OnLeftHoldStart);
        leftButton.onHoldEnd.AddListener(OnHoldEnd);
        rightButton.onHoldStart.AddListener(OnRightHoldStart);
        rightButton.onHoldEnd.AddListener(OnHoldEnd);

        spriteRenderer.sprite = idleRight;
    }

    public void SetSlow(bool value)
    {
        isSlowed = value;
    }

    void OnLeftHoldStart()
    {
        float offset = isSlowed ? 5f : 0f; // less responsive when slowed
        transform.position = new Vector3(leftPosition.x - offset, leftPosition.y, transform.position.z);
        spriteRenderer.sprite = climbLeft;
        facingRight = false;
    }

    void OnRightHoldStart()
    {
        float offset = isSlowed ? 5f : 0f;
        transform.position = new Vector3(rightPosition.x + offset, rightPosition.y, transform.position.z);
        spriteRenderer.sprite = climbRight;
        facingRight = true;
    }

    void OnHoldEnd()
    {
        spriteRenderer.sprite = facingRight ? idleRight : idleLeft;
    }
}
