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
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        Debug.Log($"[GameManager] Mood set to: {GetMoodName(mood)}");
    }

    // Metodo aggiunto per preparare le ricette dopo la selezione del mood
    public void PrepareRecipes()
    {
        Debug.Log($"[GameManager] Preparing recipes for mood: {GetMoodName(currentMood)}");
        // TODO aggiungere logica per filtrare/preparare le ricette in base al mood
    }

    // Chiamato dal MoodWindow quando viene selezionata una ricetta
    public void SetRecipe(string recipe)
    {
        currentRecipe = recipe;
        Debug.Log($"[GameManager] Recipe selected: {recipe}");

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
            // HAPPY RECIPES
            { "Strawberry Milkshake", new List<string> { "Strawberries", "Milk" }},
            { "Pastry Cream", new List<string> { "Eggs", "Lemon", "Sugar", "Corn Starch" }},
            
            // ANGRY RECIPES
            { "Garlic Oil and Chilli", new List<string> { "Pasta", "Oil", "Chili Pepper", "Garlic" }},
            { "Mac & Cheese", new List<string> { "Pasta", "Cheese", "Milk"}},
            
            // SAD RECIPES
            { "Hot Chocolate", new List<string> { "Chocolate", "Milk", "Sugar" }},
            { "Mushroom Risotto", new List<string> { "Mushroom", "Rice", "Spices" }},
            
            // SICK RECIPES
            { "Chicken Broth", new List<string> { "Chicken", "Carot", "Spices" }},
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

        // TODO: Qui puoi spawnare gli ingredienti nella scena
        SpawnIngredients();
    }

    void SpawnIngredients()
    {
        foreach (string ingredient in ingredientsToCollect)
        {
            Debug.Log($"[GameManager] Should spawn ingredient: {ingredient}");
            // TODO: Instantiate(ingredientPrefab, position, rotation);
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

    // Getter methods
    public string GetCurrentRecipe() { return currentRecipe; }
    public int GetCurrentMood() { return currentMood; }
    public bool IsCookingStarted() { return cookingStarted; }
    public List<string> GetIngredientsToCollect() { return new List<string>(ingredientsToCollect); }
    public List<string> GetCollectedIngredients() { return new List<string>(collectedIngredients); }

    string GetMoodName(int mood)
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
}