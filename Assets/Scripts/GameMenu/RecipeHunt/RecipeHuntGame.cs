using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeHuntGame : MonoBehaviour
{
    [Header("UI References")]
    public Button leftArrow;
    public Button rightArrow;
    public Transform slotsParent; // Parent of 6 slots (IngredientSlotsParent)

    [Header("Pagination Settings")]
    public int itemsPerPage = 6;

    private List<IngredientData> allIngredients = new List<IngredientData>();
    private int currentPage = 0;
    private int totalPages = 0;
    private List<GameObject> slotObjects = new List<GameObject>();

    [System.Serializable]
    public class IngredientData
    {
        public int ingredientID;
        public string ingredientName;
        public string ingredientImg;          // Unused for now
        public string ingredientContainerImg; // This is the image we’ll display
    }

    void Start()
    {
        // Collect all slot references automatically
        foreach (Transform child in slotsParent)
            slotObjects.Add(child.gameObject);

        // Load ingredient data (temporary, can be replaced later with JSON)
        LoadIngredients();

        // Calculate number of pages
        totalPages = Mathf.CeilToInt((float)allIngredients.Count / itemsPerPage);

        // Initialize first page
        UpdatePage();

        // Add arrow listeners
        leftArrow.onClick.AddListener(PrevPage);
        rightArrow.onClick.AddListener(NextPage);
    }

    void LoadIngredients()
    {
        // Example data — replace later with JSON loading
        allIngredients.Add(new IngredientData { ingredientID = 1, ingredientName = "Bay Leaves", ingredientContainerImg = "Bay_leaves_Container" });
        allIngredients.Add(new IngredientData { ingredientID = 2, ingredientName = "Bean Sprouts", ingredientContainerImg = "Bean_Sprouts_Container" });
        allIngredients.Add(new IngredientData { ingredientID = 3, ingredientName = "Beef", ingredientContainerImg = "Beef_Container" });
        allIngredients.Add(new IngredientData { ingredientID = 4, ingredientName = "Bell Pepper", ingredientContainerImg = "Bell_Pepper_Container" });
        allIngredients.Add(new IngredientData { ingredientID = 5, ingredientName = "Breadcrumbs", ingredientContainerImg = "Breadcrumbs_Container" });
        allIngredients.Add(new IngredientData { ingredientID = 6, ingredientName = "Cabbage", ingredientContainerImg = "Cabbage_Container" });
        allIngredients.Add(new IngredientData { ingredientID = 7, ingredientName = "Calamansi", ingredientContainerImg = "Calamansi_Container" });
        allIngredients.Add(new IngredientData { ingredientID = 8, ingredientName = "Chicken", ingredientContainerImg = "Chicken_Container" });
        allIngredients.Add(new IngredientData { ingredientID = 9, ingredientName = "Condensed Milk", ingredientContainerImg = "Condensed_Milk_Container" });
        allIngredients.Add(new IngredientData { ingredientID = 10, ingredientName = "Cooking Oil", ingredientContainerImg = "Cooking_Oil_Container" });
    }

    void UpdatePage()
    {
        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, allIngredients.Count);

        // Hide all slots first EXCEPT the ones we'll update soon
        for (int i = 0; i < slotObjects.Count; i++)
        {
            slotObjects[i].SetActive(i < itemsPerPage); // keep first 6 active
        }


        // Display only the current page’s images
        for (int i = startIndex, slotIndex = 0; i < endIndex; i++, slotIndex++)
        {
            var data = allIngredients[i];
            var slot = slotObjects[slotIndex];
            slot.SetActive(true);

            // If each slot IS an Image:
            var img = slot.GetComponent<Image>();

            // Load the container image from your folder
            Sprite sprite = Resources.Load<Sprite>($"Images/GameMenu/RecipeHunt/{data.ingredientContainerImg}");
            img.sprite = sprite;
        }

        // Arrow visibility
        leftArrow.gameObject.SetActive(currentPage > 0);
        rightArrow.gameObject.SetActive(currentPage < totalPages - 1);

        Debug.Log($"Displaying {endIndex - startIndex} items this page.");
        // After updating visible images, disable the rest if needed
        for (int i = endIndex - startIndex; i < slotObjects.Count; i++)
        {
            slotObjects[i].SetActive(false);
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
