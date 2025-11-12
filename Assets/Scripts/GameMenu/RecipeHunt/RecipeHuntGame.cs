using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class RecipeHuntGame : MonoBehaviour
{
    [Header("UI References")]
    public Button leftArrow;
    public Button rightArrow;
    public Transform slotsParent;
    public Transform foodTrayParent;
    public float bubbleShowDuration = 2f;
    public float bubbleFadeDuration = 0.5f;

    [Header("Cooking System")]
    public Button cookButton;
    public Image cookedDishImage;
    public float dishFadeInDuration = 1.5f;
    public float dishFadeOutDuration = 2f;
    public AudioSource sizzleAudio;

    [Header("Pagination Settings")]
    public int itemsPerPage = 6;
    private List<IngredientData> allIngredients = new List<IngredientData>();
    private List<DishData> allDishes = new List<DishData>();
    private int currentPage = 0;
    private int totalPages = 0;
    private List<GameObject> slotObjects = new List<GameObject>();
    private List<GameObject> traySlots = new List<GameObject>();
    private List<IngredientData> selectedIngredients = new List<IngredientData>();

    [Header("Cooking Result Panels")]
    public GameObject successPanel;
    public GameObject failedPanel;
    public TMP_Text dishNameText;
    public Image dishImage;
    public TMP_Text failHintText;
    public TMP_Text successHeaderText;
    public TMP_Text failedHeaderText;
    public Image failedDishImage;
    public AudioSource successCookingSFX;
    public AudioSource failedCookingSFX;

    [System.Serializable]
    public class IngredientData
    {
        public int ingredientID;
        public string ingredientName;
        public string ingredientImg;
        public string ingredientContainerImg;
    }
    private Coroutine dishAnimationCoroutine;
    private Coroutine panelFadeCoroutine;
    private CanvasGroup successPanelCanvasGroup;
    public string recipeJsonFileName = "RecipeList.json";
    private string streamingRecipePath;
    private string persistentRecipePath;

    [System.Serializable]
    public class IngredientListWrapper
    {
        public List<IngredientData> ingredients;
    }

    [System.Serializable]
    public class DishData
    {
        public int foodID;
        public bool isCooked;
        public string dishName;
        public string dishImg;
        public List<string> recipeList;
        public List<int> ingredientIDs;
        public string dishDescription;
    }

    [System.Serializable]
    public class DishListWrapper
    {
        public List<DishData> dishes;
    }

    void Start()
    {

        streamingRecipePath = Path.Combine(Application.streamingAssetsPath, recipeJsonFileName);
        persistentRecipePath = Path.Combine(Application.persistentDataPath, recipeJsonFileName);

        // Ensure persistent copy exists before any dish-loading happens
        StartCoroutine(EnsureRecipeFileExists());

        foreach (Transform child in slotsParent)
            slotObjects.Add(child.gameObject);

        foreach (Transform child in foodTrayParent)
            traySlots.Add(child.gameObject);

        // load ingredients immediately (ingredients live in StreamingAssets and we read them with platform-safe code)
        StartCoroutine(LoadIngredientsFromStreamingAssets());

        cookButton.onClick.AddListener(CookDish);
        cookButton.interactable = false;

        if (cookedDishImage != null)
            cookedDishImage.gameObject.SetActive(false);

        successPanelCanvasGroup = successPanel.GetComponent<CanvasGroup>();
        if (successPanelCanvasGroup == null)
            successPanelCanvasGroup = successPanel.AddComponent<CanvasGroup>();

        // start hidden
        successPanelCanvasGroup.alpha = 0f;
        successPanel.SetActive(false);
    }

    IEnumerator EnsureRecipeFileExists()
    {
        if (File.Exists(persistentRecipePath))
        {
            StartCoroutine(LoadDishesFromFile());
            yield break;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
    UnityWebRequest request = UnityWebRequest.Get(streamingRecipePath);
    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        yield break;
    }

    try
    {
        File.WriteAllText(persistentRecipePath, request.downloadHandler.text);
    }
    catch (System.Exception ex)
    {
        yield break;
    }
#else
        if (!File.Exists(streamingRecipePath))
        {
            yield break;
        }

        string streamingJson = null;
        try
        {
            streamingJson = File.ReadAllText(streamingRecipePath);
        }
        catch (System.Exception ex)
        {
            yield break;
        }

        try
        {
            File.WriteAllText(persistentRecipePath, streamingJson);
        }
        catch (System.Exception ex)
        {
            yield break;
        }

        yield return null;
#endif

        StartCoroutine(LoadDishesFromFile());
    }

    IEnumerator LoadDishesFromFile()
    {
        if (!File.Exists(persistentRecipePath))
        {
            yield break;
        }

        string json = File.ReadAllText(persistentRecipePath);
        DishListWrapper wrapper = JsonUtility.FromJson<DishListWrapper>(json);

        if (wrapper == null || wrapper.dishes == null)
        {
            yield break;
        }

        allDishes = wrapper.dishes;
    }

    IEnumerator LoadIngredientsFromStreamingAssets()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "IngredientsList.json");
        string json = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            yield break;
        }
        json = request.downloadHandler.text;
