using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RecipeImageDisplay : MonoBehaviour
{
    [System.Serializable]
    public class RecipeImage
    {
        public string recipeName;
        public GameObject imageObject;
    }

    [Header("Recipe Images")]
    public List<RecipeImage> recipeImages = new List<RecipeImage>();

    [Header("Settings")]
    public bool hideAllOnStart = true;  // Nascondi tutte le immagini all'inizio
    public GameObject defaultImage;     // Immagine di default se la ricetta non è trovata

    [Header("Dialogue on Scene Start")]
    public bool showDialogueOnStart = true;  // Attiva il dialogo all'inizio della scena
    public Dialogue startDialogue;            // Dialogo da mostrare all'inizio

    void Start()
    {
        // Nascondi tutte le immagini all'inizio
        if (hideAllOnStart)
        {
            HideAllImages();
        }

        // Carica e mostra l'immagine della ricetta corrente
        DisplayCurrentRecipeImage();

        // Mostra il dialogo se richiesto
        if (showDialogueOnStart && startDialogue != null)
        {
            StartCoroutine(ShowDialogue());
        }
    }


    private IEnumerator ShowDialogue()
    {

        // Controlla se il DialogueManager esiste
        if (DialogueManager.Instance == null)
        {
            //Debug.LogWarning("[RecipeImageDisplay] DialogueManager non trovato!");
            yield break;
        }

        // Mostra il dialogo
        //Debug.Log($"[RecipeImageDisplay] Mostro dialogo all'inizio della scena");
        DialogueManager.Instance.StartDialogue(startDialogue);
    }


    // Mostra l'immagine della ricetta corrente
    public void DisplayCurrentRecipeImage()
    {
        if (GameManager.Instance == null)
        {
            //Debug.LogWarning("[RecipeImageDisplay] GameManager non trovato!");
            ShowDefaultImage();
            return;
        }

        // Ottieni la ricetta corrente dal GameManager
        string currentRecipe = GameManager.Instance.GetCurrentRecipe();

        if (string.IsNullOrEmpty(currentRecipe))
        {
            //Debug.LogWarning("[RecipeImageDisplay] Nessuna ricetta selezionata!");
            ShowDefaultImage();
            return;
        }

        // Cerca e mostra l'immagine corrispondente
        bool found = ShowRecipeImage(currentRecipe);

        if (!found)
        {
            //Debug.LogWarning($"[RecipeImageDisplay] Immagine per ricetta '{currentRecipe}' non trovata!");
            ShowDefaultImage();
        }
        //else
        //{
        //    Debug.Log($"[RecipeImageDisplay] Mostrata immagine per: {currentRecipe}");
        //}
    }


    // Mostra l'immagine di una ricetta specifica
    public bool ShowRecipeImage(string recipeName)
    {
        // Nascondi tutte le immagini prima
        HideAllImages();

        // Cerca la ricetta nella lista
        foreach (RecipeImage recipeImage in recipeImages)
        {
            if (recipeImage.recipeName == recipeName && recipeImage.imageObject != null)
            {
                // Attiva l'immagine
                recipeImage.imageObject.SetActive(true);
                return true;
            }
        }

        return false;
    }

    // Forzo uno stato iniziale in cui tutte le immagini delle ricette sono disattivate
    public void HideAllImages()
    {
        foreach (RecipeImage recipeImage in recipeImages)
        {
            if (recipeImage.imageObject != null)
            {
                recipeImage.imageObject.SetActive(false);
            }
        }

        // Nascondi anche l'immagine di default
        if (defaultImage != null)
        {
            defaultImage.SetActive(false);
        }
    }

    // Mostra l'immagine di default
    private void ShowDefaultImage()
    {
        if (defaultImage != null)
        {
            HideAllImages();
            defaultImage.SetActive(true);
            //Debug.Log("[RecipeImageDisplay] Mostrata immagine di default");
        }
    }


    // Metodo di debug per testare le immagini
    //public void DebugShowRecipe(string recipeName)
    //{
    //    Debug.Log($"[RecipeImageDisplay] DEBUG: Provo a mostrare '{recipeName}'");
    //    bool success = ShowRecipeImage(recipeName);
    //    Debug.Log($"[RecipeImageDisplay] DEBUG: Risultato = {success}");
    //}
}