using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RecipeImageDisplay : MonoBehaviour
{
    [System.Serializable]
    //Permette di associare ogni nome di ricetta a un GameObject
    public class RecipeImage
    {
        public string recipeName;
        public GameObject imageObject;
    }

    [Header("Recipe Images")]
    public List<RecipeImage> recipeImages = new List<RecipeImage>();//lista delle ricette e dei rispettivi GameObject

    [Header("Settings")]
    public bool hideAllOnStart = true;//se true, tutte le immagini saranno nascoste all’inizio
    public GameObject defaultImage;//immagine mostrata se la ricetta corrente non ha un’immagine associata

    [Header("Dialogue on Scene Start")]
    public bool showDialogueOnStart = true;//indica se mostrare un dialogo all’avvio della scena
    public Dialogue startDialogue;

    [Header("Exit Button")]
    public GameObject exitButton;  // Riferimento al bottone
    public bool showExitAfterDialogue = true;  // Attiva/disattiva funzionalità

    void Start()
    {
        // Nascondi il bottone all'inizio
        if (exitButton != null)
        {
            exitButton.SetActive(false);
        }

        if (hideAllOnStart)
        {
            HideAllImages();
        }

        DisplayCurrentRecipeImage();

        if (showDialogueOnStart && startDialogue != null)
        {
            StartCoroutine(ShowDialogue());//Avvia il dialogo se richiesto
        }
    }

    //fa partire il dialogo.
    private IEnumerator ShowDialogue()
    {
        if (DialogueManager.Instance == null)
        {
            yield break;
        }

        // Mostra il dialogo
        DialogueManager.Instance.StartDialogue(startDialogue);

        // Aspetta che il dialogo finisca
        if (showExitAfterDialogue)
        {
            yield return StartCoroutine(WaitForDialogueEnd());

            // Mostra il bottone quando il dialogo è terminato
            if (exitButton != null)
            {
                exitButton.SetActive(true);
            }
        }
    }

    // Coroutine che aspetta la fine del dialogo
    private IEnumerator WaitForDialogueEnd()
    {
        // Aspetta finché il pannello del dialogo è attivo
        if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel != null)
        {
            while (DialogueManager.Instance.dialoguePanel.activeSelf)
            {
                yield return null;
            }
        }
    }

    //funzioni che assicurino che sia visualizzata almeno un'immagine
    public void DisplayCurrentRecipeImage()
    {
        if (GameManager.Instance == null)
        {
            ShowDefaultImage();
            return;
        }

        string currentRecipe = GameManager.Instance.GetCurrentRecipe();

        if (string.IsNullOrEmpty(currentRecipe))
        {
            ShowDefaultImage();
            return;
        }

        bool found = ShowRecipeImage(currentRecipe);

        if (!found)
        {
            ShowDefaultImage();
        }
    }

    //funzione che controlla l'immagine della ricetta da mostrare
    public bool ShowRecipeImage(string recipeName)
    {
        HideAllImages();

        foreach (RecipeImage recipeImage in recipeImages)
        {
            if (recipeImage.recipeName == recipeName && recipeImage.imageObject != null)
            {
                recipeImage.imageObject.SetActive(true);
                return true;
            }
        }

        return false;
    }

    //funzione che nasconde tutte le immagini delle ricette
    public void HideAllImages()
    {
        foreach (RecipeImage recipeImage in recipeImages)
        {
            if (recipeImage.imageObject != null)
            {
                recipeImage.imageObject.SetActive(false);
            }
        }

        if (defaultImage != null)
        {
            defaultImage.SetActive(false);
        }
    }

    //mostra un'immagine scelta precedentemente
    private void ShowDefaultImage()
    {
        if (defaultImage != null)
        {
            HideAllImages();
            defaultImage.SetActive(true);
        }
    }
}