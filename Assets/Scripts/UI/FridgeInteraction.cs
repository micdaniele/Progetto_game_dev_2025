using UnityEngine;
using UnityEngine.SceneManagement;


public class FridgeInteraction : MonoBehaviour
{
    
    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";
    
    [Header("Scene")]
    public string fridgeSceneName = "Fridge";
    
    private bool playerInside = false;
    
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            Debug.Log("[FridgeInteraction] Player entered fridge zone");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            Debug.Log("[FridgeInteraction] Player left fridge zone");
        }
    }
    
    void Update()
    {
        // se player è dentro il trigger E preme il tasto
        if (playerInside && Input.GetKeyDown(interactKey))
        {
            OpenFridge();
        }
    }
    
    void OpenFridge()
    {
        Debug.Log("[FridgeInteraction] Opening fridge scene!");
        
        // Carica la scena del frigo
        SceneManager.LoadScene(fridgeSceneName);
    }
}