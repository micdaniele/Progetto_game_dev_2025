using UnityEngine;

public class StartMemory : MonoBehaviour
{
    public GameObject startPanel;

    public void StartGame()
    {
        startPanel.SetActive(false);
    }
}
