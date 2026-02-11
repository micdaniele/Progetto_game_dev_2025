using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotCookingToMinigame : MonoBehaviour
{
    [Header("Impostazioni Robot")]
    public Sprite[] robotSprites;        // Array di sprite del robot
    public float[] spriteDurations;      // Durata di ogni sprite (in secondi)
    public AudioClip clickSound;         // Suono al cambio
    public GameObject[] startPanels;     // Dialogo iniziale

    [Header("Dialogo Finale")]
    public Dialogue finalDialogue;       // Dialogo da mostrare all'ultimo sprite
    public bool isFinalDialogue = false; // Variabile utile per segnare la fine dei dialoghi e passare al minigame
    public string flappyFood = "FlappyFood"; // Scena minigame

    [Header("Impostazioni Animazione Pop")]
    public float popScale = 1.2f;        // Effetto ingrandimento (1.2 = 20% più grande)
    public float popDuration = 0.1f;     // Durata dell'animazione (in secondi)

    [Header("Impostazioni Vibrazione")]
    public int vibrationFrameIndex = -1; // A quale sprite attivare la vibrazione (-1 = disattivato)
    public float vibrationIntensity = 0.1f; // Intensità della vibrazione
    [SerializeField] private AudioClip vibrationSound; // Suono della vibrazione

    private Vector3 basePosition;        // Posizione originale del robot
    private bool isVibrating = false;    // Variabile per segnare che sta vibrando
    private float vibrationTimer = 0f;   // Durata della vibrazione

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private int currentIndex = 0;
    private Vector3 baseScale;           // Memorizza la grandezza originale del robot
    private float timer = 0f;            // Timer per il cambio automatico degli sprite
    private bool isPaused = false;       // Se è in pausa per un dialogo
    private bool hasStarted = false;     // Se la sequenza è iniziata
    private float originalVolume = 1f;   // Memorizza il volume originale dell'AudioSource

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Memorizza il volume originale per poterlo ripristinare dopo il fade out
        if (audioSource != null)
            originalVolume = audioSource.volume;

        // Memorizza la grandezza iniziale che ha nella scena
        baseScale = transform.localScale;

        // Memorizza la posizione iniziale
        basePosition = transform.position;

        // Imposta lo sprite iniziale
        if (robotSprites.Length > 0)
            spriteRenderer.sprite = robotSprites[0];

        // Mostra il dialogo iniziale
        foreach (GameObject panel in startPanels)
        {
            if (panel != null)
                panel.SetActive(true);
        }
    }

    void Update()
    {
        // Nasconde il dialogo e fa partire la sequenza
        if (Input.GetKeyDown(KeyCode.Space) && !hasStarted && !isPaused)
        {
            hasStarted = true;

            // Nascondi il dialogo iniziale
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
                // Fine vibrazione -> riporta il robot alla posizione originale
                isVibrating = false;
                transform.position = basePosition;

                // Reset del pitch e del volume all'originale
                if (audioSource != null)
                {
                    audioSource.pitch = 1f;
                    audioSource.volume = originalVolume;
                }
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

            // Parte l'audio del click
            if (clickSound != null && audioSource != null)
                audioSource.PlayOneShot(clickSound);

            // Attiva la vibrazione se siamo al frame specificato
            if (currentIndex == vibrationFrameIndex)
            {
                isVibrating = true;
                vibrationTimer = 0f;
                basePosition = transform.position; // Aggiorna la posizione base

                // Riproduce il suono del blender con fade out
                if (vibrationSound != null && audioSource != null && currentIndex < spriteDurations.Length)
                {
                    // Calcola il pitch per far durare il suono quanto la vibrazione
                    float vibrationDuration = spriteDurations[currentIndex];
                    float originalDuration = vibrationSound.length;
                    float pitchAdjustment = originalDuration / vibrationDuration;

                    audioSource.pitch = pitchAdjustment;
                    audioSource.volume = originalVolume; // Assicurati che inizi al volume massimo
                    audioSource.PlayOneShot(vibrationSound);

                    // Avvia la coroutine per il fade out del volume
                    StartCoroutine(FadeOutVibrationSound(vibrationDuration));
                }
            }

            // Avvia l'animazione di ingrandimento
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
                // Avvia la coroutine che aspetta lo spazio
                StartCoroutine(WaitForDialogueEnd());
            }
        }
    }


    // Coroutine che gestisce il fade out del suono della vibrazione.
    // Il volume rimane al massimo per la prima metà della durata, poi diminuisce gradualmente fino a zero nella seconda metà.
    // Questo crea un effetto naturale di "spegnimento" del frullatore.
    IEnumerator FadeOutVibrationSound(float totalDuration)
    {
        float elapsed = 0f;
        float halfDuration = totalDuration / 2f;  // Calcola il punto a metà durata

        // Prima metà: il volume rimane costante al massimo
        // Questo simula il frullatore che lavora a piena potenza
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Seconda metà: fade out graduale del volume
        // Il frullatore inizia a spegnersi progressivamente
        float fadeStartTime = elapsed;
        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            // Calcola quanto siamo avanti nel fade out (0 = inizio fade, 1 = fine fade)
            float fadeProgress = (elapsed - fadeStartTime) / halfDuration;

            // Interpola il volume dal valore originale a zero
            // Lerp crea una transizione fluida e naturale
            if (audioSource != null)
            {
                audioSource.volume = Mathf.Lerp(originalVolume, 0f, fadeProgress);
            }

            yield return null;
        }


        // Questo garantisce che non ci siano residui sonori
        if (audioSource != null)
        {
            audioSource.volume = 0f;
        }
    }

    // Coroutine che aspetta che il dialogo finale finisca prima di caricare il minigame.
    IEnumerator WaitForDialogueEnd()
    {
        // Aspetta finché il dialoguePanel è attivo
        if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel != null)
        {
            while (DialogueManager.Instance.dialoguePanel.activeSelf)
            {
                yield return null;
            }
        }

        // Carica la scena del minigame
        SceneManager.LoadScene(flappyFood);
    }

    // Coroutine per l'effetto "pop" (molla) quando cambia sprite.
    // Il robot si ingrandisce leggermente e poi torna alla dimensione normale.
    IEnumerator PopEffect()
    {
        Vector3 targetScale = baseScale * popScale; // Calcola la dimensione ingrandita
        float timer = 0;

        // Fase 1: Ingrandimento (Lerp da dimensione base a dimensione target)
        while (timer < popDuration)
        {
            transform.localScale = Vector3.Lerp(baseScale, targetScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null; // Aspetta il frame successivo
        }

        timer = 0;

        // Fase 2: Ritorno alla normalità (Lerp da dimensione target a dimensione base)
        while (timer < popDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, baseScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Assicura che alla fine torni esattamente alla dimensione originale
        transform.localScale = baseScale;
    }
}