using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int selectedMood = -1;
    private string selectedRecipe = "";

    // Zaino ingredienti
    public List<string> ingredientiPresi = new List<string>();
    

    // Salva la posizione del player
    private Dictionary<string, bool> kitchenObjectsState = new Dictionary<string, bool>();
    private List<string> completedTasks = new List<string>();
    private Vector3 playerPosition;
    private bool hasPlayerPosition = false;


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

   

    // Salva se l'ingrediente è stato già preso

    public void SaveObjectState(string objectId, bool isActive)
    {
        if (kitchenObjectsState.ContainsKey(objectId))
            kitchenObjectsState[objectId] = isActive;
        else
            kitchenObjectsState.Add(objectId, isActive);

        Debug.Log($"[GameManager] Stato salvato -> {objectId}: {isActive}");
    }



    //Vede se il minigioco è stato completato
    public void CompleteTask(string taskName)
    {
        if (!completedTasks.Contains(taskName))
        {
            completedTasks.Add(taskName);
            Debug.Log($"[GameManager] Task completato: {taskName}");
        }
    }

    public bool IsTaskCompleted(string taskName)
    {
        return completedTasks.Contains(taskName);
    }

    public void SavePlayerPosition(Vector3 position)
    {
        playerPosition = position;
        hasPlayerPosition = true;
        Debug.Log($"[GameManager] Posizione player salvata: {position}");
    }

   
    public bool HasSavedPlayerPosition() => hasPlayerPosition;

    public void ResetKitchenState()
    {
        kitchenObjectsState.Clear();
        completedTasks.Clear();
        hasPlayerPosition = false;

        Debug.Log("[GameManager] RESET STATO CUCINA");
    }

    public void ResetAllGameState()
    {
        selectedMood = -1;
        selectedRecipe = "";
        ingredientiPresi.Clear();
        kitchenObjectsState.Clear();
        completedTasks.Clear();
        hasPlayerPosition = false;

        Debug.Log("[GameManager] RESET COMPLETO");
    }

    public void PrintCurrentState()
    {
        Debug.Log("=== GAMEMANAGER STATE ===");
        Debug.Log($"Mood: {selectedMood}");
        Debug.Log($"Recipe: {selectedRecipe}");
        Debug.Log($"Ingredienti Presi: {ingredientiPresi.Count}");
        Debug.Log($"Oggetti Salvati: {kitchenObjectsState.Count}");
        Debug.Log($"Tasks Completati: {completedTasks.Count}");
        Debug.Log($"Ha Posizione Player: {hasPlayerPosition}");
        Debug.Log("========================");
    }

    //Set
    public void SetSelection(int mood, string recipe)
    {
        selectedMood = mood;
        selectedRecipe = recipe;

        // Svuota lo zaino quando inizi una nuova ricetta
        ingredientiPresi.Clear();

        // Reset anche lo stato della cucina
        ResetKitchenState();

        Debug.Log($"[GameManager] Nuova partita -> Mood: {mood}, Ricetta: {recipe}");
        Debug.Log("[GameManager] Inventario svuotato.");
    }


    public void SetMood(int mood)
    {
        selectedMood = mood;
        ingredientiPresi.Clear();
        Debug.Log($"[GameManager] Mood impostato: {mood}");
    }

    public void SetRecipe(string recipe)
    {
        selectedRecipe = recipe;
        Debug.Log($"[GameManager] Ricetta impostata: {recipe}");
    }


    //Get
    public int GetCurrentMood() => selectedMood;
    public string GetCurrentRecipe() => selectedRecipe;

    public bool HasValidSelection()
    {
        return selectedMood >= 0 && !string.IsNullOrEmpty(selectedRecipe);
    }

    public Vector3 GetPlayerPosition() => playerPosition;

    // Legge se è già stato raccolto e sin caso lo disattiva nella scena frigo/dispenza
    public bool GetObjectState(string objectId, bool defaultState = true)
    {
        if (kitchenObjectsState.ContainsKey(objectId))
            return kitchenObjectsState[objectId];
        return defaultState;
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
}