using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public AudioClip openSound;

    private bool playerInside = false;
    private bool isOpening = false; // previene chiamate multiple
    private static bool soundPlaying = false; // previene suoni multipli

    void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            if (promptUI != null)
                promptUI.SetActive(true);

            Debug.Log("[PantryInteraction] Player vicino alla dispensa");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            if (promptUI != null)
                promptUI.SetActive(false);

            Debug.Log("[PantryInteraction] Player lontano dalla dispensa");
        }
    }

    void Update()
    {
        if (playerInside && !isOpening && !soundPlaying && Input.GetKeyDown(interactKey))
        {
            Debug.Log("[PantryInteraction] Apertura dispensa");
            OpenPantry();
        }
    }

    void OpenPantry()
    {
        // Controllo ricetta selezionata
        if (GameManager.Instance == null || !GameManager.Instance.HasValidSelection())
        {
            Debug.Log("[PantryInteraction] Devi prima scegliere una ricetta!");
            return;
        }

        isOpening = true;
        soundPlaying = true;

        Debug.Log("[PantryInteraction] Apertura dispensa");

        // Nascondi il prompt
        if (promptUI != null)
            promptUI.SetActive(false);

        // Salva posizione player
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null && GameManager.Instance != null)
        {
            GameManager.Instance.SavePlayerPosition(player.transform.position);
        }

        // Gestisci suono e caricamento scena
        if (openSound != null)
        {
            StartCoroutine(PlaySoundAndLoadScene());
        }
        else
        {
            LoadPantryScene();
        }
    }

    IEnumerator PlaySoundAndLoadScene()
    {
        Debug.Log("[PantryInteraction]  Riproduco suono dispensa");

        AudioSource.PlayClipAtPoint(openSound, Camera.main.transform.position, 1.0f);

        // Aspetta che il suono sia completo
        float waitTime = openSound.length;

        Debug.Log($"[PantryInteraction] Aspetto {waitTime} secondi");
        yield return new WaitForSeconds(waitTime);

        // Carica la scena
        LoadPantryScene();
    }

    void LoadPantryScene()
    {
        Debug.Log("[PantryInteraction] Caricamento scena Pantry");

        // Reset del flag prima di cambiare scena
        soundPlaying = false;

        SceneManager.LoadScene(pantrySceneName);
    }

    // reset quando viene disabilitato
    void OnDisable()
    {
        isOpening = false;
        soundPlaying = false;
    }

    // reset quando viene distrutto
    void OnDestroy()
    {
        soundPlaying = false;
    }
}
