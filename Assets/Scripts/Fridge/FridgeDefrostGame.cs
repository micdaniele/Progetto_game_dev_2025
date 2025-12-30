using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FridgeDefrostGame : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public Text timerText;
    public GameObject minigamePanel; // Panel attivo durante il minigioco
    public GameObject gameCompletePanel; // Panel "Minigioco completato!"
    
    [Header("Impostazioni Minigioco")]
    public float gameTime = 25f;
    public float highlightDuration = 1.5f;
    public float timeBetweenHighlights = 0.5f;
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
        
        // Attiva la modalità selezione dopo 2 secondi
        Invoke("EnableSelectionMode", 2f);
    }
    
    void EnableSelectionMode()
    {
        // Nascondi il panel di vittoria
        if (gameCompletePanel != null)
            gameCompletePanel.SetActive(false);
        
        // Ora gli ingredienti sono sbloccati e possono essere selezionati normalmente
        // Se hai script Ingredient.cs sui bottoni, il RecipeManager gestirà i colori
        
        Debug.Log("[FridgeDefrost] Modalità selezione attivata!");
        
        // Opzionale: se vuoi tornare a Kitchen2 dopo la selezione
        // Invoke("LoadKitchenScene", 3f);
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
