using UnityEngine;

public class MoodWindow : MonoBehaviour
{
    [Header("Panels")]
    public GameObject moodContent;
    public GameObject recipeContent;

    // Metodo chiamato quando viene selezionato un mood
    public void SelectMood(int mood)
    {
        Debug.Log($"[MoodWindow] Mood selezionato: {mood}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetMood(mood);

            // Nascondi il pannello mood e mostra quello ricette
            if (moodContent != null)
                moodContent.SetActive(false);

            if (recipeContent != null)
                recipeContent.SetActive(true);
        }
        else
        {
            Debug.LogError("[MoodWindow] GameManager.Instance è NULL!");
        }
    }

    // Metodo chiamato quando viene selezionata una ricetta
    public void SelectRecipe(string recipe)
    {
        Debug.Log($"[MoodWindow] Ricetta selezionata: {recipe}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetRecipe(recipe);

            // Debug: stampa lo stato del GameManager
            GameManager.Instance.PrintCurrentState();

            // Nascondi la finestra
            Hide();

            // Riprendi il gioco
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Debug.LogError("[MoodWindow] GameManager.Instance è NULL!");
        }
    }

    // Metodo per nascondere la finestra
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // Metodo per mostrare la finestra
    public void Show()
    {
        gameObject.SetActive(true);

        // Blocca il gioco quando apri la finestra
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Mostra il pannello mood all'inizio
        if (moodContent != null)
            moodContent.SetActive(true);

        if (recipeContent != null)
            recipeContent.SetActive(false);
    }
}