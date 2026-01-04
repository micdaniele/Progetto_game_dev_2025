using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FridgeDefrostGame : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public Text timerValueText;
    public GameObject minigamePanel; // Panel attivo durante il minigioco
    public GameObject gameCompletePanel; // Panel "Minigioco completato!"
    
    [Header("Impostazioni Minigioco")]
    public float gameTime = 25f;
    public float highlightDuration = 1.5f;
    public float timeBetweenHighlights = 0.3f;
    public int clicksToDefrost = 3;
    
    [Header("Ingredienti")]
    public List<FridgeIngredientButton> allIngredients;
    
    private float currentTime;
    private bool minigameActive = false;
    private List<FridgeIngredientButton> frozenIngredients;
    
    void Start()
    {
        // Mouse libero
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
        
        StartMinigame();
    }
    
    public void StartMinigame()
    {
        minigameActive = true;
        currentTime = gameTime;
        
        frozenIngredients = new List<FridgeIngredientButton>(allIngredients);

        foreach (var ingredient in allIngredients)
        {
            ingredient.InitializeForMinigame(clicksToDefrost);

            // BLOCCA Ingredient durante il minigioco
            Ingredient ing = ingredient.GetComponent<Ingredient>();
            if (ing != null)
                ing.enabled = false;
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
        
        if (timerValueText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            timerValueText.text = seconds.ToString();
        }
        
        if (currentTime <= 0)
        {
            GameOver();
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
            }
        }
    }
    
    void Victory()
    {
        minigameActive = false;
        StopAllCoroutines();
        
        
        Debug.Log("[FridgeDefrost] Minigioco completato!");
        
        // Nascondi il ghiaccio su TUTTI
        foreach (var ingredient in allIngredients)
        {
            if (ingredient != null)
            {
                ingredient.gameObject.GetComponent<FridgeIngredientButton>().enabled = false;

                if (ingredient.iceOverlay != null)
                    ingredient.iceOverlay.gameObject.SetActive(false);
            }
        }
        // RIATTIVA Ingredient (selezione ricetta)
        foreach (var ingredient in allIngredients)
        {
            Ingredient ing = ingredient.GetComponent<Ingredient>();
            if (ing != null)
                ing.enabled = true;
        }

    }
    
    void EnableSelectionMode()
    {
        // Nascondi il panel di vittoria
        if (gameCompletePanel != null)
            gameCompletePanel.SetActive(false);
        
        // Ora gli ingredienti sono sbloccati e possono essere selezionati normalmente
        // Se ho script Ingredient.cs sui bottoni, il RecipeManager gestirà i colori
        
        Debug.Log("[FridgeDefrost] Modalità selezione attivata!");
        
        // se vuoi tornare a Kitchen2 dopo la selezione
        // Invoke("LoadKitchenScene", 3f);

        // Disattiva i FridgeIngredientButton
        foreach (var ingredient in allIngredients)
        {
            FridgeIngredientButton fridgeBtn = ingredient.GetComponent<FridgeIngredientButton>();
            if (fridgeBtn != null)
                fridgeBtn.enabled = false;
        }
    
        // Attiva gli Ingredient per la selezione
        foreach (var ingredient in allIngredients)
        {
            Ingredient ing = ingredient.GetComponent<Ingredient>();
            if (ing != null)
            {
                ing.enabled = true;
                Debug.Log($"Attivato Ingredient su {ingredient.gameObject.name}");
            }
        }

        // RIATTIVA RecipeManager
        GameObject recipeManagerObj = GameObject.Find("RecipeManager");
        if (recipeManagerObj != null)
        {
            recipeManagerObj.SetActive(true);
            Debug.Log("[FridgeDefrost] RecipeManager GameObject riattivato!");
        }
        else
        {
            Debug.LogWarning("[FridgeDefrost] RecipeManager GameObject non trovato!");
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
        StartMinigame();
    }
} 
