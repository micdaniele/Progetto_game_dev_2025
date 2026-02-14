using UnityEngine;
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
    private Dictionary<string, bool> uiObjectsState = new Dictionary<string, bool>(); //UI persistenti
    public List<string> completedTasks = new List<string>();//lista delle task completate

    // Sistema di posizionamento del player
    private Vector2 playerPosition;//posizione del player prima di cambiare scena
    private bool hasPlayerPosition = false;//vede se si è salvata una posizione prim del cambio scena
    private bool shouldRestorePosition = false; // flag che indica se ripristinare la posizione alla prossima scena

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            // Se esiste già un'istanza diversa da questa, distruggi QUESTO oggetto
            //Debug.Log($"[GameManager] Già esiste un GameManager, distruggo il duplicato dalla scena");
            Destroy(gameObject);
            return;
        }

        //Non viene distrutto al cambio scena
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //Debug.Log($"[GameManager] Inizializzato come persistente");
        //Debug.Log($"[GameManager] Nome: {gameObject.name}");
    }

    // Salva se l'ingrediente è stato già preso
    public void SaveObjectState(string objectId, bool isActive)
    {
        if (kitchenObjectsState.ContainsKey(objectId))
            kitchenObjectsState[objectId] = isActive;
        else
            kitchenObjectsState.Add(objectId, isActive);

        //Debug.Log($"[GameManager] Stato salvato -> {objectId}: {isActive}");
    }

    //Salva lo stato di oggetti UI che non devono essere resettati
    public void SaveUIObjectState(string objectId, bool isActive)
    {
        if (uiObjectsState.ContainsKey(objectId))
            uiObjectsState[objectId] = isActive;
        else
            uiObjectsState.Add(objectId, isActive);

        //Debug.Log($"[GameManager] Stato UI salvato -> {objectId}: {isActive}");
    }

    // Vede se il minigioco è stato completato
    public void CompleteTask(string taskName)
    {
        if (!completedTasks.Contains(taskName))
        {
            completedTasks.Add(taskName);
            //Debug.Log($"[GameManager] Task completato: {taskName}");
        }
    }

    //Funzione utile per il completamento delle task
    public bool IsTaskCompleted(string taskName)
    {
        return completedTasks.Contains(taskName);
    }

    // Salva la posizione del player e attiva il flag di ripristino
    // Questo metodo viene chiamato quando il player sta per entrare in un minigioco o in un'altra area da cui dovrà tornare alla stessa posizione
    public void SavePlayerPosition(Vector2 position)
    {
        playerPosition = position;
        hasPlayerPosition = true;
        shouldRestorePosition = true; // Attiviamo il flag: alla prossima scena vogliamo ripristinare

        //Debug.Log($"[GameManager] Posizione player salvata: {position}");
        //Debug.Log($"[GameManager] Flag di ripristino ATTIVATO");
    }


    // Restituisce true solo se abbiamo una posizione salvata E vogliamo ripristinarla proprio ora
    public bool ShouldRestorePlayerPosition()
    {
        bool result = shouldRestorePosition && hasPlayerPosition;
        //Debug.Log($"[GameManager] ShouldRestorePlayerPosition? {result} (flag: {shouldRestorePosition}, hasPos: {hasPlayerPosition})");
        return result;
    }

    // Cancella il flag di ripristino dopo che è stato usato
    // Previene ripristini indesiderati nelle scene successive
    // Viene chiamato dal player subito dopo aver ripristinato la posizione
    public void ClearPositionRestore()
    {
        shouldRestorePosition = false;
        //Debug.Log("[GameManager] Flag di ripristino DISATTIVATO");
    }

    // Resetta lo stato della cucina
    public void ResetKitchenState()
    {
        kitchenObjectsState.Clear();
        hasPlayerPosition = false;
        shouldRestorePosition = false; 

        //Debug.Log("[GameManager] RESET STATO CUCINA completo - tutti i flag di posizione azzerati");
    }

    // Set
    public void SetSelection(int mood, string recipe)
    {
        selectedMood = mood;
        selectedRecipe = recipe;

        // Svuota lo zaino quando inizi una nuova ricetta
        ingredientiPresi.Clear();

        // Reset dello stato della cucina
        ResetKitchenState();

        //Debug.Log($"[GameManager] Nuova partita -> Mood: {mood}, Ricetta: {recipe}");
        //Debug.Log("[GameManager] Inventario svuotato e posizione resettata.");
    }

    public void SetMood(int mood)
    {
        selectedMood = mood;
        ingredientiPresi.Clear();
        //Debug.Log($"[GameManager] Mood impostato: {mood}");
    }

    public void SetRecipe(string recipe)
    {
        selectedRecipe = recipe;
        //Debug.Log($"[GameManager] Ricetta impostata: {recipe}");
    }

    // Get
    public int GetCurrentMood() => selectedMood;
    public string GetCurrentRecipe() => selectedRecipe;

    public Vector2 GetPlayerPosition() => playerPosition;

    public bool HasValidSelection()
    {
        return selectedMood >= 0 && !string.IsNullOrEmpty(selectedRecipe);
    }

    // Legge se è già stato raccolto e in caso lo disattiva nella scena frigo/dispensa
    public bool GetObjectState(string objectId, bool defaultState = true)
    {
        if (kitchenObjectsState.ContainsKey(objectId))
            return kitchenObjectsState[objectId];
        return defaultState;
    }

    // Legge lo stato degli oggetti UI
    public bool GetUIObjectState(string objectId, bool defaultState = true)
    {
        if (uiObjectsState.ContainsKey(objectId))
            return uiObjectsState[objectId];
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