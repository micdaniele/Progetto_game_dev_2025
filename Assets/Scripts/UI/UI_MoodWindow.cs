using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class UI_MoodWindow : MonoBehaviour

{
    [Header("Main Panels")]
    public GameObject moodWindowPanel;
    public GameObject ingredientsPanel;
    
    [Header("Recipe Panels")]
    public GameObject happyRecipesPanel;
    public GameObject angryRecipesPanel;
    public GameObject sadRecipesPanel;
    public GameObject sickRecipesPanel;
    
    [Header("Ingredients Panel Elements")]
    public Transform ingredientsContent;
    public Text ingredientsTitleText;
    public Button startButton;
    
    [Header("Optional")]
    public GameObject ingredientTextPrefab;
    
    private int selectedMood = -1;
    private string selectedRecipe = "";
    
    // POLIMORFISMO 
    // Array di oggetti della classe BASE che contiene oggetti delle classi FIGLIE
    private RecipeDatabase[] recipeDatabases;
    
    void Start()
    {
        // Inizializza l'array con le classi FIGLIE - POLIMORFISMO IN AZIONE!
        recipeDatabases = new RecipeDatabase[]
        {
            new HappyRecipes(),   // Classe figlia 1
            new AngryRecipes(),   // Classe figlia 2
            new SadRecipes(),     // Classe figlia 3
            new SickRecipes()     // Classe figlia 4
        };
        
        // Log per dimostrare il polimorfismo
        foreach (RecipeDatabase db in recipeDatabases)
        {
            Debug.Log(db.GetMoodDescription()); // Chiama il metodo sovrascritto!
        }
        
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
    }
    
    // CHIAMATO DAI BOTTONI MOOD 
    public void OnMoodSelected(int moodIndex)
    {
        selectedMood = moodIndex;
        
        // POLIMORFISMO: usa la classe base per accedere alle classi figlie
        if (moodIndex >= 0 && moodIndex < recipeDatabases.Length)
        {
            RecipeDatabase selectedDatabase = recipeDatabases[moodIndex];
            Debug.Log($"[MoodWindow] Selected: {selectedDatabase.GetMoodDescription()}");
        }
        
        if (moodWindowPanel != null)
            moodWindowPanel.SetActive(false);
        
        ShowRecipePanel(moodIndex);
    }
    
    void ShowRecipePanel(int moodIndex)
    {
        HideAllRecipePanels();
        
        switch (moodIndex)
        {
            case 0: if (happyRecipesPanel != null) happyRecipesPanel.SetActive(true); break;
            case 1: if (angryRecipesPanel != null) angryRecipesPanel.SetActive(true); break;
            case 2: if (sadRecipesPanel != null) sadRecipesPanel.SetActive(true); break;
            case 3: if (sickRecipesPanel != null) sickRecipesPanel.SetActive(true); break;
        }
    }
    
    // CHIAMATO DAI BOTTONI RICETTE 
    public void OnRecipeSelected(string recipeName)
    {
        selectedRecipe = recipeName;
        Debug.Log($"[MoodWindow] Recipe selected: {recipeName}");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetMood(selectedMood);
            GameManager.Instance.SetRecipe(selectedRecipe);
        }
        
        HideAllRecipePanels();
        ShowIngredientsPanel(recipeName);
    }
    
    void ShowIngredientsPanel(string recipeName)
    {
        // Pulisci ingredienti precedenti
        if (ingredientsContent != null)
        {
            foreach (Transform child in ingredientsContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        // POLIMORFISMO: usa il metodo GetRecipes() della classe figlia corretta
        if (selectedMood >= 0 && selectedMood < recipeDatabases.Length)
        {
            Dictionary<string, List<string>> recipes = recipeDatabases[selectedMood].GetRecipes();
            
            if (recipes.ContainsKey(recipeName))
            {
                List<string> ingredients = recipes[recipeName];
                
                foreach (string ingredient in ingredients)
                {
                    CreateIngredientText(ingredient);
                }
            }
            else
            {
                Debug.LogWarning($"[MoodWindow] Recipe '{recipeName}' not found!");
            }
        }
        
        if (ingredientsPanel != null)
            ingredientsPanel.SetActive(true);
    }
    
    void CreateIngredientText(string ingredientText)
    {
        GameObject ingredientObj;
        
        if (ingredientTextPrefab != null)
        {
            ingredientObj = Instantiate(ingredientTextPrefab, ingredientsContent);
        }
        else
        {
            ingredientObj = new GameObject("Ingredient");
            ingredientObj.transform.SetParent(ingredientsContent);
            
            Text text = ingredientObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 18;
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleLeft;
            
            RectTransform rt = ingredientObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, 30);
        }
        
        Text textComponent = ingredientObj.GetComponent<Text>();
        if (textComponent != null)
            textComponent.text = ingredientText;
    }
    
    void OnStartButtonClicked()
    {
        Debug.Log("[MoodWindow] Start button clicked!");
        
        CloseAllPanels();
        ResumeGame();
        
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
    }
    
    void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}