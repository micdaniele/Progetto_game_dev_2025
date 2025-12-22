using UnityEngine;
using UnityEngine.SceneManagement;

public class PantryInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Tasto per interagire")]
    public KeyCode interactionKey = KeyCode.E;
    
    [Tooltip("Distanza massima per interagire")]
    public float interactionDistance = 2f;
    
    [Header("Scene Settings")]
    [Tooltip("Nome della scena della dispensa")]
    public string pantrySceneName = "Pantry";
    
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
            Debug.LogError("[PantryInteraction] Player not found! Tag your player as 'Player'");
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
            OpenPantry();
        }
    }
    
    void OpenPantry()
    {
        Debug.Log("[PantryInteraction] Opening pantry scene!");
        
        // Carica la scena della dispensa
        SceneManager.LoadScene(pantrySceneName);
    }
    
    // Visualizza raggio nell'Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
