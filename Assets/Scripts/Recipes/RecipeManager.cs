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
        // Trova tutti gli ingredienti nella scena
        allIngredients = Object.FindObjectsByType<Ingredient>(FindObjectsSortMode.None);

        //CARICA I DATI SALVATI DAL GAMEMANAGER
        LoadSavedSelection();
    }

    // Carica mood e ricetta salvati dal GameManager
    private void LoadSavedSelection()
    {
        if (GameManager.Instance.HasValidSelection())
        {
            int savedMood = GameManager.Instance.GetCurrentMood();
            string savedRecipe = GameManager.Instance.GetCurrentRecipe();

            Debug.Log($"Carico selezione salvata: {GameManager.Instance.GetMoodName(savedMood)} ({savedMood}) - {savedRecipe}");

            SetMood(savedMood);
            SelectRecipe(savedRecipe);
        }
        else
        {
            Debug.LogWarning("Nessuna selezione salvata trovata!");
        }
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

            Debug.Log($"Ricetta selezionata: {recipeName}");
            Debug.Log($"Ingredienti richiesti: {string.Join(", ", requiredIngredients)}");
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
            // Rimuoviamo il punto elenco '•', underscore e spazi extra dal database
            string cleanReq = req.Replace("•", "")
                                 .Replace("_", "")
                                 .Trim()
                                 .ToLower();
            return cleanReq == cleanInput;
        });
    }

    // Chiamato quando un ingrediente viene cliccato
    public bool TrySelectIngredient(string ingredientName)
    {
        if (IsIngredientRequired(ingredientName))
        {
            if (!selectedIngredients.Contains(ingredientName))
            {
                selectedIngredients.Add(ingredientName);
                Debug.Log($"Ingrediente aggiunto: {ingredientName}");

                // Controlla se la ricetta è completa
                CheckRecipeCompletion();
                return true;
            }
        }
        else
        {
            Debug.Log($"Ingrediente '{ingredientName}' non necessario per questa ricetta!");
            return false;
        }

        return false;
    }

    // Aggiorna lo stato di tutti gli ingredienti (abilitati/disabilitati)
    private void UpdateIngredientsState()
    {
        foreach (var ingredient in allIngredients)
        {
            bool isRequired = IsIngredientRequired(ingredient.ingredientName);
            ingredient.SetSelectable(isRequired);
        }
    }

    // Controlla se tutti gli ingredienti sono stati raccolti
    private void CheckRecipeCompletion()
    {
        int requiredCount = requiredIngredients.Count;

        if (selectedIngredients.Count == requiredCount)
        {
            Debug.Log(" RICETTA COMPLETATA!");
            OnRecipeCompleted();
        }
        else
        {
            Debug.Log($"Progresso: {selectedIngredients.Count}/{requiredCount} ingredienti");
        }
    }

    // Chiamato quando la ricetta è completata
    private void OnRecipeCompleted()
    {
        // Qui puoi aggiungere logica per:
        // - Mostrare una UI di completamento
        // - Dare ricompense al giocatore
        // - Sbloccare nuove ricette
        // - etc.
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