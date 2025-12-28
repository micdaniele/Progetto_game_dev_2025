using UnityEngine;
using UnityEngine.UI; // Fondamentale per i Button

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

    void Awake()
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

    // Chiamato dal RecipeManager all'avvio
    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
        isSelected = false;
        UpdateVisual();
    }

    public void OnClick()
    {
        if (isSelected) return; // Già preso, non fare nulla

        if (isSelectable)
        {
            // È GIUSTO!
            if (recipeManager.TrySelectIngredient(ingredientName))
            {
                isSelected = true;
                UpdateVisual();
            }
        }
        else
        {
            // È SBAGLIATO!
            Debug.Log($"Errore: {ingredientName} non serve!");
            // Qui puoi aggiungere animazione di errore
        }
    }

    void UpdateVisual()
    {
        if (btnImage == null) return;

        if (isSelected) btnImage.color = selectedColor;
        else if (isSelectable) btnImage.color = selectableColor;
        else btnImage.color = disabledColor;
    }
}