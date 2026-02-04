using UnityEngine;
using UnityEngine.SceneManagement;

public class win_script : MonoBehaviour
{
    public void OnChangeScene()
    {
        SceneManager.LoadScene("Dish_scene");
    }
}
