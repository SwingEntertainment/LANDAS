using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InteractiveCommentsFunction : MonoBehaviour
{
    [System.Serializable]
    public class InteractiveObject
    {
        public Image clickableImage;        
        [TextArea] public string[] comments;

        [HideInInspector] public int lastIndex = -1; 
    }

    [Header("Interactive Objects Setup")]
    public InteractiveObject[] objects;     

    [Header("Shared Popup UI")]
    public GameObject popupContainer;       
    public TMP_Text popupText;             

    [Header("Popup Settings")]
    public float displayDuration = 2f;    

    private Coroutine hideRoutine;

    private void Start()
    {
        if (popupContainer != null)
        {
            popupContainer.SetActive(false);
        }

        foreach (InteractiveObject obj in objects)
        {
            if (obj.clickableImage != null)
            {
                Button btn = obj.clickableImage.GetComponent<Button>();
                if (btn == null)
                {
                    btn = obj.clickableImage.gameObject.AddComponent<Button>();
                }

                InteractiveObject currentObj = obj;
                btn.onClick.AddListener(() => ShowRandomComment(currentObj));
            }
        }
    }

    private void ShowRandomComment(InteractiveObject obj)
    {
        if (popupContainer == null || popupText == null) return;
        if (obj.comments == null || obj.comments.Length == 0) return;

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        int randomIndex;
        if (obj.comments.Length == 1)
        {
            randomIndex = 0; 
        }
        else
        {
            do
            {
                randomIndex = Random.Range(0, obj.comments.Length);
            }
            while (randomIndex == obj.lastIndex);
        }

        obj.lastIndex = randomIndex;
        string randomComment = obj.comments[randomIndex];

        popupContainer.SetActive(true);
        popupText.text = randomComment;

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        if (popupContainer != null && popupText != null)
        {
            popupText.text = "";
            popupContainer.SetActive(false);
        }
    }
}
