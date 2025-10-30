using UnityEngine;
using UnityEngine.UI;

public class LoadingSlides : MonoBehaviour
{
    [Header("Spinner Settings")]
    public Image spinnerImage;         
    public float rotationSpeed = 300f;  

    void Update()
    {
        if (spinnerImage != null)
        {
            spinnerImage.rectTransform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
    }
}
