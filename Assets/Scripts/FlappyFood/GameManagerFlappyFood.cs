using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManagerFlappyFood : MonoBehaviour
{
    public static GameManagerFlappyFood Instance { get; private set; }

    [Header("Elementi di Gioco")]
    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;

    [Header("Interfaccia Utente (UI)")]
    [SerializeField] private Text scoreText;

    [SerializeField] private GameObject playButton;

    [SerializeField] private GameObject winButton;

    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject youWon;

    public int score { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        // All'inizio mostra il bottone play
        if (playButton != null) playButton.SetActive(true);
        if (winButton != null) winButton.SetActive(false); // nascondiamo il pulsante di vittoria
        
        if (gameOver != null) gameOver.SetActive(false); // se muori mostra il bottone per riprovare
        if (youWon != null) youWon.SetActive(false);// se vinci mostra il bottone per passare a vedere il piatto
        Pause();
    }

    //ferma il minigame
    public void Pause()
    {
        Time.timeScale = 0f;
        if (player != null) player.enabled = false;
    }


    public void Play()
    {
        score = 0;
        if (scoreText != null) scoreText.text = score.ToString();//testo per lo score

        // Quando parte il gioco nasconde tutti i bottoni
        if (playButton != null) playButton.SetActive(false);
        if (winButton != null) winButton.SetActive(false);

        if (gameOver != null) gameOver.SetActive(false);
        if (youWon != null) youWon.SetActive(false);

        Time.timeScale = 1f;
        if (player != null) player.enabled = true;

        Knife[] knife = FindObjectsByType<Knife>(FindObjectsSortMode.None);
        for (int i = 0; i < knife.Length; i++)
        {
            Destroy(knife[i].gameObject);
        }
    }

    public void GameOver()
    {
        // Se perdi, mostra il bottone play per riprovare
        if (playButton != null) playButton.SetActive(true);
        if (winButton != null) winButton.SetActive(false);//ci assicuriamo che win sia spento

        if (gameOver != null) gameOver.SetActive(true);

        Pause();
    }

    public void Victory()
    {
        // Se vinci nasconde il bottone play
        if (playButton != null) playButton.SetActive(false);

        // e mostra il bottone per la vittoria
        if (winButton != null) winButton.SetActive(true);

        if (youWon != null) youWon.SetActive(true);

        Pause();
    }

    //funzione per mostrare l'incremento dello score
    public void IncreaseScore()
    {
        score++;
        if (scoreText != null) scoreText.text = score.ToString();

        if (score >= 15)
        {
            Victory();
        }
    }
}