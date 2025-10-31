using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RecipeHuntGame : MonoBehaviour
{
    [Header("UI References")]
    public Button leftArrow;
    public Button rightArrow;
    public Transform slotsParent;
    public Transform foodTrayParent;

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

    [System.Serializable]
    public class IngredientData
    {
        public int ingredientID;
        public string ingredientName;
        public string ingredientImg;
        public string ingredientContainerImg;
    }

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
        foreach (Transform child in slotsParent)
            slotObjects.Add(child.gameObject);

        foreach (Transform child in foodTrayParent)
            traySlots.Add(child.gameObject);

        StartCoroutine(LoadIngredientsFromStreamingAssets());
        StartCoroutine(LoadDishesFromStreamingAssets());

        cookButton.onClick.AddListener(CookDish);
        cookButton.interactable = false;

        if (cookedDishImage != null)
            cookedDishImage.gameObject.SetActive(false);
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
            Debug.LogError($"Failed to load JSON: {request.error}");
            yield break;
        }
        json = request.downloadHandler.text;
#else
        if (!File.Exists(path))
        {
            Debug.LogError($"JSON not found at path: {path}");
            yield break;
        }
        json = File.ReadAllText(path);
#endif

        IngredientListWrapper wrapper = JsonUtility.FromJson<IngredientListWrapper>(json);
        if (wrapper == null || wrapper.ingredients == null)
        {
            Debug.LogError("Failed to parse ingredients JSON!");
            yield break;
        }

        allIngredients = wrapper.ingredients;
        Debug.Log($"Loaded {allIngredients.Count} ingredients!");

        totalPages = Mathf.CeilToInt((float)allIngredients.Count / itemsPerPage);
        UpdatePage();

        leftArrow.onClick.AddListener(PrevPage);
        rightArrow.onClick.AddListener(NextPage);
    }

    IEnumerator LoadDishesFromStreamingAssets()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "RecipeList.json");
        string json = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load recipe JSON: " + request.error);
            yield break;
        }
        json = request.downloadHandler.text;
#else
        if (!File.Exists(path))
        {
            Debug.LogError("Recipe JSON not found at path: " + path);
            yield break;
        }
        json = File.ReadAllText(path);
#endif

        DishListWrapper wrapper = JsonUtility.FromJson<DishListWrapper>(json);
        if (wrapper == null || wrapper.dishes == null)
        {
            Debug.LogError("Failed to parse recipe JSON!");
            yield break;
        }

        allDishes = wrapper.dishes;
        Debug.Log($"Loaded {allDishes.Count} dishes from RecipeList.json!");
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
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            img.sprite = sprite;

            var addButton = slot.transform.Find("AddButton").GetComponent<Button>();
            addButton.onClick.RemoveAllListeners();
            addButton.onClick.AddListener(() => AddToFoodTray(data));
        }

        leftArrow.gameObject.SetActive(currentPage > 0);
        rightArrow.gameObject.SetActive(currentPage < totalPages - 1);
    }

    void AddToFoodTray(IngredientData ingredient)
    {
        if (selectedIngredients.Count >= traySlots.Count)
        {
            Debug.Log("Tray is full!");
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

        if (matchedDish != null)
        {
            matchedDish.isCooked = true;
            Debug.Log($"Dish created: {matchedDish.dishName}!");
            SetAddButtonsInteractable(false);

            if (sizzleAudio != null)
            {
                sizzleAudio.volume = 1f;
                sizzleAudio.Play();
                StartCoroutine(FadeOutSizzle(5f));
            }

            StartCoroutine(DelayedDishDisplay(matchedDish, 3f));
        }
        else
        {
            Debug.Log("No matching dish found!");
        }
    }

    IEnumerator ShowCookedDish(DishData dish)
    {
        cookButton.interactable = false;

        string path = $"Images/KitchenFood/{Path.GetFileNameWithoutExtension(dish.dishImg)}";
        Sprite dishSprite = Resources.Load<Sprite>(path);

        if (dishSprite == null)
        {
            Debug.LogError("Dish image not found: " + path);
            yield break;
        }

        cookedDishImage.gameObject.SetActive(true);
        cookedDishImage.sprite = dishSprite;

        // FadeIn Animation
        Color c = cookedDishImage.color;
        c.a = 0f;
        cookedDishImage.color = c;

        float elapsed = 0f;
        while (elapsed < dishFadeInDuration)
        {
            elapsed += Time.deltaTime;
            if (elapsed > dishFadeInDuration) elapsed = dishFadeInDuration;
            c.a = Mathf.Lerp(0f, 1f, elapsed / dishFadeInDuration);
            cookedDishImage.color = c;
            yield return null;
        }

        c.a = 1f;
        cookedDishImage.color = c;

        yield return new WaitForSeconds(dishFadeOutDuration);

        // FadeOut Animation
        elapsed = 0f;
        while (elapsed < dishFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            if (elapsed > dishFadeOutDuration) elapsed = dishFadeOutDuration;
            c.a = Mathf.Lerp(1f, 0f, elapsed / dishFadeOutDuration);
            cookedDishImage.color = c;
            yield return null;
        }

        cookedDishImage.gameObject.SetActive(false);

        selectedIngredients.Clear();
        UpdateFoodTray();
        SetAddButtonsInteractable(true);
        cookButton.interactable = (selectedIngredients.Count >= 4 && selectedIngredients.Count <= 5);
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
