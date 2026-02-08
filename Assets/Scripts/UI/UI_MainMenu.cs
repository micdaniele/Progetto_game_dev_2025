using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    //funzione per far partire il gioco e portare alla cucina
    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("Kitchen2");
    }
}
