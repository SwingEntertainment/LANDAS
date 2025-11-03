using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene Instance;
    public GameObject loadingScreen;
    public Animator loadingAnimator;

    private Coroutine currentRoutine; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }

    public static void LoadSceneWithLoading(string sceneName)
    {
        if (Instance == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        if (Instance.currentRoutine != null)
        {
            Debug.LogWarning("SceneLoader: Already loading a scene, ignoring duplicate call.");
            return;
        }

        Instance.currentRoutine = Instance.StartCoroutine(Instance.LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        if (loadingAnimator != null)
            loadingAnimator.Play("LoadingStartAnimation", 0, 0f);

        yield return new WaitForSeconds(1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                if (loadingAnimator != null)
                    loadingAnimator.Play("LoadingEndAnimation", 0, 0f);

                yield return new WaitForSeconds(0.3f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        currentRoutine = null;
    }
}
