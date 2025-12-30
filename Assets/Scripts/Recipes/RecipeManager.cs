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

    [Header("Minigame Settings")]
    public string fridgeMinigameScene = "Fridge";
    public string pantryMinigameScene = "MemoryGame";

    // Riferimento a tutti gli ingredienti nella scena
    private Ingredient[] allIngredients;

    void Start()
    {
        // Mouse libero
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        // Trova SOLO gli ingredienti fisicamente presenti in QUESTA scena
        allIngredients = Object.FindObjectsByType<Ingredient>(FindObjectsSortMode.None);
        Debug.Log($"[RecipeManager] Trovati {allIngredients.Length} ingredienti FISICI nella scena");

        // Carica la ricetta scelta
        LoadSavedSelection();

        // Sincronizza con gli ingredienti gi� presi
        if (GameManager.Instance != null)
        {
            selectedIngredients = new List<string>(GameManager.Instance.ingredientiPresi);
            Debug.Log($"[RecipeManager] === ZAINO ===");
            Debug.Log($"[RecipeManager] Ingredienti gi� presi: {selectedIngredients.Count}");
            foreach (string ing in selectedIngredients)
            {
                Debug.Log($"[RecipeManager]   - {ing}");
            }
            Debug.Log($"[RecipeManager] ===============");
        }

        // Aggiorna i colori
        UpdateIngredientsState();
    }

    private void LoadSavedSelection()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[RecipeManager] GameManager.Instance e NULL!");
            return;
        }

        if (GameManager.Instance.HasValidSelection())
        {
            int savedMood = GameManager.Instance.GetCurrentMood();
            string savedRecipe = GameManager.Instance.GetCurrentRecipe();

            Debug.Log($"[RecipeManager] Ricetta caricata: {savedRecipe}");

            SetMood(savedMood);
            SelectRecipe(savedRecipe);
        }
        else
        {
            Debug.LogWarning("[RecipeManager] Nessuna selezione salvata!");
        }
    }

    public void SetMood(int mood)
    {
        switch (mood)
        {
            case 0: currentDatabase = new HappyRecipes(); break;
            case 1: currentDatabase = new AngryRecipes(); break;
            case 2: currentDatabase = new SadRecipes(); break;
            case 3: currentDatabase = new SickRecipes(); break;
            default:
                Debug.LogError("Mood non riconosciuto!");
                return;
        }

        Debug.Log($"[RecipeManager] Mood: {currentDatabase.GetMoodDescription()}");
    }

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

            Debug.Log($"[RecipeManager] Ricetta: {recipeName}");
            Debug.Log($"[RecipeManager] Ingredienti totali richiesti: {string.Join(", ", requiredIngredients)}");
        }
        else
        {
            Debug.LogError($"Ricetta '{recipeName}' non trovata!");
        }
    }

    public bool IsIngredientRequired(string ingredientName)
    {
        if (string.IsNullOrEmpty(currentRecipeName))
            return false;

        string cleanInput = ingredientName.Trim().ToLower();

        // Controlla se � nella lista della ricetta
        bool inRecipe = requiredIngredients.Any(req => {
            string cleanReq = req.Replace("-", "").Replace("_", "").Trim().ToLower();
            return cleanReq == cleanInput;
        });

        // Se � nella ricetta, verifica anche che NON sia gi� stato preso
        if (inRecipe)
        {
            bool alreadyTaken = selectedIngredients.Contains(ingredientName);
            return !alreadyTaken; // Selezionabile SOLO se non � gi� stato preso
        }

        return false;
    }

    public bool TrySelectIngredient(string ingredientName)
    {
        if (IsIngredientRequired(ingredientName))
        {
            if (!selectedIngredients.Contains(ingredientName))
            {
                selectedIngredients.Add(ingredientName);

                // Salva subito nel GameManager
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ingredientiPresi.Add(ingredientName);
                }

                // Trova l'ingrediente nella scena e marcalo come "selezionato"
                Ingredient ing = System.Array.Find(allIngredients, i => i.ingredientName == ingredientName);
                if (ing != null)
                {
                    ing.SetSelected(true);
                }

                Debug.Log($"[OK] Preso: {ingredientName} ({selectedIngredients.Count}/{GetRequiredIngredientsInScene().Count})");
                CheckSceneCompletion();
                return true;
            }
        }
        else
        {
            Debug.Log($"[X] {ingredientName} non serve o e gia stato preso!");
        }
        return false;
    }

    private List<string> GetRequiredIngredientsInScene()
    {
        List<string> inSceneRequired = new List<string>();

        foreach (var ingredient in allIngredients)
        {
            string cleanName = ingredient.ingredientName.Trim().ToLower();

            bool isRequired = requiredIngredients.Any(req => {
                string cleanReq = req.Replace("-", "").Replace("_", "").Trim().ToLower();
                return cleanReq == cleanName;
            });

            if (isRequired && !inSceneRequired.Contains(ingredient.ingredientName))
            {
                inSceneRequired.Add(ingredient.ingredientName);
            }
        }

        return inSceneRequired;
    }

    private void UpdateIngredientsState()
    {
        Debug.Log("[RecipeManager] Aggiornamento colori...");

        foreach (var ingredient in allIngredients)
        {
            bool alreadySelected = selectedIngredients.Contains(ingredient.ingredientName);

            if (alreadySelected)
            {
                // GIA PRESO = GIALLO (non cliccabile)
                ingredient.SetSelected(true);
                Debug.Log($"[RecipeManager] {ingredient.ingredientName} -> GIA PRESO (giallo)");
            }
            else
            {
                // NON ANCORA PRESO
                bool isRequired = IsIngredientRequired(ingredient.ingredientName);
                ingredient.SetSelectable(isRequired);
                Debug.Log($"[RecipeManager] {ingredient.ingredientName} -> {(isRequired ? "VERDE (da prendere)" : "GRIGIO (non serve)")}");
            }
        }

        // Stampa ingredienti presenti nella scena e richiesti
        var inSceneRequired = GetRequiredIngredientsInScene();
        Debug.Log($"[RecipeManager] Ingredienti richiesti IN QUESTA SCENA: {string.Join(", ", inSceneRequired)}");
    }

    private void CheckSceneCompletion()
    {
        // Conta gli ingredienti richiesti in questa scena
        var requiredInScene = GetRequiredIngredientsInScene();

        // Conta quanti ne hai presi
        int takenCount = 0;
        foreach (string required in requiredInScene)
        {
            if (selectedIngredients.Contains(required))
            {
                takenCount++;
            }
        }

        Debug.Log($"[Progresso] {takenCount}/{requiredInScene.Count} ingredienti in questa scena");

        // Se hai preso tutto quello che c'era qui, determina quale minigioco aprire
        if (requiredInScene.Count > 0 && takenCount >= requiredInScene.Count)
        {
            Debug.Log("*** TUTTI GLI INGREDIENTI DI QUESTA SCENA RACCOLTI! ***");
            DetermineAndLoadMinigame();
        }
    }

    // Determina quale minigioco caricare in base alla scena corrente
    private void DetermineAndLoadMinigame()
    {
        // Ottieni il nome della scena corrente
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        Debug.Log($"[RecipeManager] Scena corrente: {currentSceneName}");

        // Determina quale minigioco caricare
        string minigameToLoad = "";

        // Controlla il nome della scena (puoi personalizzare questi nomi)
        if (currentSceneName.ToLower().Contains("fridge"))
        {
            minigameToLoad = fridgeMinigameScene;
            Debug.Log($"[RecipeManager] Rilevato FRIGO -> Carico {minigameToLoad}");
        }
        else if (currentSceneName.ToLower().Contains("pantry"))
        {
            minigameToLoad = pantryMinigameScene;
            Debug.Log($"[RecipeManager] Rilevato DISPENSA -> Carico {minigameToLoad}");
        }
        else
        {
            Debug.LogWarning($"[RecipeManager] Scena '{currentSceneName}' non riconosciuta! Carico minigioco di default.");
            minigameToLoad = fridgeMinigameScene; // Default
        }

        // Carica il minigioco dopo un breve delay
        Invoke("LoadMinigame", 1.5f);

        // Salva quale minigioco caricare (per usarlo in LoadMinigame)
        nextMinigameScene = minigameToLoad;
    }

    // Variabile temporanea per salvare quale scena caricare
    private string nextMinigameScene = "";

    private void LoadMinigame()
    {
        if (!string.IsNullOrEmpty(nextMinigameScene))
        {
            Debug.Log($"[RecipeManager] Caricamento minigioco: {nextMinigameScene}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextMinigameScene);
        }
        else
        {
            Debug.LogError("[RecipeManager] Errore: nessun minigioco da caricare!");
        }
    }

    public void ResetSelection()
    {
        selectedIngredients.Clear();
        currentRecipeName = "";
        requiredIngredients.Clear();

        foreach (var ingredient in allIngredients)
        {
            ingredient.SetSelectable(false);
        }
    }

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