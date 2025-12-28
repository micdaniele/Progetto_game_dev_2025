using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

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
    public TextMeshProUGUI ingredientsTitleText;
    public GameObject fridgePrompt;
    public GameObject pantryPrompt;

    private int selectedMood = -1;
    private string selectedRecipe = "";

    // POLIMORFISMO 
    private RecipeDatabase[] recipeDatabases;

    void Start()
    {
        // Inizializza l'array con le classi FIGLIE
        recipeDatabases = new RecipeDatabase[]
        {
            new HappyRecipes(),
            new AngryRecipes(),
            new SadRecipes(),
            new SickRecipes()
        };

        // Log per dimostrare il polimorfismo
        foreach (RecipeDatabase db in recipeDatabases)
        {
            Debug.Log(db.GetMoodDescription());
        }
    }

    // CHIAMATO DAI BOTTONI MOOD 
    public void OnMoodSelected(int moodIndex)
    {
        selectedMood = moodIndex;

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
        Debug.Log($"[MoodWindow] Hai scelto: {recipeName}");

        // 1. SALVA NEL GAMEMANAGER
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSelection(selectedMood, recipeName);
        }
        else
        {
            Debug.LogError("ERRORE: GameManager non trovato nella scena!");
        }

        // 2. Aggiorna la UI (mostra ingredienti necessari nel pannello)
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

        // Mostra i prompt
        if (fridgePrompt != null)
            fridgePrompt.SetActive(true);

        if (pantryPrompt != null)
            pantryPrompt.SetActive(true);

        // Sblocca il gioco 
        ResumeGame();
    }

    void CreateIngredientText(string ingredientText)
    {
        GameObject ingredientObj = new GameObject("Ingredient");
        ingredientObj.transform.SetParent(ingredientsContent);

        Text text = ingredientObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 16;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleLeft;

        RectTransform rt = ingredientObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 30);

        text.text = ingredientText;
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
        if (fridgePrompt != null) fridgePrompt.SetActive(false);
        if (pantryPrompt != null) pantryPrompt.SetActive(false);
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}