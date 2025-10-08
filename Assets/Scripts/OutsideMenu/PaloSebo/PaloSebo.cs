using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PaloSebo : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("OutsideMenu");
    }
}
