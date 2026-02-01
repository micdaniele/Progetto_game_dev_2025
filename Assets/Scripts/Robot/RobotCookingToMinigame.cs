using System.Collections; // Necessario per le Coroutine (animazioni nel tempo)
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotCookingToMinigame : MonoBehaviour
{
    [Header("Impostazioni Robot")]
    public Sprite[] robotSprites;        // Sprite del robot in ordine
    public AudioClip clickSound;         // Suono al cambio
    public GameObject[] startPanels;     // Pannelli iniziali (tutorial/intro)

    [Header("Impostazioni Animazione Pop")]
    public float popScale = 1.2f;        // Quanto si ingrandisce (1.2 = 20% più grande)
    public float popDuration = 0.1f;     // Quanto dura l'animazione (in secondi)

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private int currentIndex = 0;
    private bool hasStarted = false;
    private Vector3 baseScale;           // Memorizza la grandezza originale del robot

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Memorizziamo la grandezza iniziale che hai impostato nella scena
        baseScale = transform.localScale;

        // Imposta sprite iniziale
        if (robotSprites.Length > 0)
            spriteRenderer.sprite = robotSprites[0];

        // Attiva i pannelli visibili all'inizio
        foreach (GameObject panel in startPanels)
        {
            if (panel != null)
                panel.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Primo spazio: chiude i pannelli e inizia
            if (!hasStarted)
            {
                hasStarted = true;

                foreach (GameObject panel in startPanels)
                {
                    if (panel != null)
                        panel.SetActive(false);
                }

                // Opzionale: fa un "pop" anche quando inizia il gioco per dare feedback
                StartCoroutine(PopEffect());

                return;
            }

            // Spazi successivi: cambia sprite
            CambiaSprite();
        }
    }

    void CambiaSprite()
    {
        currentIndex++;

        if (currentIndex < robotSprites.Length)
        {
            // 1. Cambia l'immagine
            spriteRenderer.sprite = robotSprites[currentIndex];

            // 2. Suona l'audio
            if (clickSound != null && audioSource != null)
                audioSource.PlayOneShot(clickSound);

            // 3. Avvia l'animazione di rimbalzo
            // Fermiamo eventuali animazioni precedenti per evitare glitch se premi veloce
            StopAllCoroutines();
            StartCoroutine(PopEffect());
        }
        else
        {
            // Fine sequenza: carica il minigioco
            SceneManager.LoadScene("FlappyFood");
        }
    }

    // Coroutine per l'effetto "molla"
    IEnumerator PopEffect()
    {
        Vector3 targetScale = baseScale * popScale; // Calcola la dimensione ingrandita
        float timer = 0;

        // Fase 1: Ingrandimento (Lerp da base a target)
        while (timer < popDuration)
        {
            transform.localScale = Vector3.Lerp(baseScale, targetScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null; // Aspetta il frame successivo
        }

        timer = 0;

        // Fase 2: Ritorno alla normalità (Lerp da target a base)
        while (timer < popDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, baseScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Sicurezza: assicura che alla fine torni esattamente alla dimensione originale
        transform.localScale = baseScale;
    }
}