#else
        if (!File.Exists(path))
        {
            yield break;
        }
        json = File.ReadAllText(path);
#endif

        IngredientListWrapper wrapper = JsonUtility.FromJson<IngredientListWrapper>(json);
        if (wrapper == null || wrapper.ingredients == null)
        {
            yield break;
        }

        allIngredients = wrapper.ingredients;
        totalPages = Mathf.CeilToInt((float)allIngredients.Count / itemsPerPage);
        UpdatePage();

        leftArrow.onClick.AddListener(PrevPage);
        rightArrow.onClick.AddListener(NextPage);
    }

    IEnumerator LoadDishesFromStreamingAssets()
    {
        string persistentPath = Path.Combine(Application.persistentDataPath, "RecipeProgress.json");
        string json = "";

        if (File.Exists(persistentPath))
        {
            json = File.ReadAllText(persistentPath);
        }
        else
        {
            string streamingPath = Path.Combine(Application.streamingAssetsPath, "RecipeList.json");

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest request = UnityWebRequest.Get(streamingPath);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            yield break;
        }
        json = request.downloadHandler.text;
#else
            if (!File.Exists(streamingPath))
            {
                yield break;
            }
            json = File.ReadAllText(streamingPath);
#endif
        }

        DishListWrapper wrapper = JsonUtility.FromJson<DishListWrapper>(json);
        if (wrapper == null || wrapper.dishes == null)
        {
            yield break;
        }

        allDishes = wrapper.dishes;
    }


    void UpdatePage()
    {
        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, allIngredients.Count);

        for (int i = 0; i < slotObjects.Count; i++)
            slotObjects[i].SetActive(false);

        for (int i = startIndex, slotIndex = 0; i < endIndex; i++, slotIndex++)
        {
            var data = allIngredients[i];
            var slot = slotObjects[slotIndex];
            slot.SetActive(true);

            var img = slot.GetComponent<Image>();
            string imagePath = $"Images/Ingredients/{Path.GetFileNameWithoutExtension(data.ingredientContainerImg)}";
            img.sprite = Resources.Load<Sprite>(imagePath);

            var addButton = slot.transform.Find("AddButton").GetComponent<Button>();
            addButton.onClick.RemoveAllListeners();
            addButton.onClick.AddListener(() => AddToFoodTray(data));

            var slotButton = slot.GetComponent<Button>();
            Transform nameBubble = slot.transform.Find("TextBubble");
            TMP_Text nameText = nameBubble?.GetComponentInChildren<TMP_Text>();

            if (nameBubble != null)
                nameBubble.gameObject.SetActive(false);

            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() =>
            {
                if (nameBubble != null && nameText != null)
                {
                    nameText.text = data.ingredientName;
                    StartCoroutine(ShowBubble(nameBubble.gameObject, 2f));
                }
            });
        }

        leftArrow.gameObject.SetActive(currentPage > 0);
        rightArrow.gameObject.SetActive(currentPage < totalPages - 1);
    }

    IEnumerator ShowBubble(GameObject bubble, float duration)
    {
        // Make sure it’s visible
        bubble.SetActive(true);

        CanvasGroup cg = bubble.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = bubble.AddComponent<CanvasGroup>();

        // Fade In
        float fadeTime = 0.3f;
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }

        // Stay visible for duration
        yield return new WaitForSeconds(duration);

        // Fade Out
        t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        bubble.SetActive(false);
    }

    void AddToFoodTray(IngredientData ingredient)
    {
        if (selectedIngredients.Count >= traySlots.Count)
        {
            return;
        }

        selectedIngredients.Add(ingredient);
        UpdateFoodTray();
    }

    void UpdateFoodTray()
    {
        for (int i = 0; i < traySlots.Count; i++)
        {
            var slot = traySlots[i];
            var img = slot.GetComponent<Image>();

            foreach (Transform child in slot.transform)
                Destroy(child.gameObject);

            if (i >= selectedIngredients.Count)
            {
                slot.SetActive(false);
                continue;
            }

            slot.SetActive(true);

            var data = selectedIngredients[i];
            string path = $"Images/Ingredients/{Path.GetFileNameWithoutExtension(data.ingredientImg)}";
            Sprite sprite = Resources.Load<Sprite>(path);

            if (sprite != null)
                img.sprite = sprite;
            img.preserveAspect = true;

            var foodTrayImg = img.GetComponent<RectTransform>();
            foodTrayImg.sizeDelta = new Vector2(70, 70);
            foodTrayImg.localScale = Vector3.one;
            foodTrayImg.anchoredPosition = Vector2.zero;

            // Remove button Creation
            GameObject removeObj = new GameObject("RemoveButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            removeObj.transform.SetParent(slot.transform, false);

            var removeButton = removeObj.GetComponent<RectTransform>();
            removeButton.anchorMin = new Vector2(1, 1);
            removeButton.anchorMax = new Vector2(1, 1);
            removeButton.pivot = new Vector2(1, 1);
            removeButton.anchoredPosition = new Vector2(-5, -5);
            removeButton.sizeDelta = new Vector2(25, 25);

            Image removeImg = removeObj.GetComponent<Image>();
            removeImg.sprite = Resources.Load<Sprite>("Images/Ingredients/Remove-Button");

            Button removeBtn = removeObj.GetComponent<Button>();
            int index = i;
            removeBtn.onClick.AddListener(() => RemoveFromTray(index));
        }

        cookButton.interactable = (selectedIngredients.Count >= 4 && selectedIngredients.Count <= 5);
    }

    void RemoveFromTray(int index)
    {
        if (index >= 0 && index < selectedIngredients.Count)
        {
            selectedIngredients.RemoveAt(index);
            UpdateFoodTray();
        }
    }

    void SetAddButtonsInteractable(bool interactable)
    {
        foreach (var slot in slotObjects)
        {
            var addButton = slot.transform.Find("AddButton")?.GetComponent<Button>();
            if (addButton != null)
                addButton.interactable = interactable;
        }
    }

    void CookDish()
    {
        cookButton.interactable = false;
        SetAllRemoveButtonsInteractable(false);
        List<int> trayIngredientIDs = selectedIngredients.Select(i => i.ingredientID).ToList();
        trayIngredientIDs.Sort();

        DishData matchedDish = null;

        foreach (var dish in allDishes)
        {
            if (dish.ingredientIDs == null || dish.ingredientIDs.Count == 0)
                continue;

            List<int> sortedRecipeIDs = new List<int>(dish.ingredientIDs);
            sortedRecipeIDs.Sort();

            if (trayIngredientIDs.SequenceEqual(sortedRecipeIDs))
            {
                matchedDish = dish;
                break;
            }
        }

        successPanel.SetActive(false);
        failedPanel.SetActive(false);

        if (matchedDish != null)
        {
            matchedDish.isCooked = true;
            SaveDishProgress();
            SetAddButtonsInteractable(false);

            if (sizzleAudio != null)
            {
                sizzleAudio.volume = 1f;
                sizzleAudio.Play();
                StartCoroutine(FadeOutSizzle(5f));
            }

            if (dishAnimationCoroutine != null)
            {
                StopCoroutine(dishAnimationCoroutine);
                dishAnimationCoroutine = null;
            }

            dishAnimationCoroutine = StartCoroutine(DelayedDishDisplay(matchedDish, 3f));
        }
        else
        {
            SetAddButtonsInteractable(false);

            if (sizzleAudio != null)
            {
                sizzleAudio.volume = 1f;
                sizzleAudio.Play();
                StartCoroutine(FadeOutSizzle(5f));
            }

            if (dishAnimationCoroutine != null)
            {
                StopCoroutine(dishAnimationCoroutine);
                dishAnimationCoroutine = null;
            }

            dishAnimationCoroutine = StartCoroutine(DelayedFailedDishDisplay("Hint: Lola wrote some recipe notes around so she doesn't forget them. Check the surroundings!", 3f));
        }

    }

    IEnumerator ShowCookedDish(DishData dish)
    {
        cookButton.interactable = false;

        // Load dish sprite
        string path = $"Images/KitchenFood/{Path.GetFileNameWithoutExtension(dish.dishImg)}";
        Sprite dishSprite = Resources.Load<Sprite>(path);

        if (dishSprite == null)
        {
            yield break;
        }

        // INITIAL UI SETUP
        successPanel.SetActive(false);
        yield return null;
        successPanel.SetActive(true);

        CanvasGroup cg = successPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = successPanel.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        // Ensure all key elements exist and are invisible initially
        if (successHeaderText != null)
        {
            Color headerColor = successHeaderText.color;
            headerColor.a = 0f;
            successHeaderText.color = headerColor;
        }

        if (dishNameText != null)
        {
            dishNameText.text = dish.dishName.ToUpper();
            Color nameColor = dishNameText.color;
            nameColor.a = 0f;
            dishNameText.color = nameColor;
        }

        if (cookedDishImage != null)
        {
            cookedDishImage.gameObject.SetActive(true);
            cookedDishImage.sprite = dishSprite;

            Color imgColor = cookedDishImage.color;
            imgColor.a = 0f;
            cookedDishImage.color = imgColor;
        }

        // Add small scale-in effect
        float startScale = 0.8f;
        cookedDishImage.rectTransform.localScale = Vector3.one * startScale;
        dishNameText.rectTransform.localScale = Vector3.one * startScale;

        Canvas.ForceUpdateCanvases();

        // STEP 1: Wait 1.5 seconds before showing header
        yield return new WaitForSeconds(1.5f);

        // Instantly show "You Have Created!"
        if (successHeaderText != null)
        {
            Color headerColor = successHeaderText.color;
            headerColor.a = 1f;
            successHeaderText.color = headerColor;
        }

        // STEP 2: Wait another 1.5 seconds before fading in dish image & name
        yield return new WaitForSeconds(1.5f);

        if (successCookingSFX != null)
        {
            successCookingSFX.Play();
        }

        // STEP 3: Fade in dish image and name together
        float elapsed = 0f;
        while (elapsed < dishFadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dishFadeInDuration);

            float alpha = Mathf.Lerp(0f, 1f, t);
            float scale = Mathf.Lerp(startScale, 1f, t);

            if (cookedDishImage != null)
            {
                Color imgColor = cookedDishImage.color;
                imgColor.a = alpha;
                cookedDishImage.color = imgColor;
                cookedDishImage.rectTransform.localScale = Vector3.one * scale;
            }

            if (dishNameText != null)
            {
                Color nameColor = dishNameText.color;
                nameColor.a = alpha;
                dishNameText.color = nameColor;
                dishNameText.rectTransform.localScale = Vector3.one * scale;
            }

            yield return null;
        }

        // Ensure both are fully visible at end
        if (cookedDishImage != null)
        {
            Color imgColor = cookedDishImage.color;
            imgColor.a = 1f;
            cookedDishImage.color = imgColor;
        }

        if (dishNameText != null)
        {
            Color nameColor = dishNameText.color;
            nameColor.a = 1f;
            dishNameText.color = nameColor;
        }

        // --- STEP 4: Keep visible for 2 seconds ---
        yield return new WaitForSeconds(2f);

        // --- STEP 5: Fade out entire panel ---
        yield return StartCoroutine(FadeOutPanel(successPanel, 1.5f));

        successPanel.SetActive(false);
        cookedDishImage.gameObject.SetActive(false);

        // RESET UI Elements
        selectedIngredients.Clear();
        UpdateFoodTray();
        SetAddButtonsInteractable(true);
        cookButton.interactable = (selectedIngredients.Count >= 4 && selectedIngredients.Count <= 5);
        SetAllRemoveButtonsInteractable(true);
        dishAnimationCoroutine = null;
    }


    void ShowSuccessPanel(DishData dish)
    {
        // Ensure any in-progress panel fade is stopped
        if (panelFadeCoroutine != null)
        {
            StopCoroutine(panelFadeCoroutine);
            panelFadeCoroutine = null;
        }

        // Ensure successPanel's CanvasGroup is ready and visible
        if (successPanelCanvasGroup == null)
        {
            successPanelCanvasGroup = successPanel.GetComponent<CanvasGroup>();
            if (successPanelCanvasGroup == null)
                successPanelCanvasGroup = successPanel.AddComponent<CanvasGroup>();
        }

        successPanel.SetActive(true);
        successPanelCanvasGroup.alpha = 1f;

        failedPanel.SetActive(false);

        if (dishNameText != null)
        {
            dishNameText.text = dish.dishName.ToUpper();

            Color tColor = dishNameText.color;
            tColor.a = 0f;
            dishNameText.color = tColor;
        }

        if (dishImage != null)
        {
            string path = $"Images/KitchenFood/{Path.GetFileNameWithoutExtension(dish.dishImg)}";
            Sprite sprite = Resources.Load<Sprite>(path);
            dishImage.sprite = sprite;
        }

        if (cookedDishImage != null)
        {
            Color imgColor = cookedDishImage.color;
            imgColor.a = 0f;
            cookedDishImage.color = imgColor;
        }
    }

    private IEnumerator ShowFailedPanel(string hintMessage)
    {
        cookButton.interactable = false;

        // INITIAL SETUP
        failedPanel.SetActive(false);
        yield return null;
        failedPanel.SetActive(true);

        CanvasGroup cg = failedPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = failedPanel.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        // Hide all UI elements initially
        if (failedHeaderText != null)
        {
            Color headerColor = failedHeaderText.color;
            headerColor.a = 0f;
            failedHeaderText.color = headerColor;
        }

        if (failHintText != null)
        {
            Color hintColor = failHintText.color;
            hintColor.a = 0f;
            failHintText.color = hintColor;
            failHintText.text = hintMessage;
        }

        if (failedDishImage != null)
        {
            Color imgColor = failedDishImage.color;
            imgColor.a = 0f;
            failedDishImage.color = imgColor;
        }

        Canvas.ForceUpdateCanvases();

        // Delay of 1.5 seconds before showing everything
        yield return new WaitForSeconds(1.5f);

        if (failedCookingSFX != null)
        {
            failedCookingSFX.Play();
        }

        // Instantly show all UI elements (no fade-in)
        if (failedHeaderText != null)
        {
            Color headerColor = failedHeaderText.color;
            headerColor.a = 1f;
            failedHeaderText.color = headerColor;
        }

        if (failedDishImage != null)
        {
            Color imgColor = failedDishImage.color;
            imgColor.a = 1f;
            failedDishImage.color = imgColor;
        }

        if (failHintText != null)
        {
            Color hintColor = failHintText.color;
            hintColor.a = 1f;
            failHintText.color = hintColor;
        }

        //  Visible for 3 seconds 
        yield return new WaitForSeconds(3f);

        // Fade out entire panel
        yield return StartCoroutine(FadeOutPanel(failedPanel, 1.5f));

        failedPanel.SetActive(false);

        // RESET UI Elements
        selectedIngredients.Clear();
        UpdateFoodTray();
        SetAddButtonsInteractable(true);
        cookButton.interactable = (selectedIngredients.Count >= 4 && selectedIngredients.Count <= 5);
        SetAllRemoveButtonsInteractable(true);
        dishAnimationCoroutine = null;
    }


    IEnumerator FadeOutSizzle(float duration)
    {
        if (sizzleAudio == null) yield break;

        float startVolume = sizzleAudio.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (elapsed > duration) elapsed = duration;
            sizzleAudio.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        sizzleAudio.Stop();
        sizzleAudio.volume = startVolume;
    }

    IEnumerator DelayedDishDisplay(DishData dish, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(ShowCookedDish(dish));
    }

    IEnumerator DelayedFailedDishDisplay(string hintMessage, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(ShowFailedPanel(hintMessage));
    }

    IEnumerator FadeOutPanelCoroutine(GameObject panel, float duration)
    {
        if (panel == null) yield break;

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        float startAlpha = cg.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            cg.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        cg.alpha = 0f;
    }

    IEnumerator FadeOutPanel(GameObject panel, float duration)
    {
        if (panelFadeCoroutine != null)
        {
            StopCoroutine(panelFadeCoroutine);
            panelFadeCoroutine = null;
        }

        panelFadeCoroutine = StartCoroutine(FadeOutPanelCoroutine(panel, duration));
        yield return panelFadeCoroutine;

        panelFadeCoroutine = null;
    }

    void SetAllRemoveButtonsInteractable(bool interactable)
    {
        // Find all RemoveButtons in the current tray
        foreach (Transform slot in foodTrayParent)
        {
            Button removeBtn = slot.GetComponentInChildren<Button>(true);
            if (removeBtn != null && removeBtn.name == "RemoveButton")
            {
                removeBtn.interactable = interactable;
            }
        }
    }
    void SaveDishProgress()
    {
        try
        {
            DishListWrapper wrapper = new DishListWrapper { dishes = allDishes };
            string json = JsonUtility.ToJson(wrapper, true);

            File.WriteAllText(persistentRecipePath, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save recipe progress: {ex.Message}");
        }
    }


    void NextPage()
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }

    void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }
}

