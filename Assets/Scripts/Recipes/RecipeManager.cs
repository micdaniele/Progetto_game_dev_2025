using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RecipeManager : MonoBehaviour
{
    [Header("Recipe Selection")]
    public string currentRecipeName;
    private List<string> requiredIngredients = new List<string>();
    private RecipeDatabase currentDatabase;

    [Header("Ingredient Tracking")]
    private List<string> selectedIngredients = new List<string>();

    // Riferimento a tutti gli ingredienti nella scena
    private Ingredient[] allIngredients;

    void Start()
    {
        // === 1. RISOLUZIONE PROBLEMA MOUSE (Così non devi premere ESC) ===
        Cursor.lockState = CursorLockMode.None; // Sblocca il cursore dal centro
        Cursor.visible = true;                  // Lo rende visibile
        Time.timeScale = 1f;                    // Assicura che il gioco non sia in pausa

        // === 2. INIZIALIZZAZIONE LOGICA ===
        // Trova tutti gli ingredienti (bottoni) nella scena
        allIngredients = Object.FindObjectsByType<Ingredient>(FindObjectsSortMode.None);

        Debug.Log($"[RecipeManager] Trovati {allIngredients.Length} ingredienti nella scena");

        // Carica la ricetta che hai scelto in cucina
        LoadSavedSelection();

        // === 3. SINCRONIZZAZIONE (Per non perdere gli oggetti già presi) ===
        if (GameManager.Instance != null)
        {
            // Copia la lista dallo "zaino" globale del GameManager alla lista locale
            selectedIngredients = new List<string>(GameManager.Instance.ingredientiPresi);
        }

        // === 4. AGGIORNAMENTO GRAFICO ===
        // Colora di verde quelli da prendere e di giallo quelli già presi
        UpdateIngredientsState();
    }

    // Carica mood e ricetta salvati dal GameManager
    private void LoadSavedSelection()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[RecipeManager] GameManager.Instance e NULL! Assicurati che ci sia un GameManager nella scena precedente.");
            return;
        }

        if (GameManager.Instance.HasValidSelection())
        {
            int savedMood = GameManager.Instance.GetCurrentMood();
            string savedRecipe = GameManager.Instance.GetCurrentRecipe();

            Debug.Log($"[RecipeManager] Carico selezione salvata: {GameManager.Instance.GetMoodName(savedMood)} ({savedMood}) - {savedRecipe}");

            SetMood(savedMood);
            SelectRecipe(savedRecipe);

            // DEBUG: Stampa lo stato degli ingredienti
            PrintIngredientsState();
        }
        else
        {
            Debug.LogWarning("[RecipeManager] Nessuna selezione salvata trovata!");
        }
    }

    // DEBUG: Stampa lo stato di tutti gli ingredienti
    private void PrintIngredientsState()
    {
        Debug.Log("=== STATO INGREDIENTI ===");
        foreach (var ingredient in allIngredients)
        {
            bool isRequired = IsIngredientRequired(ingredient.ingredientName);
            Debug.Log($"{ingredient.ingredientName}: {(isRequired ? "[OK] SELEZIONABILE" : "[X] DISABILITATO")}");
        }
        Debug.Log("========================");
    }

    // Imposta il mood usando l'int (0=Happy, 1=Angry, 2=Sad, 3=Sick)
    public void SetMood(int mood)
    {
        switch (mood)
        {
            case 0: // Happy
                currentDatabase = new HappyRecipes();
                break;
            case 1: // Angry
                currentDatabase = new AngryRecipes();
                break;
            case 2: // Sad
                currentDatabase = new SadRecipes();
                break;
            case 3: // Sick
                currentDatabase = new SickRecipes();
                break;
            default:
                Debug.LogError("Mood non riconosciuto!");
                return;
        }

        Debug.Log(currentDatabase.GetMoodDescription());
    }

    // Seleziona una ricetta specifica
    public void SelectRecipe(string recipeName)
    {
        if (currentDatabase == null)
        {
            Debug.LogError("Nessun database selezionato!");
            return;
        }

        var recipes = currentDatabase.GetRecipes();

        if (recipes.ContainsKey(recipeName))
        {
            currentRecipeName = recipeName;
            requiredIngredients = recipes[recipeName];
            selectedIngredients.Clear();

            // Aggiorna lo stato di tutti gli ingredienti
            UpdateIngredientsState();

            Debug.Log($"[RecipeManager] Ricetta selezionata: {recipeName}");
            Debug.Log($"[RecipeManager] Ingredienti richiesti: {string.Join(", ", requiredIngredients)}");
        }
        else
        {
            Debug.LogError($"Ricetta '{recipeName}' non trovata!");
        }
    }

    // Verifica se un ingrediente è richiesto per la ricetta corrente
    public bool IsIngredientRequired(string ingredientName)
    {
        if (string.IsNullOrEmpty(currentRecipeName))
            return false;

        // Puliamo il nome dell'ingrediente cliccato
        string cleanInput = ingredientName.Trim().ToLower();

        return requiredIngredients.Any(req => {
            // Rimuoviamo il punto elenco, underscore e spazi extra dal database
            string cleanReq = req.Replace("-", "").Replace("_", "").Trim().ToLower();

            Debug.Log($"[RecipeManager] Confronto: '{cleanReq}' vs '{cleanInput}'");
            return cleanReq == cleanInput;
        });
    }

    public bool TrySelectIngredient(string ingredientName)
    {
        if (IsIngredientRequired(ingredientName))
        {
            if (!selectedIngredients.Contains(ingredientName))
            {
                selectedIngredients.Add(ingredientName);

                // SALVA SUBITO NEL GAMEMANAGER!
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ingredientiPresi.Add(ingredientName);
                }

                Debug.Log($"[OK] Preso: {ingredientName}");
                CheckRecipeCompletion();
                return true;
            }
        }
        return false;
    }

    // Aggiorna lo stato di tutti gli ingredienti (abilitati/disabilitati)
    private void UpdateIngredientsState()
    {
        Debug.Log("[RecipeManager] Aggiornamento stato ingredienti...");

        foreach (var ingredient in allIngredients)
        {
            bool isRequired = IsIngredientRequired(ingredient.ingredientName);
            ingredient.SetSelectable(isRequired);

            Debug.Log($"[RecipeManager] {ingredient.ingredientName} -> {(isRequired ? "VERDE" : "GRIGIO")}");
        }
    }

    // Controlla se tutti gli ingredienti sono stati raccolti
    private void CheckRecipeCompletion()
    {
        int requiredCount = requiredIngredients.Count;

        if (selectedIngredients.Count == requiredCount)
        {
            Debug.Log("*** RICETTA COMPLETATA! ***");
            OnRecipeCompleted();
        }
        else
        {
            Debug.Log($"[Progresso] {selectedIngredients.Count}/{requiredCount} ingredienti");
        }
    }

    // Chiamato quando la ricetta è completata
    private void OnRecipeCompleted()
    {
        // TODO aggiungere logica per
        // - Mostrare una UI di completamento
        // - Dare ingredienti al giocatore
        // - Cambiare scena con il memory

        Debug.Log("*** Complimenti! Hai completato la ricetta! ***");
    }

    // Resetta la selezione corrente
    public void ResetSelection()
    {
        selectedIngredients.Clear();
        currentRecipeName = "";
        requiredIngredients.Clear();

        // Disabilita tutti gli ingredienti
        foreach (var ingredient in allIngredients)
        {
            ingredient.SetSelectable(false);
        }
    }

    // Metodi helper per UI
    public List<string> GetAvailableRecipes()
    {
        if (currentDatabase == null) return new List<string>();
        return new List<string>(currentDatabase.GetRecipes().Keys);
    }

    public string GetCurrentRecipeName()
    {
        return currentRecipeName;
    }

    public List<string> GetRequiredIngredients()
    {
        return new List<string>(requiredIngredients);
    }

    public List<string> GetSelectedIngredients()
    {
        return new List<string>(selectedIngredients);
    }
}