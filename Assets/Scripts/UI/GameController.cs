using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Sprite bgImage;

    public List<Sprite> puzzles = new List<Sprite>();
    public List<Sprite> gamePuzzles = new List<Sprite>();
    public List<Button> btns = new List<Button>();

    private bool firstGuess, secondGuess;
    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessPuzzle, secondGuessPuzzle;

    void Start()
    {
        GetButtons();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;
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

    IEnumerator FlipCard(int index, Sprite targetSprite)
    {
        Button btn = btns[index];
        Vector3 originalScale = btn.transform.localScale;

        // Riduci la scala X a 0 (nasconde)
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scaleX = Mathf.Lerp(1f, 0f, elapsed / duration);
            btn.transform.localScale = new Vector3(scaleX, originalScale.y, originalScale.z);
            yield return null;
        }

        // Cambia sprite
        btn.image.sprite = targetSprite;

        // Riporta la scala X a 1 (mostra)
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

    public void PickAPuzzle(int index)
    {
        // Evita di cliccare sulla stessa carta due volte
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
            //COPPIA CORRETTA!
            yield return new WaitForSeconds(0.5f);

            // Disabilita i bottoni delle carte trovate
            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            // Opzionale: rendi le carte trasparenti
            btns[firstGuessIndex].image.color = new Color(1f, 1f, 1f, 0.5f);
            btns[secondGuessIndex].image.color = new Color(1f, 1f, 1f, 0.5f);

            countCorrectGuesses++;

            CheckIfTheGameIsFinished();
        }
        else
        {
            // COPPIA SBAGLIATA - rigira le carte
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(FlipCard(firstGuessIndex, bgImage));
            StartCoroutine(FlipCard(secondGuessIndex, bgImage));
        }

        // Reset per il prossimo turno
        firstGuess = secondGuess = false;
    }

    void CheckIfTheGameIsFinished()
    {
        if (countCorrectGuesses == gameGuesses)
        {
            Debug.Log("GAME FINISHED!");
            Debug.Log("It took you " + countGuesses + " guesses to finish the game!");
        }
    }
}