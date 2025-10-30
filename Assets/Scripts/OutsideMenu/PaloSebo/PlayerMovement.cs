using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            animator.SetTrigger("ClimbLeft");
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            animator.SetTrigger("ClimbRight");
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            animator.SetTrigger("IdleLeft");
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            animator.SetTrigger("IdleRight");
        }
    }
}
