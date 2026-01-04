using UnityEngine;
using UnityEngine.UI;

public class Ingredient : MonoBehaviour
{
    [Header("Configurazione")]
    public string ingredientName; // DEVE ESSERE IDENTICO AL DATABASE

    [Header("Colori")]
    public Color normalColor = Color.white;
    public Color selectableColor = Color.green; // Verde = Serve
    public Color selectedColor = Color.yellow;  // Giallo = Preso
    public Color disabledColor = Color.gray;    // Grigio = Inutile

    private Image btnImage;
    private Button btn;
    private RecipeManager recipeManager;

    private bool isSelectable = false;
    private bool isSelected = false;

    void OnEnable()
    {
        btnImage = GetComponent<Image>();
        btn = GetComponent<Button>();
        recipeManager = Object.FindFirstObjectByType<RecipeManager>();

        // Auto-collegamento del click
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }
    }
    void OnDisable()
{
    if (btn != null)
    {
        btn.onClick.RemoveListener(OnClick);
    }
}

    // Chiamato dal RecipeManager per gli ingredienti NON ancora presi
    public void SetSelectable(bool selectable)
    {
        // NON resettare isSelected se � gi� true!
        if (!isSelected)
        {
            isSelectable = selectable;
            isSelected = false;
        }
        UpdateVisual();
    }

    // Chiamato dal RecipeManager per gli ingredienti GIA' presi
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selected)
        {
            isSelectable = false; // Se gi� preso, non � pi� selezionabile
        }
        UpdateVisual();
    }

    public void OnClick()
    {
        if (isSelected)
        {
            Debug.Log($"[Ingredient] {ingredientName} e gia stato preso!");
            return;
        }

        if (isSelectable)
        {
            // SE E' GIUSTO
            if (recipeManager != null && recipeManager.TrySelectIngredient(ingredientName))
            {
                isSelected = true;
                isSelectable = false;
                UpdateVisual();
            }
        }
        else
        {
            // SE E' SBAGLIATO
            Debug.Log($"[Ingredient] Errore: {ingredientName} non serve!");
            // TODO: Aggiungi animazione di errore
        }
    }

    void UpdateVisual()
    {
        if (btnImage == null) return;

        if (isSelected)
        {
            // GIA PRESO = GIALLO
            btnImage.color = selectedColor;
            if (btn != null) btn.interactable = false; // Disabilita il bottone
        }
        else if (isSelectable)
        {
            // DA PRENDERE = VERDE
            btnImage.color = selectableColor;
            if (btn != null) btn.interactable = true;
        }
        else
        {
            // NON SERVE = GRIGIO
            btnImage.color = disabledColor;
            if (btn != null) btn.interactable = false;
        }
    }

    // Per debug
    public bool IsSelected() => isSelected;
    public bool IsSelectable() => isSelectable;
}