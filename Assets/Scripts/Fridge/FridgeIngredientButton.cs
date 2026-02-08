using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JetBrains.Annotations;

public class FridgeIngredientButton : MonoBehaviour
{
    [Header("Riferimenti")]
    public Image iceOverlay; // Overlay del ghiaccio
    public Image ingredientImage; // L'immagine dell'ingrediente
    public GameObject highlightEffect; // Effetto outline/glow

    [Header("Sprite Ghiaccio")]
    public Sprite[] iceSprites; // Array di sprite per i vari stati
    public bool useSpriteChange = true;

    [Header("Impostazioni Ghiaccio")]
    public Color iceColor = new Color(0.7f, 0.9f, 1f, 0.85f);

    [Header("Audio")]
    public AudioClip iceClickSound;
    public AudioClip iceBreakSound;

    private int clicksRemaining;
    private int maxClicks;
    private bool isDefrosted = false;
    private bool isHighlighted = false;
    private Button button;
    private FridgeDefrostGame gameManager;
    private AudioSource audioSource;



    void Awake()
    {
        //recuperi il button
        //Debug.Log($"[{gameObject.name}] Awake chiamato");
        button = GetComponent<Button>();

        //aggiungi il listener
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
            //Debug.Log($"[{gameObject.name}] Button trovato e listener aggiunto");
        }
        //else
        //{
        //    Debug.LogError($"[{gameObject.name}] BUTTON NON TROVATO!");
        //}

        //trova il FridgeDefrostGame
        gameManager = FindFirstObjectByType<FridgeDefrostGame>();

        //if (gameManager != null)
        //{
        //    Debug.Log($"[{gameObject.name}] GameManager trovato");
        //}
        //else
        //{
        //    Debug.LogError($"[{gameObject.name}] GAMEMANAGER NON TROVATO!");
        //}

        //prepari highlightEffect
        if (highlightEffect != null)
            highlightEffect.SetActive(false);

        //recuperi l'audio source
        audioSource = GetComponent<AudioSource>();

        //gestisci AudioSource aggiungendolo se è null
        //e gli impedisce di riprodurre automaticamente un suono all'avvio
        //o quando il game object viene attivato
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

    }

    //funzione per inizializzare il gioco
    public void InitializeForMinigame(int clicks)
    {
        StopAllCoroutines();
        //resetta i click
        maxClicks = clicks;
        clicksRemaining = clicks;
        isDefrosted = false;

        // Imposta il ghiaccio completamente visibile
        if (iceOverlay != null)
        {
            iceOverlay.gameObject.SetActive(true);
            iceOverlay.color = iceColor;
            iceOverlay.transform.localScale = Vector3.one;

            //reset sprite ghiaccio
            if(useSpriteChange && iceSprites != null && iceSprites.Length > 0)
            {
                iceOverlay.sprite = iceSprites[0];  //sprite integro 
            }
        }

        if(highlightEffect != null)
           highlightEffect.SetActive(false);

        // imposta il bottone come sempre cliccabile
        if (button != null)
            button.interactable = true;
    }

    //funzione per controllare se ingrediente è già stato scongelato-> non viene mai evidenziato
    public void Highlight(float duration)
    {
        if (isDefrosted) return;

        isHighlighted = true;
        StartCoroutine(HighlightCoroutine(duration));
    }

    //funzione per evidenziare l'ingrediente da scongelare
    IEnumerator HighlightCoroutine(float duration)
    {
        isHighlighted = true;

        // Attiva l'effetto visivo
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true);
        }
        else
        {
            // Effetto pulse con scale
            StartCoroutine(PulseEffect(duration));
        }

        yield return new WaitForSeconds(duration);

        isHighlighted = false;

        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }
    }

    //effetto di pulsazione dell'ingrediente
    IEnumerator PulseEffect(float duration)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.15f;

        float elapsed = 0f;
        float pulseDuration = duration / 2f;

        // Scale up
        while (elapsed < pulseDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / pulseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        // Scale down
        while (elapsed < pulseDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / pulseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    //funzione che interagisce con il game manager di questo gioco
    void OnButtonClick()
    {
        if (gameManager != null)
        {
            gameManager.OnIngredientClicked(this);
        }
    }

    public bool OnClick()
    {
        //il giocatore può cliccare solo quando è evidenziato
        if (isDefrosted || !isHighlighted)
           return false;
        //se il click è valido decrementa il numero di click rimanenti
        clicksRemaining--;
        //attiva il suono
        if (iceClickSound != null)
        {
            audioSource.PlayOneShot(iceClickSound);
        }

        //Debug.Log($"[{gameObject.name}]Click registrato! Click rimanenti: {clicksRemaining}/{maxClicks}");

        UpdateIceOpacity();

        if (clicksRemaining <= 0)
        {
            //imposta isDefrosted
            //Debug.Log($"[{gameObject.name}] Ghiaccio completamente scongelato!");
            isDefrosted = true;
            //fa partire il suono di rottura
            if (iceBreakSound != null)
            {
                audioSource.PlayOneShot(iceBreakSound);
            }
            //avvia l'animazione
            StartCoroutine(DefrostEffect());
        }

        return true;
    }

    void UpdateIceOpacity()
    {
        if (iceOverlay == null)
        {
            //Debug.LogError($"[{gameObject.name}] IceOverlay è NULL!");
            return;
        }

        // usiamo il cambio di sprite
        if (useSpriteChange && iceSprites != null && iceSprites.Length > 0)
        {
            // Calcola quale sprite usare in base ai click rimanenti
            int spriteIndex = maxClicks - clicksRemaining;

            // Assicura che l'indice sia valido
            if (spriteIndex >= 0 && spriteIndex < iceSprites.Length)
            {
                iceOverlay.sprite = iceSprites[spriteIndex];
                //Debug.Log($"[{gameObject.name}] Cambio sprite: {iceSprites[spriteIndex].name} (click: {clicksRemaining}/{maxClicks})");
            }
        }
        else
        {
            //fallback con alpha fade
            float progress = (float)clicksRemaining / maxClicks;
            Color newColor = iceOverlay.color;
            newColor.a = progress;
            iceOverlay.color = newColor;

            //Debug.Log($"[{gameObject.name}] Alpha aggiornato: {newColor.a} (click: {clicksRemaining}/{maxClicks})");
        }
    }

    IEnumerator DefrostEffect()
    {
        //fa partire il suono
        if (iceBreakSound != null)
        {
            audioSource.PlayOneShot(iceBreakSound);
        }

        // Effetto di "rottura" del ghiaccio
        if (iceOverlay != null)
        {
            //Debug.Log($"[{gameObject.name}] Ghiaccio completamente rotto!");

            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 originalScale = iceOverlay.transform.localScale;
            Color startColor = iceOverlay.color;

            while (elapsed < duration)
            {
                float t = elapsed / duration;

                // Scala aumenta leggermente
                iceOverlay.transform.localScale = originalScale * (1f + t * 0.3f);

                // Fade out
                Color newColor = startColor;
                newColor.a = startColor.a * (1 - t);
                iceOverlay.color = newColor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            iceOverlay.gameObject.SetActive(false);
            iceOverlay.transform.localScale = originalScale;
        }
    }

    //viene passato al game manager del minigame per indicare quale ingrediente deve essere eliminato dalla lista
    public bool IsDefrosted()
    {
        return isDefrosted;
    }
}