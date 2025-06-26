using UnityEngine;
using UnityEngine.SceneManagement;

public class Dictionary : MonoBehaviour
{
    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("GameMenu");
    }
}
