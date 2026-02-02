using System.Collections; // Necessario per le Coroutine (animazioni nel tempo)
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotCookingToMinigame : MonoBehaviour
{
    [Header("Impostazioni Robot")]
    public Sprite[] robotSprites;        // Sprite del robot in ordine
    public float[] spriteDurations;      // Durata di ogni sprite (in secondi)
    public AudioClip clickSound;         // Suono al cambio
    public GameObject[] startPanels;     // Pannelli iniziali (tutorial/intro)

    [Header("Dialogo Finale")]
    public Dialogue finalDialogue;       // Dialogo da mostrare all'ultimo sprite
    public bool isFinalDialogue = false;
    public string flappyFood = "FlappyFood";


    [Header("Impostazioni Animazione Pop")]
    public float popScale = 1.2f;        // Quanto si ingrandisce (1.2 = 20% più grande)
    public float popDuration = 0.1f;     // Quanto dura l'animazione (in secondi)

    [Header("Impostazioni Vibrazione")]
    public int vibrationFrameIndex = -1; // A quale sprite attivare la vibrazione (-1 = disattivato)
    public float vibrationIntensity = 1f; // Intensità della vibrazione

    private Vector3 basePosition;        // Posizione originale del robot
    private bool isVibrating = false;    // Se sta vibrando
    private float vibrationTimer = 0f;   // Timer per la vibrazione

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private int currentIndex = 0;
    private Vector3 baseScale;           // Memorizza la grandezza originale del robot
    private float timer = 0f;            // Timer per il cambio automatico
    private bool isPaused = false;       // Se è in pausa per un dialogo
    private bool hasStarted = false;     // Se la sequenza è iniziata


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Memorizziamo la grandezza iniziale che hai impostato nella scena
        baseScale = transform.localScale;

        // Memorizziamo la posizione iniziale
        basePosition = transform.position;

        // Imposta sprite iniziale
        if (robotSprites.Length > 0)
            spriteRenderer.sprite = robotSprites[0];

        // Mostra i pannelli iniziali
        foreach (GameObject panel in startPanels)
        {
            if (panel != null)
                panel.SetActive(true);
        }
    }

    void Update()
    {
        //Nasconde i pannelli e fa partire la sequenza
        if (Input.GetKeyDown(KeyCode.Space) && !hasStarted && !isPaused)
        {
            hasStarted = true;

            // Nascondi tutti i pannelli iniziali
            foreach (GameObject panel in startPanels)
            {
                if (panel != null)
                    panel.SetActive(false);
            }

            return;
        }

        // Se non è ancora iniziato, non far avanzare nulla
        if (!hasStarted)
            return;

        // Gestione vibrazione
        if (isVibrating)
        {
            vibrationTimer += Time.deltaTime;

            // Vibra per la durata dello sprite corrente
            if (currentIndex < spriteDurations.Length && vibrationTimer < spriteDurations[currentIndex])
            {
                float x = Random.Range(-1f, 1f) * vibrationIntensity;
                float y = Random.Range(-1f, 1f) * vibrationIntensity;
                transform.position = basePosition + new Vector3(x, y, 0);
            }
            else
            {
                // Fine vibrazione
                isVibrating = false;
                transform.position = basePosition;
            }
        }

        // Gestisce il cambio automatico degli sprite
        timer += Time.deltaTime;

        if (!isPaused)
        {
            // Controlla se è il momento di cambiare sprite
            if (currentIndex < spriteDurations.Length && timer >= spriteDurations[currentIndex])
            {
                timer = 0f; // Reset del timer
                CambiaSprite();
            }
        }
    }

    void CambiaSprite()
    {
        currentIndex++;

        if (currentIndex < robotSprites.Length)
        {
            // Cambia l'immagine
            spriteRenderer.sprite = robotSprites[currentIndex];

            // Suona l'audio
            if (clickSound != null && audioSource != null)
                audioSource.PlayOneShot(clickSound);

            // Attiva la vibrazione se siamo al frame specificato
            if (currentIndex == vibrationFrameIndex)
            {
                isVibrating = true;
                vibrationTimer = 0f;
                basePosition = transform.position; // Aggiorna la posizione base
            }

            // Avvia l'animazione di rimbalzo
            StopAllCoroutines();
            StartCoroutine(PopEffect());
        }
        else
        {
            // Alla fine della sequenza mostra il dialogo finale
            if (finalDialogue != null && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(finalDialogue);
                isPaused = true;
                // Avvia la coroutine che aspetta e poi permette di andare avanti
                StartCoroutine(WaitForDialogueEnd());
            }
        }
    }

    // Coroutine che aspetta che il dialogo finisca
    IEnumerator WaitForDialogueEnd()
    {
        // Aspetta finché il dialoguePanel è attivo
        if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel != null)
        {
            while (DialogueManager.Instance.dialoguePanel.activeSelf)
            {
                yield return null;
            }
        };

        // Carica la scena del minigame
        SceneManager.LoadScene(flappyFood);
    }


    // Coroutine per l'effetto "molla"
    IEnumerator PopEffect()
    {
        Vector3 targetScale = baseScale * popScale; // Calcola la dimensione ingrandita
        float timer = 0;

        //Ingrandimento (Lerp da base a target)
        while (timer < popDuration)
        {
            transform.localScale = Vector3.Lerp(baseScale, targetScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null; // Aspetta il frame successivo
        }

        timer = 0;

        //Ritorno alla normalità (Lerp da target a base)
        while (timer < popDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, baseScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Assicura che alla fine torni alla dimensione originale
        transform.localScale = baseScale;
    }
}