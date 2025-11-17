using UnityEngine;

public class PaloSeboGameManager : MonoBehaviour
{
    public static bool GameStarted = false;

    public static void StartGame()
    {
        GameStarted = true;
    }

    public static void StopGame()
    {
        GameStarted = false;
    }
}
