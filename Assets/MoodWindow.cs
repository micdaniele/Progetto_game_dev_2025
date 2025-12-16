using UnityEngine;

public class MoodWindow : MonoBehaviour
{
    public GameObject moodContent;
    public GameObject recipeContent;

    public void SelectMood(string mood)
    {
        GameManager.Instance.SetMood(mood);

        moodContent.SetActive(false);
        recipeContent.SetActive(true);

        GameManager.Instance.PrepareRecipes();
    }

    public void SelectRecipe(string recipe)
    {
        GameManager.Instance.SetRecipe(recipe);
        Hide();
    }
}
