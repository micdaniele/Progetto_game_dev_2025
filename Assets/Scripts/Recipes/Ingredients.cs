using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Ingredient : MonoBehaviour
{
    [Header("Ingredient Info")]
    public string ingredientName;

    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color selectableColor = Color.green;
    public Color selectedColor = Color.yellow;
    public Color disabledColor = Color.gray;

    [Header("Components")]
    private SpriteRenderer spriteRenderer;
    private RecipeManager recipeManager;

    private bool isSelectable = false;
    private bool isSelected = false;

    void Start()
    {
        // Ottieni il componente SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"SpriteRenderer non trovato su {gameObject.name}");
        }

        // Trova il RecipeManager nella scena
        recipeManager = Object.FindFirstObjectByType<RecipeManager>();

        if (recipeManager == null)
        {
            Debug.LogError("RecipeManager non trovato nella scena!");
        }

        // Inizializza come non selezionabile
        SetSelectable(false);
    }

    void OnMouseDown()
    {
        // Controlla se l'ingrediente può essere selezionato
        if (isSelectable && !isSelected)
        {
            TrySelect();
        }
        else if (!isSelectable)
        {
            // Feedback visivo o sonoro quando si clicca un ingrediente sbagliato
            StartCoroutine(ShakeEffect());
            Debug.Log($" {ingredientName} non è necessario per questa ricetta!");
        }
    }

    private void TrySelect()
    {
        if (recipeManager != null)
        {
            bool success = recipeManager.TrySelectIngredient(ingredientName);

            if (success)
            {
                isSelected = true;
                UpdateVisualState();

                // Opzionale: animazione o effetto
                StartCoroutine(SelectAnimation());
            }
        }
    }

    // Imposta se l'ingrediente può essere selezionato
    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
        isSelected = false;
        UpdateVisualState();
    }

    // Aggiorna l'aspetto visivo dell'ingrediente
    private void UpdateVisualState()
    {
        if (spriteRenderer == null) return;

        if (isSelected)
        {
            spriteRenderer.color = selectedColor;
        }
        else if (isSelectable)
        {
            spriteRenderer.color = selectableColor;
        }
        else
        {
            spriteRenderer.color = disabledColor;
        }
    }

    // Animazione quando viene selezionato
    private System.Collections.IEnumerator SelectAnimation()
    {
        Vector3 originalScale = transform.localScale;
        float duration = 0.2f;
        float elapsed = 0f;

        // Ingrandisci
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 1.2f, elapsed / duration);
            transform.localScale = originalScale * scale;
            yield return null;
        }

        elapsed = 0f;

        // Torna normale
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1.2f, 1f, elapsed / duration);
            transform.localScale = originalScale * scale;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    // Effetto shake per ingrediente sbagliato
    private System.Collections.IEnumerator ShakeEffect()
    {
        Vector3 originalPos = transform.position;
        float duration = 0.3f;
        float magnitude = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-magnitude, magnitude);
            float y = originalPos.y + Random.Range(-magnitude, magnitude);

            transform.position = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }

    // Resetta l'ingrediente
    public void Reset()
    {
        isSelected = false;
        isSelectable = false;
        UpdateVisualState();
    }

    // Per debug
    void OnMouseEnter()
    {
        if (isSelectable && !isSelected)
        {
            // Opzionale: effetto hover
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(selectableColor, Color.white, 0.5f);
            }
        }
    }

    void OnMouseExit()
    {
        UpdateVisualState();
    }
}