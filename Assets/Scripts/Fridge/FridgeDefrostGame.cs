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

    //funzione per far iniziare il minigame
    public void StartMinigame()
    {
        StopAllCoroutines();
        //variabili per inizializzare il giusto inizio del gioco
        gameEnded = false;
        waitingForRestart = false;
        minigameActive = true;
        currentTime = gameTime;

        //crea una lista con tutti gli ingredienti
        frozenIngredients = new List<FridgeIngredientButton>(allIngredients);


        foreach (var ingredient in allIngredients)
        {
            ingredient.InitializeForMinigame(clicksToDefrost); //inizializza ogni ingrediente con quante volte devono essere cliccati
        }

        StartCoroutine(HighlightRoutine());

        //Disattiva i pannel della vittoria e della sconfitta
        if (winText != null) winText.SetActive(false);
        if (gameOverText != null) gameOverText.SetActive(false);
    }

    void Update()
    {
        // Aspetta spazio per iniziare
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

        // Attende R per ricominciare dopo Game Over
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

        //si ferma se il minigame è concluso o è finito il gioco
        if (!minigameActive || gameEnded) return;

        currentTime -= Time.deltaTime;

        //decrementa il timer
        if (timerValueText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            timerValueText.text = seconds.ToString();
        }

        //se arriva a 0 il timer parte il game over
        if (currentTime <= 0)
        {
            GameOver();
        }
    }

    //funzione per evidenziare un ingrediente
    IEnumerator HighlightRoutine()
    {
        while (minigameActive && frozenIngredients.Count > 0)
        {
            // Scegle random un ingrediente non scongelato e lo evidenzia
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

    //funzione che controlla se gli ingredienti sono scongelati
    public void OnIngredientClicked(FridgeIngredientButton ingredient)
    {
        if (!minigameActive) return;

        //delega al botttone con ingredient
        if (ingredient.OnClick())
        {
            //se è scongelato lo rimuovee dalla lista
            if (ingredient.IsDefrosted())
            {
                frozenIngredients.Remove(ingredient);
                //Debug.Log($"[FridgeDefrost] Ingrediente scongelato! Rimanenti: {frozenIngredients.Count}");

                // controlla se hai vinto
                if (frozenIngredients.Count == 0)
                {
                    Victory();
                }
            }
        }
    }


//funzione per la vittoria
    void Victory()
    {
        if (gameEnded) return; // Evita doppie chiamate

        //variabili per la fine del minigame
        gameEnded = true;
        minigameActive = false;
        StopAllCoroutines();

        //Debug.Log("[FridgeDefrost] VITTORIA!");
        //segna al game manager che è finita il minigame
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteTask("FridgeMinigame");
            //Debug.Log("[FridgeDefrost] Task 'FridgeMinigame' completato!");
        }
        //else
        //{
        //    Debug.LogWarning("[FridgeDefrost] GameManager non trovato!");
        //}

        //attiva il pannello della vittoria
        if (winText != null)
            winText.SetActive(true);

        // Aspetta e poi torna alla scena frigo
        StartCoroutine(LoadFridgeSceneAfterDelay(2f));
    }

    //funzione per passare alla nuova scena con un po' di dilay
    IEnumerator LoadFridgeSceneAfterDelay(float delay)
    {
        //Debug.Log($"[FridgeDefrost] Caricamento scena frigo tra {delay} secondi...");
        yield return new WaitForSeconds(delay);
        //Debug.Log($"[FridgeDefrost] Carico scena: {fridgeSceneName}");
        SceneManager.LoadScene(fridgeSceneName);
    }

    //funzione per il game over
    void GameOver()
    {
        if (gameEnded) return;//esce se è finito il gioco

        //variabili per impostare il gioco all'inizio
        gameEnded = true;
        minigameActive = false;
        waitingForRestart = true;

        StopAllCoroutines();

        //Debug.Log("[FridgeDefrost] Tempo scaduto!");

        //attiva il pannello del game over
        if (gameOverText != null)
            gameOverText.SetActive(true);
    }

    //funzione per il restart
    void RestartMinigame()
    {
        StartMinigame();
    }
}