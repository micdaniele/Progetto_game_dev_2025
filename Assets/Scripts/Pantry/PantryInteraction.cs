using UnityEngine;
using UnityEngine.SceneManagement;

public class PantryInteraction : MonoBehaviour
{
    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";
    
    [Header("Scene")]
    public string pantrySceneName = "Pantry";
    
    private bool playerInside = false;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            Debug.Log("[PantryInteraction] Player entered pantry zone");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            Debug.Log("[PantryInteraction] Player left pantry zone");
        }
    }
    
    void Update()
    {
        // Se player è dentro il trigger E preme il tasto
        if (playerInside && Input.GetKeyDown(interactKey))
        {
            OpenPantry();
        }
    }

    void OpenPantry()
    {
        // Controllo di sicurezza
        if (GameManager.Instance != null && GameManager.Instance.HasValidSelection())
        {
            Debug.Log("Vado in dispensa...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(pantrySceneName);
        }
        else
        {
            Debug.Log("NON PUOI ENTRARE: Devi prima scegliere una ricetta dal pannello!");
        }
    }
}