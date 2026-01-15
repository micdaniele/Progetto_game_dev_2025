using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public string kitchenSceneName = "Kitchen2";
    public AudioClip button_click;

    public void GoBackToKitchen()
    {
        if (button_click != null)
            AudioSource.PlayClipAtPoint(button_click, Camera.main.transform.position);
            
        Debug.Log("[BackButton] Torno alla cucina");
        SceneManager.LoadScene(kitchenSceneName);
    }
}
