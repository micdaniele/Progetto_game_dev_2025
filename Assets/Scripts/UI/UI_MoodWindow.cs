using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    [Header("Dialogues")]
    public Dialogue recipeSelectedDialogue;

    [Header("GameObject Switching")]
    public GameObject currentMoodInteraction;  // Il GameObject da disattivare
    public GameObject nextDialogueTrigger;     // Il GameObject con DialogueTrigger da attivare
    public string moodInteractionID = "MoodInteraction_Main";  // ID univoco per salvare lo stato

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

        // ripristina lo stato salvato quando torni nella scena
        RestoreGameObjectStates();
    }

    // chiamato dai mood buttons 
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

    // chiamato dai recipe buttons 
    public void OnRecipeSelected(string recipeName)
    {
        selectedRecipe = recipeName;
        Debug.Log($"[MoodWindow] Hai scelto: {recipeName}");

        // salva nel gamemanager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSelection(selectedMood, recipeName);
        }
        else
        {
            Debug.LogError("ERRORE: GameManager non trovato nella scena!");
        }

        // 2. Nascondi i pannelli delle ricette
        HideAllRecipePanels();

        // 3. Mostra ingredienti
        ShowIngredientsPanel(recipeName);

        // 4. Mostra il dialogo dopo aver scelto la ricetta
        StartCoroutine(ShowDialogueAfterPanelClose());
    }

    IEnumerator ShowDialogueAfterPanelClose()
    {
        // Aspetta un frame per assicurarsi che i pannelli siano chiusi
        yield return null;

        // Mostra il dialogo
        if (DialogueManager.Instance != null && recipeSelectedDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(recipeSelectedDialogue);
        }
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

        // Usa il metodo GetRecipes() della classe figlia
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

        // Sblocca il gioco 
        ResumeGame();

        // Disattiva questo GameObject e attiva il DialogueTrigger
        SwitchToNextDialogue();
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

    void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Cambia GameObject e SALVA lo stato usando SaveUIObjectState
    void SwitchToNextDialogue()
    {
        // Attiva il GameObject con DialogueTrigger
        if (nextDialogueTrigger != null)
        {
            nextDialogueTrigger.SetActive(true);
            Debug.Log($"[MoodWindow] Attivato DialogueTrigger: {nextDialogueTrigger.name}");

            // Salva che il DialogueTrigger è attivo usando SaveUIObjectState
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SaveUIObjectState(moodInteractionID + "_NextDialogue", true);
            }
        }
        else
        {
            Debug.LogWarning("[MoodWindow] nextDialogueTrigger non assegnato nell'Inspector!");
        }

        // Disattiva il GameObject corrente (MoodInteraction)
        if (currentMoodInteraction != null)
        {
            currentMoodInteraction.SetActive(false);
            Debug.Log($"[MoodWindow] Disattivato MoodInteraction: {currentMoodInteraction.name}");

            // Salva che il MoodInteraction è disattivo
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SaveUIObjectState(moodInteractionID, false);
            }
        }
        else
        {
            Debug.LogWarning("[MoodWindow] currentMoodInteraction non assegnato nell'Inspector!");
        }
    }

    //Ripristina lo stato dei GameObject quando torni nella scena
    void RestoreGameObjectStates()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[MoodWindow] GameManager non trovato, impossibile ripristinare stati");
            return;
        }

        // Controlla se il MoodInteraction deve essere disattivo
        bool moodInteractionActive = GameManager.Instance.GetUIObjectState(moodInteractionID, true);
        if (currentMoodInteraction != null)
        {
            currentMoodInteraction.SetActive(moodInteractionActive);
            Debug.Log($"[MoodWindow] Ripristinato stato MoodInteraction: {moodInteractionActive}");
        }

        // Controlla se il DialogueTrigger deve essere attivo
        bool nextDialogueActive = GameManager.Instance.GetUIObjectState(moodInteractionID + "_NextDialogue", false);
        if (nextDialogueTrigger != null)
        {
            nextDialogueTrigger.SetActive(nextDialogueActive);
            Debug.Log($"[MoodWindow] Ripristinato stato NextDialogue: {nextDialogueActive}");
        }
    }
}