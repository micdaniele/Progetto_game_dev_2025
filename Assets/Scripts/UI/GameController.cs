using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Sprite bgImage;

    [Header("COLLEGAMENTI UI")]
    public GameObject pannelloGameOver;
    public GameObject pannelloVittoria;

    [Header("IMPOSTAZIONI SCENA")]
    public string cucina = "Kitchen2";

    public List<Sprite> puzzles = new List<Sprite>();
    public List<Sprite> gamePuzzles = new List<Sprite>();
    public List<Button> btns = new List<Button>();

    private bool firstGuess, secondGuess;
    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessPuzzle, secondGuessPuzzle;

    private int erroriAttuali = 0;
    private int erroriMassimi = 6;
    private bool giocoFinito = false;

    void Start()
    {
        if (pannelloGameOver != null) pannelloGameOver.SetActive(false);
        if (pannelloVittoria != null) pannelloVittoria.SetActive(false);

        GetButtons();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;
        AddListeners();

        Debug.Log("[Memory] Gioco iniziato");
    }

    void Update()
    {
        if (giocoFinito && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void PickAPuzzle(int index)
    {
        if (giocoFinito) return;
        if (firstGuess && index == firstGuessIndex) return;

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
            yield return new WaitForSeconds(0.5f);
            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            var color = btns[firstGuessIndex].image.color;
            color.a = 0.5f;
            btns[firstGuessIndex].image.color = color;
            btns[secondGuessIndex].image.color = color;

            countCorrectGuesses++;

            if (countCorrectGuesses == gameGuesses)
            {
                StartCoroutine(SequenzaVittoria());
            }
        }
        else
        {
            erroriAttuali++;

            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FlipCard(firstGuessIndex, bgImage));
            StartCoroutine(FlipCard(secondGuessIndex, bgImage));

            if (erroriAttuali >= erroriMassimi)
            {
                GameOver();
            }
        }
        firstGuess = secondGuess = false;
    }

   
    IEnumerator SequenzaVittoria()
    {
        giocoFinito = true;

        Debug.Log("[Memory] VITTORIA! ");

        // Segna il memory come completato
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteTask("Memory");
            Debug.Log("[Memory] Task 'Memory' completato nel GameManager!");
        }
        else
        {
            Debug.LogWarning("[Memory] GameManager non trovato!");
        }

        if (pannelloVittoria != null)
        {
            pannelloVittoria.SetActive(true);
        }

        yield return new WaitForSeconds(2f);

        // Torna alla cucina
        Debug.Log($"[Memory] Torno alla cucina: {cucina}");
        SceneManager.LoadScene(cucina);
    }

    void GameOver()
    {
        giocoFinito = true;
        Debug.Log("[Memory] GAME OVER!");

        if (pannelloGameOver != null)
        {
            pannelloGameOver.SetActive(true);
        }

        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }
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
            if (index == looper / 2) index = 0;
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