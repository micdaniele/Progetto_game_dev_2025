using UnityEngine;

public class MoodWindow : MonoBehaviour
{
    public GameObject moodContent;
    public GameObject recipeContent;

    // Metodo chiamato quando viene selezionato un mood
    public void SelectMood(int mood)
    {
        GameManager.Instance.SetMood(mood);

        moodContent.SetActive(false);
        recipeContent.SetActive(true);

        GameManager.Instance.PrepareRecipes();
    }

    // Metodo chiamato quando viene selezionata una ricetta
    public void SelectRecipe(string recipe)
    {
        GameManager.Instance.SetRecipe(recipe);
        Hide();
    }

    // Metodo per nascondere la finestra
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}