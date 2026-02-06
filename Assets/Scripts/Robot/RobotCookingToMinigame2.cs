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
    public bool isFinalDialogue = false; //variabile utile per segnare la fine dei dialoghi e passare al minigame
    public string flappyFood = "FlappyFood"; //scena minigame

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

    void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

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
        //Nasconde il dialogo e fa partire la sequenza
        if (Input.GetKeyDown(KeyCode.Space) && !hasStarted && !isPaused)
        {
            hasStarted = true;

            // Nascondi il gialogo iniziale
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

                // Reset del pitch
                if (audioSource != null)
                {
                    audioSource.pitch = 1f;
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

            // Parte l'audio
            if (clickSound != null && audioSource != null)
                audioSource.PlayOneShot(clickSound);

            // Attiva la vibrazione se siamo al frame specificato
            if (currentIndex == vibrationFrameIndex)
            {
                isVibrating = true;
                vibrationTimer = 0f;
                basePosition = transform.position; // Aggiorna la posizione base

                // riproduce il suono del blender
                if (vibrationSound != null && audioSource != null && currentIndex < spriteDurations.Length)
                {
                    // Calcola il pitch per far durare il suono quanto la vibrazione
                    float vibrationDuration = spriteDurations[currentIndex];
                    float originalDuration = vibrationSound.length;
                    float pitchAdjustment = originalDuration / vibrationDuration;

                    audioSource.pitch = pitchAdjustment;
                    audioSource.PlayOneShot(vibrationSound);
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
        }
        ;

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