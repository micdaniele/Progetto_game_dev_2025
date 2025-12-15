using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("Kitchen");

    }
}

