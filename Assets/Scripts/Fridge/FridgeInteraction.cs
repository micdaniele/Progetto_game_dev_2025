using UnityEngine;
using UnityEngine.SceneManagement;

public class FridgeInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptUI;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Header("Scene")]
    public string fridgeMinigameScene = "FridgeMinigame";

    private bool playerInside = false;

    void Start()
    {
        // FORZIAMO lo stato iniziale corretto
        if (promptUI != null) promptUI.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            Debug.Log("[PantryInteraction] Il player è entrato nella zona della dispensa");
            promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            Debug.Log("[PantryInteraction] Il player ha lasciato la zona della dispensa");
            promptUI.SetActive(false);
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

            Debug.Log("[PantryInteraction] Vado nel frigo...");
            SceneManager.LoadScene(fridgeMinigameScene);
        }
        else
        {
            Debug.Log("[PantryInteraction] NON PUOI ENTRARE: Devi prima scegliere una ricetta!");
        }
    }
}