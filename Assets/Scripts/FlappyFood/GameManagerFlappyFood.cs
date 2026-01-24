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

    // TRASCINA QUI IL BOTTONE NORMALE (TRIANGOLO)
    [SerializeField] private GameObject playButton;

    // NUOVO: TRASCINA QUI IL BOTTONE CON LA "X"
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
        // All'inizio mostriamo il bottone Play normale
        if (playButton != null) playButton.SetActive(true);
        if (winButton != null) winButton.SetActive(false); // La X è nascosta

        if (gameOver != null) gameOver.SetActive(false);
        if (youWon != null) youWon.SetActive(false);
        Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        if (player != null) player.enabled = false;
    }

    public void Play()
    {
        score = 0;
        if (scoreText != null) scoreText.text = score.ToString();

        // Quando giochi, nascondiamo TUTTI i bottoni
        if (playButton != null) playButton.SetActive(false);
        if (winButton != null) winButton.SetActive(false);

        if (gameOver != null) gameOver.SetActive(false);
        if (youWon != null) youWon.SetActive(false);

        Time.timeScale = 1f;
        if (player != null) player.enabled = true;

        Knife[] knife = FindObjectsOfType<Knife>();
        for (int i = 0; i < knife.Length; i++)
        {
            Destroy(knife[i].gameObject);
        }
    }

    public void GameOver()
    {
        // Se perdi, mostriamo il bottone Play normale
        if (playButton != null) playButton.SetActive(true);
        if (winButton != null) winButton.SetActive(false); // Assicuriamoci che la X sia spenta

        if (gameOver != null) gameOver.SetActive(true);

        Pause();
    }

    public void Victory()
    {
        // SE VINCI: Nascondi il bottone Play normale...
        if (playButton != null) playButton.SetActive(false);

        // ...e MOSTRA IL BOTTONE X!
        if (winButton != null) winButton.SetActive(true);

        if (youWon != null) youWon.SetActive(true);

        Pause();
    }

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