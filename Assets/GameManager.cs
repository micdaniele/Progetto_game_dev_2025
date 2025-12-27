using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    private string currentRecipe = "";
    private int currentMood = -1;
    private bool cookingStarted = false;

    [Header("Ingredients")]
    private List<string> ingredientsToCollect = new List<string>();
    private List<string> collectedIngredients = new List<string>();

    void Awake()
    {
        // Singleton pattern con DontDestroyOnLoad per persistenza tra scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Questo mantiene l'oggetto tra le scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Chiamato dal MoodWindow quando viene selezionato un mood
    public void SetMood(int mood)
    {
        currentMood = mood;
        Debug.Log($"[GameManager] Mood salvato: {GetMoodName(mood)} ({mood})");
    }

    // Metodo per preparare le ricette dopo la selezione del mood
    public void PrepareRecipes()
    {
        Debug.Log($"[GameManager] Preparing recipes for mood: {GetMoodName(currentMood)}");
    }

    // Chiamato dal MoodWindow quando viene selezionata una ricetta
    public void SetRecipe(string recipe)
    {
        currentRecipe = recipe;
        Debug.Log($" [GameManager] Ricetta salvata: {recipe}");

        // Imposta gli ingredienti da raccogliere
        SetIngredientsForRecipe(recipe);
    }

    void SetIngredientsForRecipe(string recipe)
    {
        ingredientsToCollect.Clear();
        collectedIngredients.Clear();

        // Database ingredienti (nomi base senza quantità)
        Dictionary<string, List<string>> recipeIngredients = new Dictionary<string, List<string>>()
        {
            // HAPPY RECIPES (mood 0)
            { "Strawberry Milkshake", new List<string> { "Strawberries", "Milk" }},
            { "Pastry Cream", new List<string> { "Eggs", "Lemon", "Sugar", "Corn Starch" }},
            
            // ANGRY RECIPES (mood 1)
            { "Garlic Oil and Chilli", new List<string> { "Pasta", "Oil", "Chili Pepper", "Garlic" }},
            { "Mac & Cheese", new List<string> { "Pasta", "Cheese", "Milk"}},
            
            // SAD RECIPES (mood 2)
            { "Hot Chocolate", new List<string> { "Chocolate", "Milk", "Sugar" }},
            { "Mushroom Risotto", new List<string> { "Mushroom", "Rice", "Spices" }},
            
            // SICK RECIPES (mood 3)
            { "Chicken Broth", new List<string> { "Chicken", "Carrot", "Water", "Spices" }},
            { "Pumpkin and Chickpea Soup", new List<string> { "Pumpkin", "Water", "Chickpea", "Spices" }}
        };

        if (recipeIngredients.ContainsKey(recipe))
        {
            ingredientsToCollect = new List<string>(recipeIngredients[recipe]);
            Debug.Log($"[GameManager] Ingredients to collect: {ingredientsToCollect.Count}");
        }
    }

    // Chiamato quando l'utente clicca "Start Button" nel pannello ingredienti
    public void StartCookingGameplay()
    {
        cookingStarted = true;

        Debug.Log("=== COOKING STARTED ===");
        Debug.Log($"Recipe: {currentRecipe}");
        Debug.Log($"Mood: {GetMoodName(currentMood)}");
        Debug.Log($"Ingredients needed: {ingredientsToCollect.Count}");

        SpawnIngredients();
    }

    void SpawnIngredients()
    {
        foreach (string ingredient in ingredientsToCollect)
        {
            Debug.Log($"[GameManager] Should spawn ingredient: {ingredient}");
        }
    }

    // Chiamato quando il player raccoglie un ingrediente
    public void CollectIngredient(string ingredientName)
    {
        if (ingredientsToCollect.Contains(ingredientName) && !collectedIngredients.Contains(ingredientName))
        {
            collectedIngredients.Add(ingredientName);
            Debug.Log($"[GameManager] Collected: {ingredientName} ({collectedIngredients.Count}/{ingredientsToCollect.Count})");

            if (collectedIngredients.Count >= ingredientsToCollect.Count)
            {
                OnAllIngredientsCollected();
            }
        }
    }

    void OnAllIngredientsCollected()
    {
        Debug.Log("=== ALL INGREDIENTS COLLECTED! ===");
        // TODO: Passa alla fase di cucina
    }

    // ============ GETTER METHODS ============

    public string GetCurrentRecipe() { return currentRecipe; }
    public int GetCurrentMood() { return currentMood; }
    public bool IsCookingStarted() { return cookingStarted; }
    public List<string> GetIngredientsToCollect() { return new List<string>(ingredientsToCollect); }
    public List<string> GetCollectedIngredients() { return new List<string>(collectedIngredients); }

    // Verifica se ci sono dati validi salvati
    public bool HasValidSelection()
    {
        return currentMood >= 0 && !string.IsNullOrEmpty(currentRecipe);
    }

    //Resetta le selezioni
    public void ResetSelections()
    {
        currentMood = -1;
        currentRecipe = "";
        ingredientsToCollect.Clear();
        collectedIngredients.Clear();
        cookingStarted = false;
        Debug.Log("🔄 Selezioni resettate");
    }

    // ============ UTILITY ============

    public string GetMoodName(int mood)
    {
        switch (mood)
        {
            case 0: return "Happy";
            case 1: return "Angry";
            case 2: return "Sad";
            case 3: return "Sick";
            default: return "Unknown";
        }
    }

    // Ottieni l'indice del mood dal nome 
    public int GetMoodIndex(string moodName)
    {
        switch (moodName.ToLower())
        {
            case "happy": return 0;
            case "angry": return 1;
            case "sad": return 2;
            case "sick": return 3;
            default: return -1;
        }
    }
}