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
            Debug.Log("[PantryInteraction] Il player è entrato nella zona della dispensa");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            Debug.Log("[PantryInteraction] Il player ha lasciato la zona della dispensa");
        }
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(interactKey))
        {
            OpenPantry();
        }
    }

    void OpenPantry()
    {
        if (GameManager.Instance != null && GameManager.Instance.HasValidSelection())
        {
            // Salva la posizione prima di cambiare scena
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                GameManager.Instance.SavePlayerPosition(player.transform.position);
            }

            Debug.Log("[PantryInteraction] Vado in dispensa...");
            SceneManager.LoadScene(pantrySceneName);
        }
        else
        {
            Debug.Log("[PantryInteraction] NON PUOI ENTRARE: Devi prima scegliere una ricetta!");
        }
    }
}