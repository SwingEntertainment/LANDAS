using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SkippableIntro : MonoBehaviour
{
    public PlayableDirector IntroSequence;
    public string nextSceneName = "MainMenu";

    private double[] skipTimes = { 5.167, 10.367, 14.167 };
    private int skipIndex = 0;

    void Start()
    {
        if (IntroSequence != null)
            IntroSequence.Play();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
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
