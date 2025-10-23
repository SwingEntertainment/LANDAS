using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class RecipeList : MonoBehaviour
{
    [Header("UI - Detail View")]
    public TMP_Text dishTitleText;
    public TMP_Text dishDescriptionText;
    public TMP_Text ingredientsListText;
    public Image dishImage;
    public GameObject detailPanel;

    [Header("UI - Navigation Buttons")]
    public Button nextButton;
    public Button prevButton;
    public Button viewRecipeButton;
    public Button clearDataButton;

    [Header("Scene / File Paths")]
    public string mainSceneName = "GameMenu";
    public string recipeJsonFileName = "RecipeList.json";
    public string imageFolderPath = "../Images/GameMenu/KitchenMenu";

    private List<DishEntry> dishes = new List<DishEntry>();
    private int currentRecipeIndex = 0;
    private string recipeJsonPath;

    void Start()
    {
        recipeJsonPath = Path.Combine(Application.streamingAssetsPath, recipeJsonFileName);
        LoadRecipesFromStreamingAssets();
        SetupButtons();

        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    // ===== JSON LOADING =====
    void LoadRecipesFromStreamingAssets()
    {
        if (!File.Exists(recipeJsonPath))
        {
            Debug.LogWarning($"Recipe JSON not found: {recipeJsonPath}");
            dishes = new List<DishEntry>();
            return;
        }

        string jsonData = File.ReadAllText(recipeJsonPath);
        RecipeData data = JsonUtility.FromJson<RecipeData>(jsonData);

        if (data == null || data.dishes == null)
        {
            Debug.LogWarning("Recipe JSON parsed but no dishes found.");
            dishes = new List<DishEntry>();
            return;
        }

        dishes = new List<DishEntry>(data.dishes);
        dishes.Sort((a, b) => a.foodID.CompareTo(b.foodID));
    }

    // ===== JSON SAVE =====
    void SaveRecipesToStreamingAssets()
    {
        RecipeData data = new RecipeData { dishes = dishes.ToArray() };
        string jsonOutput = JsonUtility.ToJson(data, true);
        File.WriteAllText(recipeJsonPath, jsonOutput);
        Debug.Log($"Recipe JSON updated and saved to: {recipeJsonPath}");
    }

    // ===== RESET ALL RECIPES =====
    public void ResetAllRecipes()
    {
        foreach (var dish in dishes)
            dish.isCooked = false;

        SaveRecipesToStreamingAssets();
        Debug.Log("All recipes reset to uncooked.");
        ShowRecipeDetail(currentRecipeIndex);
    }

    // ===== BUTTONS SETUP =====
    void SetupButtons()
    {
        if (viewRecipeButton != null)
            viewRecipeButton.onClick.AddListener(() => ShowRecipeDetail(0));

        if (nextButton != null)
            nextButton.onClick.AddListener(ShowNextRecipe);

        if (prevButton != null)
            prevButton.onClick.AddListener(ShowPreviousRecipe);

        if (clearDataButton != null)
            clearDataButton.onClick.AddListener(ResetAllRecipes);
    }

    // ===== MARK AS COOKED =====
    public void MarkDishAsCooked(int foodID)
    {
        DishEntry dish = dishes.Find(d => d.foodID == foodID);
        if (dish != null && !dish.isCooked)
        {
            dish.isCooked = true;
            SaveRecipesToStreamingAssets();
            Debug.Log($"Dish '{dish.dishName}' marked as cooked!");
        }
    }

    // ===== NAVIGATION =====
    void ShowNextRecipe()
    {
        if (dishes.Count == 0) return;
        currentRecipeIndex++;
        if (currentRecipeIndex >= dishes.Count)
            currentRecipeIndex = dishes.Count - 1;

        ShowRecipeDetail(currentRecipeIndex);
    }

    void ShowPreviousRecipe()
    {
        if (dishes.Count == 0) return;
        currentRecipeIndex--;
        if (currentRecipeIndex < 0)
            currentRecipeIndex = 0;

        ShowRecipeDetail(currentRecipeIndex);
    }

    // ===== SHOW RECIPE DETAILS =====
    void ShowRecipeDetail(int index)
    {
        if (index < 0 || index >= dishes.Count) return;

        currentRecipeIndex = index;
        DishEntry dish = dishes[index];

        // Title
        dishTitleText.text = dish.isCooked ? dish.dishName : "???";

        // Image path
        string imgPath = Path.Combine(Path.GetFullPath(Application.streamingAssetsPath), imageFolderPath, dish.dishImg ?? "");
        Sprite spriteToUse = null;

        if (File.Exists(imgPath))
            spriteToUse = LoadSpriteFromFile(imgPath);

        if (dishImage != null)
        {
            dishImage.sprite = spriteToUse;

            RectTransform rt = dishImage.GetComponent<RectTransform>();
            if (rt != null)
                rt.sizeDelta = new Vector2(290f, 110f);

            dishImage.color = dish.isCooked ? Color.white : new Color(0.05f, 0.05f, 0.05f, 0.9f);
        }

        // Description & Ingredients
        if (dish.isCooked)
        {
            dishDescriptionText.text = string.IsNullOrEmpty(dish.dishDescription)
                ? "No description."
                : dish.dishDescription;

            ingredientsListText.text = FormatIngredients(dish.recipeList, dish.ingredientIDs);
        }
        else
        {
            dishDescriptionText.text = "Recipe not found.";
            ingredientsListText.text = "Recipe not found.";
        }

        if (detailPanel != null)
            detailPanel.SetActive(true);

        prevButton.interactable = (currentRecipeIndex > 0);
        nextButton.interactable = (currentRecipeIndex < dishes.Count - 1);
    }

    // ===== LOAD SPRITE FROM FILE =====
    Sprite LoadSpriteFromFile(string path)
    {
        if (!File.Exists(path)) return null;
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        if (!tex.LoadImage(fileData)) return null;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    // ===== FORMAT INGREDIENTS =====
    string FormatIngredients(List<string> ingredients, List<int> ingredientIDs)
    {
        if ((ingredients == null || ingredients.Count == 0) && (ingredientIDs == null || ingredientIDs.Count == 0))
            return "No ingredients listed.";

        List<string> lines = new List<string>();
        if (ingredients != null && ingredients.Count > 0)
        {
            foreach (string ing in ingredients)
                lines.Add("• " + ing);
        }
        else if (ingredientIDs != null && ingredientIDs.Count > 0)
        {
            foreach (int id in ingredientIDs)
                lines.Add("• Ingredient #" + id);
        }

        return string.Join("\n", lines);
    }
}

#region JSON DATA CLASSES
[System.Serializable]
public class DishEntry
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
public class RecipeData
{
    public DishEntry[] dishes;
}
#endregion
