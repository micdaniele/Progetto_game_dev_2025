using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FridgeDefrostGame : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public Text timerText;
    public GameObject minigamePanel;
    public GameObject gameCompletePanel;

    [Header("Impostazioni Minigioco")]
    public float gameTime = 25f;
    public float highlightDuration = 1.5f;
    public float timeBetweenHighlights = 0.5f;
    public int clicksToDefrost = 3;

    [Header("Ingredienti")]
    public List<FridgeIngredientButton> allIngredients;

    [Header("DEBUG - Test rapido")]
    public bool useDebugMode = false;
    public int debugMood = 0; // 0=Happy, 1=Angry, 2=Sad, 3=Sick
    public string debugRecipeName = "Strawberry Milkshake";

    private float currentTime;
    private bool minigameActive = false;
    private List<FridgeIngredientButton> frozenIngredients;
    private List<string> requiredIngredientNames; // NUOVO: Ingredienti necessari dalla ricetta

    void Start()
    {
        // Mouse libero
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        // NUOVO: Ottieni gli ingredienti richiesti dal GameManager
        LoadRequiredIngredients();

        StartMinigame();
    }

    // NUOVO: Carica gli ingredienti richiesti dalla ricetta, ESCLUDENDO quelli già presi
    void LoadRequiredIngredients()
    {
        requiredIngredientNames = new List<string>();

        if (GameManager.Instance != null && GameManager.Instance.HasValidSelection())
        {
            int mood = GameManager.Instance.GetCurrentMood();
            string recipe = GameManager.Instance.GetCurrentRecipe();

            Debug.Log($"[FridgeDefrost] Ricetta: {recipe}");

            // Ottieni il database della ricetta
            RecipeDatabase database = null;
            switch (mood)
            {
                case 0: database = new HappyRecipes(); break;
                case 1: database = new AngryRecipes(); break;
                case 2: database = new SadRecipes(); break;
                case 3: database = new SickRecipes(); break;
            }

            if (database != null)
            {
                var recipes = database.GetRecipes();
                if (recipes.ContainsKey(recipe))
                {
                    // NUOVO: Ottieni la lista degli ingredienti già presi
                    List<string> alreadyTaken = GameManager.Instance.ingredientiPresi;
                    Debug.Log($"[FridgeDefrost] === ZAINO ===");
                    foreach (string taken in alreadyTaken)
                    {
                        Debug.Log($"[FridgeDefrost]   - {taken} (gia preso)");
                    }
                    Debug.Log($"[FridgeDefrost] ===============");

                    // Pulisci i nomi degli ingredienti e ESCLUDI quelli già presi
                    foreach (string ingredient in recipes[recipe])
                    {
                        string cleanName = ingredient.Replace("-", "")
                                                    .Replace("_", "")
                                                    .Trim()
                                                    .ToLower();

                        // Controlla se è già stato preso
                        bool alreadyHas = alreadyTaken.Any(taken =>
                            taken.Trim().ToLower() == cleanName
                        );

                        if (!alreadyHas)
                        {
                            // NON è stato preso → aggiungilo alla lista da congelare
                            requiredIngredientNames.Add(cleanName);
                            Debug.Log($"[FridgeDefrost] {cleanName} → DA SCONGELARE");
                        }
                        else
                        {
                            Debug.Log($"[FridgeDefrost] {cleanName} → GIA PRESO (skip)");
                        }
                    }

                    Debug.Log($"[FridgeDefrost] Ingredienti DA SCONGELARE: {string.Join(", ", requiredIngredientNames)}");
                }
            }
        }
        else
        {
            Debug.LogWarning("[FridgeDefrost] Nessuna ricetta selezionata! Congelando tutti gli ingredienti.");
        }
    }

    // MODIFICATO: Controlla se un ingrediente è necessario per la ricetta
    bool IsIngredientRequired(FridgeIngredientButton ingredient)
    {
        if (requiredIngredientNames == null || requiredIngredientNames.Count == 0)
            return true; // Se non c'è ricetta, congela tutto (fallback)

        // Ottieni il nome dell'ingrediente dal componente Ingredient
        Ingredient ingComponent = ingredient.GetComponent<Ingredient>();
        if (ingComponent != null)
        {
            string cleanName = ingComponent.ingredientName.Trim().ToLower();
            bool isRequired = requiredIngredientNames.Contains(cleanName);

            Debug.Log($"[FridgeDefrost] {ingComponent.ingredientName} richiesto? {isRequired}");
            return isRequired;
        }

        return false;
    }

    public void StartMinigame()
    {
        minigameActive = true;
        currentTime = gameTime;

        frozenIngredients = new List<FridgeIngredientButton>();

        // Disabilita gli Ingredient.cs durante il minigioco
        Ingredient[] selectableIngredients = FindObjectsByType<Ingredient>(FindObjectsSortMode.None);
        foreach (var ing in selectableIngredients)
        {
            ing.enabled = false;
        }

        // MODIFICATO: Congela SOLO gli ingredienti necessari per la ricetta
        foreach (var ingredient in allIngredients)
        {
            if (IsIngredientRequired(ingredient))
            {
                // Questo ingrediente SERVE per la ricetta → congelalo
                ingredient.InitializeForMinigame(clicksToDefrost);
                frozenIngredients.Add(ingredient);

                Ingredient ing = ingredient.GetComponent<Ingredient>();
                if (ing != null)
                {
                    Debug.Log($"[FridgeDefrost] {ing.ingredientName} → CONGELATO");
                }
            }
            else
            {
                // Questo ingrediente NON serve → lascialo normale (senza ghiaccio)
                ingredient.gameObject.SetActive(false); // Opzionale: nascondilo

                Ingredient ing = ingredient.GetComponent<Ingredient>();
                if (ing != null)
                {
                    Debug.Log($"[FridgeDefrost] {ing.ingredientName} → NON SERVE (nascosto)");
                }
            }
        }

        Debug.Log($"[FridgeDefrost] Ingredienti da scongelare: {frozenIngredients.Count}");

        // NUOVO: Se non c'è nulla da scongelare, salta direttamente alla selezione
        if (frozenIngredients.Count == 0)
        {
            Debug.Log("[FridgeDefrost] Nessun ingrediente da scongelare! Skip minigioco.");
            if (minigamePanel != null)
                minigamePanel.SetActive(false);
            EnableSelectionMode();
            return;
        }

        if (minigamePanel != null)
            minigamePanel.SetActive(true);

        if (gameCompletePanel != null)
            gameCompletePanel.SetActive(false);

        StartCoroutine(HighlightRoutine());
    }

    void Update()
    {
        if (!minigameActive) return;

        currentTime -= Time.deltaTime;

        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            timerText.text = "Tempo: " + seconds + "s";
        }

        if (currentTime <= 0)
        {
            GameOver();
        }

        if (frozenIngredients.Count == 0)
        {
            Victory();
        }
    }

    IEnumerator HighlightRoutine()
    {
        while (minigameActive && frozenIngredients.Count > 0)
        {
            FridgeIngredientButton randomIngredient = frozenIngredients[Random.Range(0, frozenIngredients.Count)];
            randomIngredient.Highlight(highlightDuration);

            yield return new WaitForSeconds(highlightDuration + timeBetweenHighlights);
        }
    }

    public void OnIngredientClicked(FridgeIngredientButton ingredient)
    {
        if (!minigameActive) return;

        if (ingredient.OnClick())
        {
            if (ingredient.IsDefrosted())
            {
                frozenIngredients.Remove(ingredient);
                Debug.Log($"[FridgeDefrost] Ingrediente scongelato! Rimangono: {frozenIngredients.Count}");
            }
        }
    }

    void Victory()
    {
        minigameActive = false;
        StopAllCoroutines();

        Debug.Log("[FridgeDefrost] Minigioco completato!");

        // Mostra panel di vittoria
        if (gameCompletePanel != null)
            gameCompletePanel.SetActive(true);

        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        // Attiva la modalita selezione dopo 2 secondi
        Invoke("EnableSelectionMode", 2f);
    }

    void EnableSelectionMode()
    {
        // Nascondi il panel di vittoria
        if (gameCompletePanel != null)
            gameCompletePanel.SetActive(false);

        Debug.Log("[FridgeDefrost] Modalita selezione attivata!");

        // Disattiva i FridgeIngredientButton
        foreach (var ingredient in allIngredients)
        {
            FridgeIngredientButton fridgeBtn = ingredient.GetComponent<FridgeIngredientButton>();
            if (fridgeBtn != null)
                fridgeBtn.enabled = false;
        }

        // Riattiva tutti gli ingredienti (anche quelli nascosti)
        foreach (var ingredient in allIngredients)
        {
            ingredient.gameObject.SetActive(true);
        }

        // Attiva gli Ingredient per la selezione
        Ingredient[] selectableIngredients = FindObjectsByType<Ingredient>(FindObjectsSortMode.None);
        foreach (var ing in selectableIngredients)
        {
            ing.enabled = true;
        }

        // Trova il RecipeManager per aggiornare i colori
        RecipeManager recipeManager = FindFirstObjectByType<RecipeManager>();
        if (recipeManager != null)
        {
            Debug.Log("[FridgeDefrost] RecipeManager trovato! Gli ingredienti verranno colorati correttamente.");
        }
        else
        {
            Debug.LogWarning("[FridgeDefrost] RecipeManager non trovato!");
        }
    }

    void GameOver()
    {
        minigameActive = false;
        StopAllCoroutines();

        Debug.Log("[FridgeDefrost] Tempo scaduto! Riprovo...");

        // Riavvia il minigioco
        Invoke("RestartMinigame", 1f);
    }

    void RestartMinigame()
    {
        // Riattiva tutti gli ingredienti prima di ricominciare
        foreach (var ingredient in allIngredients)
        {
            ingredient.gameObject.SetActive(true);
        }

        StartMinigame();
    }
}