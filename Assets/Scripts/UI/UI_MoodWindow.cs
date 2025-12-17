using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class UI_MoodWindow : MonoBehaviour

{
    [Header("Main Panels")]
    public GameObject moodWindowPanel; // Il Panel sotto MoodWindow
    public GameObject ingredientsPanel; // IngredientsPanel
    
    [Header("Recipe Panels")]
    public GameObject happyRecipesPanel;
    public GameObject angryRecipesPanel;
    public GameObject sadRecipesPanel;
    public GameObject sickRecipesPanel;
    
    [Header("Ingredients Panel Elements")]
    public Transform ingredientsContent; // Il Content dentro IngredientsPanel ScrollView
    public Text ingredientsTitleText; // Titolo del pannello ingredienti
    public Button startButton; // Il bottone "START BUTTON"
    
    [Header("Optional")]
    public GameObject ingredientTextPrefab; // Prefab opzionale per il testo ingrediente
    
    private int selectedMood = -1;
    private string selectedRecipe = "";
    
    // Database ricette CON QUANTITÀ (per visualizzazione)
    private Dictionary<string, List<string>> recipeIngredients = new Dictionary<string, List<string>>()
    {
        // === HAPPY RECIPES ===
        { "Strawberry Milkshake", new List<string> { 
            "• Strawberries", 
            "• Milk" 
        }},
        { "Pastry Cream", new List<string> { 
            "• Eggs", 
            "• Lemon", 
            "• Sugar", 
            "• Corn Starch"
        }},
        
        // === ANGRY RECIPES ===
        { "Garlic Oil and Chilli", new List<string> { 
            "• Pasta", 
            "• Oil", 
            "• Chili Pepper", 
            "• Garlic"
        }}, 
        { "Mac & Cheese", new List<string> { 
            "• Pasta", 
            "• Cheese", 
            "• Milk" 
        }},

        // === SAD RECIPES ===
        { "Hot Chocolate", new List<string> {
            "• Chocolate", 
            "• Milk",
            "• Sugar"
        }},
        { "Mushroom risotto", new List<string> { 
            "• Mushroom", 
            "• Rice",
            "• Spices"
        }},
        
        // === SICK RECIPES ===
        { "Chicken Broth", new List<string> { 
            "• Chicken", 
            "• Carot", 
            "• Water", 
            "• Spices"
        }},
        { "Pumpkin and Chickpea Soup", new List<string> { 
            "• Pumpkin", 
            "• Water", 
            "• Chickpea", 
            "• Spices" 
        }}
    };
    
    void Start()
    {
        // Aggiungi il listener al bottone Start
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
    }
    
    // ========== CHIAMATO DAI BOTTONI MOOD ==========
    // Happy_Button → OnMoodSelected(0)
    // Angry_Button → OnMoodSelected(1)
    // Sad_Button → OnMoodSelected(2)
    // Sick_Button → OnMoodSelected(3)
    public void OnMoodSelected(int moodIndex)
    {
        selectedMood = moodIndex;
        
        Debug.Log($"[MoodWindow] Mood selected: {GetMoodName(moodIndex)}");
        
        // Nascondi il pannello mood principale
        if (moodWindowPanel != null)
            moodWindowPanel.SetActive(false);
        
        // Mostra il pannello ricette corrispondente
        ShowRecipePanel(moodIndex);
    }
    
    void ShowRecipePanel(int moodIndex)
    {
        // Nascondi tutti i pannelli ricette
        HideAllRecipePanels();
        
        // Mostra quello corretto
        switch (moodIndex)
        {
            case 0: // Happy
                if (happyRecipesPanel != null) happyRecipesPanel.SetActive(true);
                break;
            case 1: // Angry
                if (angryRecipesPanel != null) angryRecipesPanel.SetActive(true);
                break;
            case 2: // Sad
                if (sadRecipesPanel != null) sadRecipesPanel.SetActive(true);
                break;
            case 3: // Sick
                if (sickRecipesPanel != null) sickRecipesPanel.SetActive(true);
                break;
        }
    }
    
    // ========== CHIAMATO DAI BOTTONI RICETTE ==========
    // Ogni bottone ricetta chiamerà questo metodo passando il nome della ricetta
    // Es: Recipe_Button → OnRecipeSelected("Tiramisù")
    public void OnRecipeSelected(string recipeName)
    {
        selectedRecipe = recipeName;
        
        Debug.Log($"[MoodWindow] Recipe selected: {recipeName}");
        
        // Informa il GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetMood(selectedMood);
            GameManager.Instance.SetRecipe(selectedRecipe);
        }
        
        // Nascondi tutti i pannelli ricette
        HideAllRecipePanels();
        
        // Mostra il pannello ingredienti con la lista specifica
        ShowIngredientsPanel(recipeName);
    }
    
    void ShowIngredientsPanel(string recipeName)
    {
        // Pulisci eventuali ingredienti precedenti
        if (ingredientsContent != null)
        {
            foreach (Transform child in ingredientsContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        
        // Crea la lista ingredienti dinamicamente
        if (recipeIngredients.ContainsKey(recipeName))
        {
            foreach (string ingredient in recipeIngredients[recipeName])
            {
                CreateIngredientText(ingredient);
            }
        }
        else
        {
            Debug.LogWarning($"[MoodWindow] Recipe '{recipeName}' not found in database!");
        }
        
        // Mostra il pannello
        if (ingredientsPanel != null)
            ingredientsPanel.SetActive(true);
    }
    
    void CreateIngredientText(string ingredientText)
    {
        GameObject ingredientObj;
        
        if (ingredientTextPrefab != null)
        {
            // Usa il prefab se esiste
            ingredientObj = Instantiate(ingredientTextPrefab, ingredientsContent);
        }
        else
        {
            // Crea un Text al volo
            ingredientObj = new GameObject("Ingredient");
            ingredientObj.transform.SetParent(ingredientsContent);
            
            Text text = ingredientObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 9;
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleLeft;
            
            RectTransform rt = ingredientObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, 35);
        }
        
        // Imposta il testo
        Text textComponent = ingredientObj.GetComponent<Text>();
        if (textComponent != null)
            textComponent.text = ingredientText;
    }
    
    // ========== CHIAMATO DAL BOTTONE START ==========
    void OnStartButtonClicked()
    {
        Debug.Log("[MoodWindow] Start button clicked!");
        
        // Chiudi tutti i pannelli
        CloseAllPanels();
        
        // Riprendi il gioco
        ResumeGame();
        
        // Avvia il gameplay di cucina
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartCookingGameplay();
        }
    }
    
    void HideAllRecipePanels()
    {
        if (happyRecipesPanel != null) happyRecipesPanel.SetActive(false);
        if (angryRecipesPanel != null) angryRecipesPanel.SetActive(false);
        if (sadRecipesPanel != null) sadRecipesPanel.SetActive(false);
        if (sickRecipesPanel != null) sickRecipesPanel.SetActive(false);
    }
    
    void CloseAllPanels()
    {
        if (moodWindowPanel != null) moodWindowPanel.SetActive(false);
        HideAllRecipePanels();
        if (ingredientsPanel != null) ingredientsPanel.SetActive(false);
        
        // Nascondi l'intero Canvas se vuoi
        // gameObject.SetActive(false);
    }
    
    void ResumeGame()
    {
        // Riprendi il tempo
        Time.timeScale = 1f;
        
        // Blocca il cursore (se il tuo gioco è first person)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
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
