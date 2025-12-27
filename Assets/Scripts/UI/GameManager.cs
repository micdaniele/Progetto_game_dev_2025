using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int selectedMood = -1;
    private string selectedRecipe = "";

    void Awake()
    {
        // Singleton pattern
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[GameManager] Inizializzato e persistente tra le scene");
    }

    // === SALVATAGGIO ===
    public void SetMood(int mood)
    {
        selectedMood = mood;
        Debug.Log($"[GameManager] Mood salvato: {mood} ({GetMoodName(mood)})");
    }

    public void SetRecipe(string recipe)
    {
        selectedRecipe = recipe;
        Debug.Log($"[GameManager] Ricetta salvata: {recipe}");
    }

    // === LETTURA ===
    public int GetCurrentMood() => selectedMood;

    public string GetCurrentRecipe() => selectedRecipe;

    public bool HasValidSelection()
    {
        bool isValid = selectedMood >= 0 && !string.IsNullOrEmpty(selectedRecipe);
        Debug.Log($"[GameManager] HasValidSelection: {isValid} (Mood: {selectedMood}, Recipe: {selectedRecipe})");
        return isValid;
    }

    // Converte il numero del mood in nome
    public string GetMoodName(int mood)
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

    // Metodo per resettare la selezione (opzionale)
    public void ResetSelection()
    {
        selectedMood = -1;
        selectedRecipe = "";
        Debug.Log("[GameManager] Selezione resettata");
    }

    // Debug: stampa lo stato corrente
    public void PrintCurrentState()
    {
        Debug.Log("=== GAMEMANAGER STATE ===");
        Debug.Log($"Mood: {selectedMood} ({GetMoodName(selectedMood)})");
        Debug.Log($"Recipe: {selectedRecipe}");
        Debug.Log($"Valid: {HasValidSelection()}");
        Debug.Log("========================");
    }
}