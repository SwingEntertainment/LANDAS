using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ImageAltName : MonoBehaviour
{
    [System.Serializable]
    public class ImageData
    {
        public Image image;        
        public string altName;     
    }

    public ImageData[] targetImages;
    public TMP_Text altNameText;    
    public float displayDuration = 2f; 

    private Coroutine hideRoutine;

    private void Start()
    {
        foreach (ImageData data in targetImages)
        {
            if (data.image != null)
            {
                Button btn = data.image.GetComponent<Button>();
                if (btn == null)
                {
                    btn = data.image.gameObject.AddComponent<Button>();
                }

                string nameToShow = data.altName; 
                btn.onClick.AddListener(() => ShowAltName(nameToShow));
            }
        }

        if (altNameText != null)
        {
            altNameText.text = "";
        }
    }

    private void ShowAltName(string name)
    {
        if (altNameText == null) return;

        altNameText.text = name;

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        if (altNameText != null)
        {
            altNameText.text = "";
        }
    }
}
