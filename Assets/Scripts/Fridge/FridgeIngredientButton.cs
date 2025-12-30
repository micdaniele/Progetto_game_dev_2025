using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FridgeIngredientButton : MonoBehaviour
{
    [Header("Riferimenti")]
    public Image iceOverlay; // Overlay del ghiaccio
    public Image ingredientImage; // L'immagine dell'ingrediente
    public GameObject highlightEffect; // Effetto outline/glow (opzionale)
    
    [Header("Impostazioni Ghiaccio")]
    public Color iceColor = new Color(0.7f, 0.9f, 1f, 0.85f);
    
    [Header("Effetti Particelle (opzionale)")]
    public ParticleSystem defrostParticles; // Sistema particelle quando si scongela
    
    private int clicksRemaining;
    private int maxClicks;
    private bool isHighlighted = false;
    private bool isDefrosted = false;
    private Button button;
    private FridgeDefrostGame gameManager;
    
    void Awake()
    {
        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        
        gameManager = FindFirstObjectByType<FridgeDefrostGame>();
        
        if (highlightEffect != null)
            highlightEffect.SetActive(false);
    }
    
    public void InitializeForMinigame(int clicks)
    {
        maxClicks = clicks;
        clicksRemaining = clicks;
        isDefrosted = false;
        isHighlighted = false;
        
        // Imposta il ghiaccio completamente visibile
        if (iceOverlay != null)
        {
            iceOverlay.gameObject.SetActive(true);
            iceOverlay.color = iceColor;
        }
        
        // Il bottone è cliccabile solo quando è illuminato
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
        // Ritorna true se il click è valido
        if (!isHighlighted || isDefrosted) return false;
        
        clicksRemaining--;
        
        // Aggiorna l'opacità del ghiaccio
        UpdateIceOpacity();
        
        // Check se è completamente scongelato
        if (clicksRemaining <= 0)
        {
            isDefrosted = true;
            StartCoroutine(DefrostEffect());
        }
        
        return true;
    }
    
    void UpdateIceOpacity()
    {
        if (iceOverlay == null) return;
        
        float progress = (float)clicksRemaining / maxClicks;
        Color newColor = iceOverlay.color;
        newColor.a = progress;
        iceOverlay.color = newColor;
    }
    
    IEnumerator DefrostEffect()
    {
        // Effetto di "rottura" del ghiaccio
        if (iceOverlay != null)
        {
            float duration = 0.4f;
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

