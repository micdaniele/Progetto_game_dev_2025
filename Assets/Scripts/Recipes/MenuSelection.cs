using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuSelection : MonoBehaviour
{
    [Header("UI Elements")]
    public TMPro.TextMeshProUGUI moodText;
    public TMPro.TextMeshProUGUI recipeText;
    public TMPro.TextMeshProUGUI feedbackText;
    public Button startButton;

    [Header("Recipe Display")]
    public Transform recipeButtonsContainer;

    [Header("Scene Settings")]
    public string gameSceneName = "GameScene";

    private int selectedMood = -1; // 0=Happy, 1=Angry, 2=Sad, 3=Sick
    private string selectedRecipe = "";
    private RecipeDatabase currentDatabase;

    void Start()
    {
        if (startButton != null)
        {
            startButton.interactable = false;
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        UpdateFeedback("Seleziona un mood per iniziare");
    }

    // ============ METODI PER BOTTONI MOOD ============

    // Questi metodi vanno collegati ai bottoni Mood nell'Inspector
    public void SelectHappyMood()
    {
        SelectMood(0); // Happy = 0
    }

    public void SelectAngryMood()
    {
        SelectMood(1); // Angry = 1
    }

    public void SelectSadMood()
    {
        SelectMood(2); // Sad = 2
    }

    public void SelectSickMood()
    {
        SelectMood(3); // Sick = 3
    }

    // Metodo generico per selezionare il mood
    private void SelectMood(int mood)
    {
        selectedMood = mood;
        selectedRecipe = "";

        // Crea il database appropriato
        switch (mood)
        {
            case 0: // Happy
                currentDatabase = new HappyRecipes();
                break;
            case 1: // Angry
                currentDatabase = new AngryRecipes();
                break;
            case 2: // Sad
                currentDatabase = new SadRecipes();
                break;
            case 3: // Sick
                currentDatabase = new SickRecipes();
                break;
        }

        string moodName = GetMoodName(mood);

        // Aggiorna UI
        if (moodText != null)
            moodText.text = $"Mood: {moodName}";

        if (recipeText != null)
            recipeText.text = "Ricetta: -";

        UpdateFeedback($"Mood {moodName} selezionato! Ora scegli una ricetta:");

        // Mostra le ricette disponibili
        ShowAvailableRecipes();

        // Disabilita il bottone Start finché non scegli una ricetta
        if (startButton != null)
            startButton.interactable = false;
    }

    // ============ GESTIONE RICETTE ============

    // Mostra i bottoni delle ricette disponibili
    private void ShowAvailableRecipes()
    {
        if (recipeButtonsContainer == null || currentDatabase == null)
            return;

        // Pulisci i bottoni esistenti
        foreach (Transform child in recipeButtonsContainer)
        {
            Destroy(child.gameObject);
        }

        // Crea un bottone per ogni ricetta
        var recipes = currentDatabase.GetRecipes();
        foreach (string recipeName in recipes.Keys)
        {
            CreateRecipeButton(recipeName);
        }
    }

    // Crea un bottone per una singola ricetta
    private void CreateRecipeButton(string recipeName)
    {
        GameObject buttonObj = new GameObject(recipeName);
        buttonObj.transform.SetParent(recipeButtonsContainer);

        RectTransform rt = buttonObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(250, 50);

        Button btn = buttonObj.AddComponent<Button>();
        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.9f, 0.9f, 0.9f);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        Text txt = textObj.AddComponent<Text>();
        txt.text = recipeName;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.black;
        txt.fontSize = 18;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        RectTransform txtRt = textObj.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;

        btn.onClick.AddListener(() => SelectRecipe(recipeName));
    }

    // Seleziona una ricetta (chiamato dai bottoni ricetta)
    private void SelectRecipe(string recipe)
    {
        selectedRecipe = recipe;

        // Aggiorna UI
        if (recipeText != null)
            recipeText.text = $"Ricetta: {recipe}";

        UpdateFeedback($"Perfetto! Pronto per iniziare con {recipe}");

        // Abilita il bottone Start
        if (startButton != null)
            startButton.interactable = true;
    }

    // ============ START GAME ============

    // Chiamato dal bottone Start
    private void OnStartButtonClicked()
    {
        if (selectedMood < 0 || string.IsNullOrEmpty(selectedRecipe))
        {
            UpdateFeedback("Seleziona sia mood che ricetta!");
            return;
        }

        //SALVA I DATI NEL GAMEMANAGER
        GameManager.Instance.SetMood(selectedMood);
        GameManager.Instance.SetRecipe(selectedRecipe);

        UpdateFeedback("Dati salvati");

        // Carica la scena del gioco
        SceneManager.LoadScene(gameSceneName);
    }

    // ============ UTILITY ============

    // Aggiorna il testo di feedback
    private void UpdateFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
        Debug.Log(message);
    }

    // Resetta le selezioni
    public void ResetSelections()
    {
        selectedMood = -1;
        selectedRecipe = "";

        if (moodText != null)
            moodText.text = "Mood: -";

        if (recipeText != null)
            recipeText.text = "Ricetta: -";

        if (startButton != null)
            startButton.interactable = false;

        // Pulisci i bottoni ricette
        if (recipeButtonsContainer != null)
        {
            foreach (Transform child in recipeButtonsContainer)
            {
                Destroy(child.gameObject);
            }
        }

        UpdateFeedback("Seleziona un mood per iniziare");
    }

    // Ottieni il nome del mood dall'indice
    private string GetMoodName(int mood)
    {
        switch (mood)
        {
            case 0: return "Happy";
            case 1: return "Angry";
            case 2: return "Sad";
            case 3: return "Sick";
            default: return "Unknown";
        }
    }
}