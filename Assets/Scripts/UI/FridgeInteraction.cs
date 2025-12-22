using UnityEngine;
using UnityEngine.SceneManagement;


public class FridgeInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Tasto per interagire")]
    public KeyCode interactionKey = KeyCode.E;
    
    [Tooltip("Distanza massima per interagire")]
    public float interactionDistance = 2f;
    
    [Header("Scene Settings")]
    [Tooltip("Nome della scena del frigo")]
    public string fridgeSceneName = "Fridge";
    
    private Transform player;
    private bool playerInRange = false;
    
    void Start()
    {
        // Trova il player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("[FridgeInteraction] Player not found! Tag your player as 'Player'");
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Calcola distanza
        float distance = Vector3.Distance(transform.position, player.position);
        
        // Player nel raggio?
        playerInRange = (distance <= interactionDistance);
        
        // Se player è nel raggio e preme E
        if (playerInRange && Input.GetKeyDown(interactionKey))
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
    
    // Visualizza raggio nell'Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}