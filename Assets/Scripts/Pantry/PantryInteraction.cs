using UnityEngine;
using UnityEngine.SceneManagement;

public class PantryInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptUI;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Header("Scene")]
    public string pantrySceneName = "Pantry";

    [Header("Audio")]
    public AudioSource audioSource; // Trascina qui l'AudioSource
    public AudioClip openSound;    // Il suono della dispensa

    private bool playerInside = false;

    void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
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
            // --- GESTIONE AUDIO ---
            if (audioSource != null && openSound != null)
            {
                // Riproduce il suono
                audioSource.PlayOneShot(openSound);

                // TRUCCO: Sposta l'AudioSource fuori dalla gerarchia 
                // così non viene distrutto quando cambia la scena
                DontDestroyOnLoad(audioSource.gameObject);
            }
            // ----------------------

            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                GameManager.Instance.SavePlayerPosition(player.transform.position);
            }

            SceneManager.LoadScene(pantrySceneName);
        }
    }
}