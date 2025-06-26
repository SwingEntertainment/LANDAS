using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("GameMenu");
    }
}
