using UnityEngine;
using UnityEngine.UI; // Necessario per usare il componente Text
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Sprite bgImage;

    [Header("UI Settings")]
    [SerializeField]
    private Text feedbackText; // Trascina qui un oggetto Text dalla scena Unity

    public List<Sprite> puzzles = new List<Sprite>();
    public List<Sprite> gamePuzzles = new List<Sprite>();
    public List<Button> btns = new List<Button>();

    private bool firstGuess, secondGuess;
    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessPuzzle, secondGuessPuzzle;

    // Variabili per la gestione degli errori
    private int currentErrors = 0;
    private const int maxErrors = 4;
    private bool gameEnded = false; // Per bloccare il gioco se finisce

    void Start()
    {
        GetButtons();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;

        // Resetta il testo all'inizio
        if (feedbackText != null)
            feedbackText.text = "";

        AddListeners();
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        for (int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = bgImage;
        }
    }

    void AddGamePuzzles()
    {
        int looper = btns.Count;
        int index = 0;
        for (int i = 0; i < looper; i++)
        {
            if (index == looper / 2)
            {
                index = 0;
            }
            gamePuzzles.Add(puzzles[index]);
            index++;
        }
    }

    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void AddListeners()
    {
        for (int i = 0; i < btns.Count; i++)
        {
            int index = i;
            btns[i].onClick.AddListener(() => PickAPuzzle(index));
        }
    }

    public void PickAPuzzle(int index)
    {
        // Se il gioco è finito, non fare nulla
        if (gameEnded) return;

        // Evita di cliccare sulla stessa carta due volte o mentre si stanno controllando le carte
        if (firstGuess && index == firstGuessIndex)
            return;

        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = index;
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;
            StartCoroutine(FlipCard(firstGuessIndex, gamePuzzles[firstGuessIndex]));
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = index;
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
            StartCoroutine(FlipCard(secondGuessIndex, gamePuzzles[secondGuessIndex]));

            countGuesses++;

            StartCoroutine(CheckIfThePuzzlesMatch());
        }
    }

    IEnumerator CheckIfThePuzzlesMatch()
    {
        yield return new WaitForSeconds(1f);

        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            // --- COPPIA CORRETTA! ---
            yield return new WaitForSeconds(0.5f);

            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            btns[firstGuessIndex].image.color = new Color(1f, 1f, 1f, 0.5f);
            btns[secondGuessIndex].image.color = new Color(1f, 1f, 1f, 0.5f);

            countCorrectGuesses++;

            CheckIfTheGameIsFinished();
        }
        else
        {
            // --- COPPIA SBAGLIATA ---
            currentErrors++; // Aumento gli errori

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(FlipCard(firstGuessIndex, bgImage));
            StartCoroutine(FlipCard(secondGuessIndex, bgImage));

            // Controllo se ho superato il limite di errori
            if (currentErrors >= maxErrors)
            {
                GameOver();
            }
        }

        // Reset per il prossimo turno
        firstGuess = secondGuess = false;
    }

    void CheckIfTheGameIsFinished()
    {
        if (countCorrectGuesses == gameGuesses)
        {
            Debug.Log("GAME FINISHED!");
            if (feedbackText != null)
            {
                feedbackText.text = "HAI VINTO, ORA CONTINUA A CUCINARE";
                feedbackText.color = Color.green; // Opzionale: testo verde per vittoria
            }
            gameEnded = true;
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!");
        if (feedbackText != null)
        {
            feedbackText.text = "GAME OVER HAI SBAGLIATO TROPPE VOLTE";
            feedbackText.color = Color.red; // Opzionale: testo rosso per sconfitta
        }

        // Blocca il gioco e disabilita tutti i pulsanti rimasti
        gameEnded = true;
        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }
    }

    IEnumerator FlipCard(int index, Sprite targetSprite)
    {
        Button btn = btns[index];
        Vector3 originalScale = btn.transform.localScale;

        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scaleX = Mathf.Lerp(1f, 0f, elapsed / duration);
            btn.transform.localScale = new Vector3(scaleX, originalScale.y, originalScale.z);
            yield return null;
        }

        btn.image.sprite = targetSprite;

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scaleX = Mathf.Lerp(0f, 1f, elapsed / duration);
            btn.transform.localScale = new Vector3(scaleX, originalScale.y, originalScale.z);
            yield return null;
        }

        btn.transform.localScale = originalScale;
    }
}