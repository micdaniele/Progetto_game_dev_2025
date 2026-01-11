using UnityEngine;
using UnityEngine.UI;

public class Ingredient : MonoBehaviour
{
    [Header("Configurazione")]
    public string ingredientName;

    [Header("Colori")]
    public Color normalColor = Color.white;
    public Color selectableColor = Color.green; //Serve
    public Color selectedColor = Color.yellow; //Preso
    public Color disabledColor = Color.gray; //Non serve

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

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }

        //Ripristina lo stato
        RestoreState();
    }

    void OnDisable()
    {
        if (btn != null)
        {
            btn.onClick.RemoveListener(OnClick);
        }
    }

    void RestoreState()
    {
        if (GameManager.Instance == null) return;

        // Controlla se questo ingrediente è già stato raccolto
        string objectId = "Ingredient_" + ingredientName;
        bool wasCollected = !GameManager.Instance.GetObjectState(objectId, true);

        if (wasCollected)
        {
            // Se già raccolto, nascondi l'oggetto
            gameObject.SetActive(false);
            Debug.Log($"[Ingredient] {ingredientName} già raccolto, nascosto");
        }
    }

    public void SetSelectable(bool selectable)
    {
        if (!isSelected)
        {
            isSelectable = selectable;
            isSelected = false;
        }
        UpdateVisual();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selected)
        {
            isSelectable = false;
        }
        UpdateVisual();
    }

    public void OnClick()
    {
        if (isSelected) { Debug.Log($"[Ingredient] {ingredientName} già preso!"); return; }
        if (!isSelectable) { Debug.Log($"[Ingredient] {ingredientName} non serve!"); return; }

        if (recipeManager != null && recipeManager.TrySelectIngredient(ingredientName))
        {
            isSelected = true;
            isSelectable = false;
            GameManager.Instance?.SaveObjectState("Ingredient_" + ingredientName, false);
            gameObject.SetActive(false);
        }
    }

    void UpdateVisual()
    {
        if (btnImage == null) return;

        if (isSelected)
        {
            btnImage.color = selectedColor;
            if (btn != null) btn.interactable = false;
        }
        else if (isSelectable)
        {
            btnImage.color = selectableColor;
            if (btn != null) btn.interactable = true;
        }
        else
        {
            btnImage.color = disabledColor;
            if (btn != null) btn.interactable = false;
        }
    }

    public bool IsSelected() => isSelected;
    public bool IsSelectable() => isSelectable;
}