using UnityEngine;
using UnityEngine.UI;

public class Ingredient : MonoBehaviour
{
    [Header("Configurazione")]
    public string ingredientName;

    [Header("Colori")]
    public Color selectableColor = Color.green; //Serve
    public Color disabledColor = Color.gray; //Non serve

    [Header("Audio")]
    public AudioClip ingredient_click;      // suono quando prendi l’ingrediente

    private Image btnImage;
    private Button btn;
    private RecipeManager recipeManager;

    private bool isSelectable = false;
    private bool isSelected = false;

    void OnEnable()
    {
        //recuperi le immagini e i bottoni degli ingredienti
        btnImage = GetComponent<Image>();
        btn = GetComponent<Button>();
        //aggiorni lo stato degli ingredienti salvati
        recipeManager = Object.FindFirstObjectByType<RecipeManager>();

        //assegni listener
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }

        //controlla se l’ingrediente è già stato raccolto in passato, se sì -> lo nasconde
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
            //Debug.Log($"[Ingredient] {ingredientName} già raccolto, nascosto");
        }
    }

    //se non è stato già preso aggiorna se serve oppure no e la grafica
    public void SetSelectable(bool selectable)
    {
        if (!isSelected)
        {
            isSelectable = selectable;
            isSelected = false;
        }
        UpdateVisual();
    }

    //quando l’ingrediente viene scelto viene anche bloccato visivamente
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
        if (ingredient_click != null)
        {
            AudioSource.PlayClipAtPoint(ingredient_click, Camera.main.transform.position);
        }
        //if (isSelected) { Debug.Log($"[Ingredient] {ingredientName} già preso!"); return;}
        //if (!isSelectable) { Debug.Log($"[Ingredient] {ingredientName} non serve!"); return; }

        //chiede al game manager se è nella ricetta ed in base alla "risposta" lo salva nel manager, segni come preso o lo nascondi
        if (recipeManager != null && recipeManager.TrySelectIngredient(ingredientName))
        {
            isSelected = true;
            isSelectable = false;
            //tramite il null conditional operator controlla se instance esiste e poi esegue il metodo
            //altrimenti non farà niente
            GameManager.Instance?.SaveObjectState("Ingredient_" + ingredientName, false);
            gameObject.SetActive(false);
        }
    }

    //aggiorna i colori dei vari ingredienti
    void UpdateVisual()
    {
        if (btnImage == null) return;

        if (isSelectable)
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
    public bool IsSelectable() => isSelectable;
}