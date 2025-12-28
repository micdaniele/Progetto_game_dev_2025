using UnityEngine;
using System.Collections.Generic; // Serve per le Liste!

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int selectedMood = -1;
    private string selectedRecipe = "";

    // === LO ZAINO (LA VARIABILE CHE MANCAVA) ===
    public List<string> ingredientiPresi = new List<string>();
    // ===========================================

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

    // === METODO NUOVO (Consigliato) ===
    public void SetSelection(int mood, string recipe)
    {
        selectedMood = mood;
        selectedRecipe = recipe;

        // Svuota lo zaino quando inizi una nuova ricetta
        ingredientiPresi.Clear();

        Debug.Log($"[GameManager] Nuova partita -> Mood: {mood}, Ricetta: {recipe}");
        Debug.Log("[GameManager] Inventario svuotato.");
    }

    // === METODI VECCHI (Aggiunti per compatibilità con MoodWindow) ===
    // Questi metodi servono per non far dare errore al tuo MoodWindow.cs

    public void SetMood(int mood)
    {
        selectedMood = mood;
        ingredientiPresi.Clear(); // Reset anche qui per sicurezza
        Debug.Log($"[GameManager] Mood impostato: {mood}");
    }

    public void SetRecipe(string recipe)
    {
        selectedRecipe = recipe;
        // Nota: non puliamo la lista qui perché di solito chiami SetMood prima
        Debug.Log($"[GameManager] Ricetta impostata: {recipe}");
    }
    // ================================================================

    // === LETTURA ===
    public int GetCurrentMood() => selectedMood;

    public string GetCurrentRecipe() => selectedRecipe;

    public bool HasValidSelection()
    {
        return selectedMood >= 0 && !string.IsNullOrEmpty(selectedRecipe);
    }

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

    public void PrintCurrentState()
    {
        Debug.Log("=== GAMEMANAGER STATE ===");
        Debug.Log($"Mood: {selectedMood}");
        Debug.Log($"Recipe: {selectedRecipe}");
        Debug.Log($"Ingredienti Presi: {ingredientiPresi.Count}");
        Debug.Log("========================");
    }
}