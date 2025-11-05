using UnityEngine;

public class psTriggerTester : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered: " + other.name);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Staying on: " + other.name);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exited: " + other.name);
    }
}
