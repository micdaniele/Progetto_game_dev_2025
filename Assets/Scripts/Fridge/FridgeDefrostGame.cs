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

    [Header("Recipe Manager")]
    public GameObject recipeManagerObject; // Assegna nell'Inspector

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
                Debug.Log($"[FridgeDefrost] Ingrediente scongelato! Rimanenti: {frozenIngredients.Count}");

                // CONTROLLA SE HAI VINTO
                if (frozenIngredients.Count == 0)
                {
                    Victory();
                }
            }
        }
    }

    void Victory()
    {
        minigameActive = false;
        StopAllCoroutines();

        Debug.Log("[FridgeDefrost] 🎉 VITTORIA! Tutti gli ingredienti scongelati!");

        // Mostra il tempo finale
        if (timerValueText != null)
        {
            int finalTime = Mathf.CeilToInt(currentTime);
            timerValueText.text = finalTime.ToString();
        }

        // Mostra il panel di vittoria
        if (gameCompletePanel != null)
            gameCompletePanel.SetActive(true);

        // Aspetta prima di uscire dal minigioco
        StartCoroutine(ExitMinigameAfterDelay(2f));
    }

    IEnumerator ExitMinigameAfterDelay(float delay)
    {
        Debug.Log($"[FridgeDefrost] ⏳ Uscita dal minigioco tra {delay} secondi...");

        yield return new WaitForSeconds(delay);

        Debug.Log("[FridgeDefrost] ✅ Uscita dal minigioco!");

        // Nascondi i panel del minigioco
        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (gameCompletePanel != null)
            gameCompletePanel.SetActive(false);

        // Nascondi il ghiaccio su TUTTI gli ingredienti
        foreach (var ingredient in allIngredients)
        {
            if (ingredient != null && ingredient.iceOverlay != null)
            {
                ingredient.iceOverlay.gameObject.SetActive(false);
            }
        }

        // Disattiva i FridgeIngredientButton
        foreach (var ingredient in allIngredients)
        {
            FridgeIngredientButton fridgeBtn = ingredient.GetComponent<FridgeIngredientButton>();
            if (fridgeBtn != null)
            {
                fridgeBtn.enabled = false;
                Debug.Log($"[FridgeDefrost] Disattivato FridgeIngredientButton su {ingredient.gameObject.name}");
            }
        }

        // Attiva gli Ingredient per la selezione
        foreach (var ingredient in allIngredients)
        {
            Ingredient ing = ingredient.GetComponent<Ingredient>();
            if (ing != null)
            {
                ing.enabled = true;
                Debug.Log($"[FridgeDefrost] ✅ Attivato Ingredient su {ingredient.gameObject.name}");
            }
        }

        // RIATTIVA RecipeManager GameObject
        if (recipeManagerObject != null)
        {
            recipeManagerObject.SetActive(true);
            Debug.Log("[FridgeDefrost] ✅ RecipeManager GameObject riattivato!");

            // Verifica che il componente RecipeManager sia presente e attivo
            RecipeManager rm = recipeManagerObject.GetComponent<RecipeManager>();
            if (rm != null)
            {
                rm.enabled = true;
                Debug.Log("[FridgeDefrost] ✅ RecipeManager componente attivato!");
            }
        }
        else
        {
            Debug.LogError("[FridgeDefrost] ❌ RecipeManager GameObject NON assegnato nell'Inspector!");
        }

        // Disattiva questo script PER ULTIMO
        this.enabled = false;
        Debug.Log("[FridgeDefrost] ✅ FridgeDefrostGame disattivato!");
    }

    void GameOver()
    {
        minigameActive = false;
        StopAllCoroutines();

        Debug.Log("[FridgeDefrost] ⏰ Tempo scaduto! Riprovo...");

        // Riavvia il minigioco
        Invoke("RestartMinigame", 1f);
    }

    void RestartMinigame()
    {
        StartMinigame();
    }
}