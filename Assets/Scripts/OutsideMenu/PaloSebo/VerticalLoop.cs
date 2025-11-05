using UnityEngine;

public class VerticalLoop : MonoBehaviour
{
    public float height = 10f; // height of sprite

    void Update()
    {
        if (Camera.main.transform.position.y - transform.position.y > height)
        {
            transform.position += new Vector3(0, height * 2f, 0);
        }
    }
}
