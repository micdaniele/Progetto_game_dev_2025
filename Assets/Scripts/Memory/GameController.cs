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

    [Header("AUDIO")]
    public AudioSource suonoDispensa; // Audio della dispensa che si chiude
    [SerializeField] private AudioClip suonoClick; // Suono quando clicchi una carta
    private AudioSource audioSource;

    [Header("IMPOSTAZIONI SCENA")]
    public string cucina = "Kitchen2"; //scena di ritorno

    public List<Sprite> puzzles = new List<Sprite>();
    public List<Sprite> gamePuzzles = new List<Sprite>();
    public List<Button> btns = new List<Button>(); //lista di bottoni per le carte 

    //controllo sulle tessere girate del memory
    private bool firstGuess, secondGuess;
    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessPuzzle, secondGuessPuzzle;

    //controllo errori
    private int erroriAttuali = 0;
    private int erroriMassimi = 6;
    private bool giocoFinito = false;

    void Start()
    {
        // Setup AudioSource per i click
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        //forza lo stato disattivato dei pannelli
        if (pannelloGameOver != null) pannelloGameOver.SetActive(false);
        if (pannelloVittoria != null) pannelloVittoria.SetActive(false);

        //inizzializza il gioco
        GetButtons();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;
        AddListeners();
    }

    //controlla se il gioco è finito

    void Update()
    {
        if (giocoFinito && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void PickAPuzzle(int index)
    {
        if (giocoFinito) return;//esce se il gioco è finito
        if (firstGuess && index == firstGuessIndex) return;//esce se le due carte sono uguali

        // riproduce il suono quando clicchi un bottone
        if (suonoClick != null && audioSource != null)
        {
            audioSource.PlayOneShot(suonoClick);
        }

        if (!firstGuess)
        {
            firstGuess = true;//segna che la prima scelta è stata fatta
            firstGuessIndex = index;//registra quale indice della carta è stata seleziionata
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;//salva il nome
            StartCoroutine(FlipCard(firstGuessIndex, gamePuzzles[firstGuessIndex]));//gira la carta
        }
        else if (!secondGuess)
        {
            secondGuess = true;//segna che la seconda scelta è stata fatta
            secondGuessIndex = index;
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
            StartCoroutine(FlipCard(secondGuessIndex, gamePuzzles[secondGuessIndex]));
            countGuesses++;
            StartCoroutine(CheckIfThePuzzlesMatch());//confronta se la prima e seconda scelta sono uguali
        }
    }

    //controlla se le due scelte sono uguali
    IEnumerator CheckIfThePuzzlesMatch()
    {
        yield return new WaitForSeconds(1f);

        //se le scelta è uguale
        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(0.5f);
            //disattiva i bottoni
            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            //li rende grigi
            var color = btns[firstGuessIndex].image.color;
            color.a = 0.5f;
            btns[firstGuessIndex].image.color = color;
            btns[secondGuessIndex].image.color = color;

            countCorrectGuesses++;

            //se sono finite le carte avvia la vittoria
            if (countCorrectGuesses == gameGuesses)
            {
                StartCoroutine(SequenzaVittoria());
            }
        }
        else
        {
            //se sbagli incrementi il nummero degli errori
            erroriAttuali++;

            //gira le carte
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FlipCard(firstGuessIndex, bgImage));
            StartCoroutine(FlipCard(secondGuessIndex, bgImage));

            //se gli errori superano il massimo parte il game over
            if (erroriAttuali >= erroriMassimi)
            {
                GameOver();
            }
        }
        firstGuess = secondGuess = false;
    }


    //sequenza di vittoria
    IEnumerator SequenzaVittoria()
    {
        giocoFinito = true;
        //segna nel game manager che il memory è stato completato
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteTask("Memory");
        }

        //mostra il pannello di vittoria
        if (pannelloVittoria != null)
        {
            pannelloVittoria.SetActive(true);
        }

        // Aspetta che il giocatore veda il pannello vittoria
        yield return new WaitForSeconds(2f);

        // riproduce il suono della chiusuraì
        if (suonoDispensa != null)
        {
            suonoDispensa.Play();
            // Aspettiamo un tempo pari alla durata del suono per non tagliarlo
            yield return new WaitForSeconds(suonoDispensa.clip.length);
        }

        // Torna alla cucina
        SceneManager.LoadScene(cucina);
    }

    //sequenza di sconfitta
    void GameOver()
    {
        giocoFinito = true;
        //attiva il pannello della sconfitta
        if (pannelloGameOver != null)
        {
            pannelloGameOver.SetActive(true);
        }

        //disattiva tutti i bottoni
        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }
    }

    //crea ivari bottoni dal prefab
    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        for (int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = bgImage;
        }
    }

    //crea coppie di sprite
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

    //Mescola casualmente le carte prima di iniziare la partita.
    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            //Per ogni posizione:
            //-sceglie un indice casuale
            //-scambia gli elementi
            Sprite temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    //Collega ogni bottone alla sua carta
    void AddListeners()
    {
        for (int i = 0; i < btns.Count; i++)
        {
            int index = i;
            btns[i].onClick.AddListener(() => PickAPuzzle(index));
        }
    }


    //funzione per l'animazione in cui gira la carta (si divide in due sotto animazioni)
    IEnumerator FlipCard(int index, Sprite targetSprite)
    {
        Button btn = btns[index];
        Vector3 originalScale = btn.transform.localScale;
        float elapsed = 0f;
        float duration = 0.2f;

        //prima metà in cui riduce le x da 1 a 0 e la fa scomparire
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scaleX = Mathf.Lerp(1f, 0f, elapsed / duration);
            btn.transform.localScale = new Vector3(scaleX, originalScale.y, originalScale.z);
            yield return null;
        }
        //cambio di sprite quando x è a 0
        btn.image.sprite = targetSprite;
        elapsed = 0f;
        //seconda metà in cui aumenta le x da 0 a 1 con la nuova immagine
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