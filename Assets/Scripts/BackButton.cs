using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public string kitchenSceneName = "Kitchen2";

    public void GoBackToKitchen()
    {
        Debug.Log("[BackButton] Torno alla cucina");
        SceneManager.LoadScene(kitchenSceneName);
    }
}
