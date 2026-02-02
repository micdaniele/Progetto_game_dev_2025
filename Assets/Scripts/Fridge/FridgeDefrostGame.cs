using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class FridgeDefrostGame : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public Text timerValueText;
    public GameObject rulesPanel;
    public GameObject winText;
    public GameObject gameOverText;

    [Header("Impostazioni Minigioco")]
    public float gameTime = 40f;
    public float highlightDuration = 0.35f;
    public float timeBetweenHighlights = 0.05f;
    public int clicksToDefrost = 3;

    [Header("Ingredienti")]
    public List<FridgeIngredientButton> allIngredients;

    [Header("Recipe Manager")]
    public GameObject recipeManagerObject;

    [Header("Scene Settings")]
    public string fridgeSceneName = "Fridge";

    private bool gameEnded = false;
    private bool waitingForStart = true;
    private bool waitingForRestart = false;
    private float currentTime;
    private bool minigameActive = false;
    private List<FridgeIngredientButton> frozenIngredients;

    void Start()
    {
        if (rulesPanel != null)
            rulesPanel.SetActive(true);

        // Mouse libero
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        waitingForStart = true;
    }

    public void StartMinigame()
    {
        StopAllCoroutines();

        gameEnded = false;
        waitingForRestart = false;
        minigameActive = true;
        currentTime = gameTime;

        frozenIngredients = new List<FridgeIngredientButton>(allIngredients);

        foreach (var ingredient in allIngredients)
        {
            ingredient.InitializeForMinigame(clicksToDefrost);
        }

        StartCoroutine(HighlightRoutine());

        if (winText != null) winText.SetActive(false);
        if (gameOverText != null) gameOverText.SetActive(false);
    }

    void Update()
    {
        // Aspetta SPACE per iniziare
        if (waitingForStart)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                waitingForStart = false;
                if (rulesPanel != null)
                    rulesPanel.SetActive(false);
                StartMinigame();
            }
            return;
        }

        // Attendi R per ricominciare dopo Game Over
        if (waitingForRestart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (gameOverText != null)
                    gameOverText.SetActive(false);

                RestartMinigame();
            }
            return; // esci dall'Update durante l'attesa
        }

        if (!minigameActive || gameEnded) return;

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
            // Scegli un ingrediente NON scongelato
            FridgeIngredientButton current = frozenIngredients[Random.Range(0, frozenIngredients.Count)];

            // Lampeggia finché non viene rotto
            while (minigameActive && !current.IsDefrosted())
            {
                current.Highlight(highlightDuration);
                yield return new WaitForSeconds(highlightDuration);
            }

            // Piccola pausa prima di passare al prossimo
            yield return new WaitForSeconds(timeBetweenHighlights);
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
                //Debug.Log($"[FridgeDefrost] Ingrediente scongelato! Rimanenti: {frozenIngredients.Count}");

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
        if (gameEnded) return; // Evita doppie chiamate

        gameEnded = true;
        minigameActive = false;
        StopAllCoroutines();

        //Debug.Log("[FridgeDefrost] VITTORIA!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteTask("FridgeMinigame");
            //Debug.Log("[FridgeDefrost] Task 'FridgeMinigame' completato!");
        }
        //else
        //{
        //    Debug.LogWarning("[FridgeDefrost] GameManager non trovato!");
        //}

        if (winText != null)
            winText.SetActive(true);

        // Aspetta e poi torna alla scena frigo
        StartCoroutine(LoadFridgeSceneAfterDelay(2f));
    }

    IEnumerator LoadFridgeSceneAfterDelay(float delay)
    {
        //Debug.Log($"[FridgeDefrost] Caricamento scena frigo tra {delay} secondi...");
        yield return new WaitForSeconds(delay);
        //Debug.Log($"[FridgeDefrost] Carico scena: {fridgeSceneName}");
        SceneManager.LoadScene(fridgeSceneName);
    }

    void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;
        minigameActive = false;
        waitingForRestart = true;

        StopAllCoroutines();

        Debug.Log("[FridgeDefrost] Tempo scaduto!");

        if (gameOverText != null)
            gameOverText.SetActive(true);
    }

    void RestartMinigame()
    {
        StartMinigame();
    }
}