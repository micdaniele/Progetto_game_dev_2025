using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FridgeIngredientButton : MonoBehaviour
{
    [Header("Riferimenti")]
    public Image iceOverlay; // Overlay del ghiaccio
    public Image ingredientImage; // L'immagine dell'ingrediente
    public GameObject highlightEffect; // Effetto outline/glow (opzionale)

    [Header("Sprite Ghiaccio")]
    public Sprite[] iceSprites; // Array di sprite per i vari stati
    public bool useSpriteChange = true; // Usa cambio sprite invece di alpha

    [Header("Impostazioni Ghiaccio")]
    public Color iceColor = new Color(0.7f, 0.9f, 1f, 0.85f);

    private int clicksRemaining;
    private int maxClicks;
    private bool isDefrosted = false;
    private Button button;
    private FridgeDefrostGame gameManager;

    void Awake()
    {
        Debug.Log($"[{gameObject.name}] Awake chiamato");
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
            Debug.Log($"[{gameObject.name}] Button trovato e listener aggiunto");
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] BUTTON NON TROVATO!");
        }

        gameManager = FindFirstObjectByType<FridgeDefrostGame>();

        if (gameManager != null)
        {
            Debug.Log($"[{gameObject.name}] GameManager trovato");
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] GAMEMANAGER NON TROVATO!");
        }

        if (highlightEffect != null)
            highlightEffect.SetActive(false);
    }

    public void InitializeForMinigame(int clicks)
    {
        maxClicks = clicks;
        clicksRemaining = clicks;
        isDefrosted = false;

        // Imposta il ghiaccio completamente visibile
        if (iceOverlay != null)
        {
            iceOverlay.gameObject.SetActive(true);
            iceOverlay.color = iceColor;
        }

        // Il bottone è sempre cliccabile
        if (button != null)
            button.interactable = true;
    }

    public void Highlight(float duration)
    {
        if (isDefrosted) return;

        StartCoroutine(HighlightCoroutine(duration));
    }

    IEnumerator HighlightCoroutine(float duration)
    {
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

        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }
    }

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

    void OnButtonClick()
    {
        if (gameManager != null)
        {
            gameManager.OnIngredientClicked(this);
        }
    }

    public bool OnClick()
    {
        if (isDefrosted)
        {
            Debug.Log($"[{gameObject.name}] Click ignorato: GIÀ scongelato");
            return false;
        }

        // ACCETTA SEMPRE I CLICK - nessun controllo su highlight
        clicksRemaining--;
        Debug.Log($"[{gameObject.name}]Click registrato! Click rimanenti: {clicksRemaining}/{maxClicks}");

        UpdateIceOpacity();

        if (clicksRemaining <= 0)
        {
            Debug.Log($"[{gameObject.name}] Ghiaccio completamente scongelato!");
            isDefrosted = true;
            StartCoroutine(DefrostEffect());
        }

        return true;
    }

    void UpdateIceOpacity()
    {
        if (iceOverlay == null)
        {
            Debug.LogError($"[{gameObject.name}] IceOverlay è NULL!");
            return;
        }

        // Se usiamo cambio sprite
        if (useSpriteChange && iceSprites != null && iceSprites.Length > 0)
        {
            // Calcola quale sprite usare in base ai click rimanenti
            int spriteIndex = maxClicks - clicksRemaining;

            // Assicurati che l'indice sia valido
            if (spriteIndex >= 0 && spriteIndex < iceSprites.Length)
            {
                iceOverlay.sprite = iceSprites[spriteIndex];
                Debug.Log($"[{gameObject.name}] Cambio sprite: {iceSprites[spriteIndex].name} (click: {clicksRemaining}/{maxClicks})");
            }
        }
        else
        {
            float progress = (float)clicksRemaining / maxClicks;
            Color newColor = iceOverlay.color;
            newColor.a = progress;
            iceOverlay.color = newColor;

            Debug.Log($"[{gameObject.name}] Alpha aggiornato: {newColor.a} (click: {clicksRemaining}/{maxClicks})");
        }
    }

    IEnumerator DefrostEffect()
    {
        // Effetto di "rottura" del ghiaccio
        if (iceOverlay != null)
        {
            Debug.Log($"[{gameObject.name}] Ghiaccio completamente rotto!");

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

    public bool IsDefrosted()
    {
        return isDefrosted;
    }
}