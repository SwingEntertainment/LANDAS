using UnityEngine;
using UnityEngine.SceneManagement;

public class RecipeHuntMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string kitchenScene = "KitchenMenu";

    public void GoToKitchenMenu()
    {
        SceneManager.LoadScene(kitchenScene);
    }
}
