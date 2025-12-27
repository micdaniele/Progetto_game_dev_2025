using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance;

    private int selectedMood = -1;
    private string selectedRecipe = "";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // === SALVATAGGIO ===
    public void SetMood(int mood)
    {
        selectedMood = mood;
        Debug.Log("[GameManager] Mood salvato: " + mood);
    }

    public void SetRecipe(string recipe)
    {
        selectedRecipe = recipe;
        Debug.Log("[GameManager] Ricetta salvata: " + recipe);
    }

    // === LETTURA ===
    public int GetMood() => selectedMood;
    public string GetRecipe() => selectedRecipe;

    public bool HasSelection()
    {
        return selectedMood >= 0 && !string.IsNullOrEmpty(selectedRecipe);
    }
}
