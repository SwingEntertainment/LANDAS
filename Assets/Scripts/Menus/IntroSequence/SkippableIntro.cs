using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SkippableIntro : MonoBehaviour
{
    public PlayableDirector IntroSequence;
    public string nextSceneName = "MainMenu";

    private double[] skipTimes = { 3.10, 6.22, 8.50 };
    private int skipIndex = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) ||
             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Debug.Log("Skip pressed!");
            Skip();
        }
    }

    void Skip()
    {
        if (IntroSequence == null) return;

        if (skipIndex < skipTimes.Length)
        {
            IntroSequence.time = skipTimes[skipIndex];
            IntroSequence.Evaluate();
            skipIndex++;
        }
        else
        {
            IntroSequence.time = IntroSequence.duration;
            IntroSequence.Evaluate();
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
