using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FridgeInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptUI;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Header("Scene")]
    public string fridgeMinigameScene = "FridgeMinigame";
    public string fridgeScene = "Fridge";

    [Header("Audio")]
    public AudioClip openSound;

    private bool playerInside = false;
    private bool isOpening = false;

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
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInside && !isOpening && Input.GetKeyDown(interactKey))
        {
            OpenFridge();
        }
    }

    void OpenFridge()
    {
        // Controllo ricetta selezionata
        if (GameManager.Instance == null || !GameManager.Instance.HasValidSelection())
        {
            //Debug.Log("[PantryInteraction] Devi prima scegliere una ricetta!");
            return;
        }

        isOpening = true;

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
            LoadFridgeMinigameScene();
        }
    }

    IEnumerator PlaySoundAndLoadScene()
    {
        // Salva la durata del suono prima di qualsiasi operazione
        float soundDuration = openSound != null ? openSound.length : 0f;

        if (openSound != null)
        {
            AudioSource.PlayClipAtPoint(openSound, Camera.main.transform.position, 1.0f);
            yield return new WaitForSeconds(soundDuration);
        }

        // Carica la scena
        LoadFridgeMinigameScene();
    }

    void LoadFridgeMinigameScene()
    {
        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.IsTaskCompleted("FridgeMinigame"))
                SceneManager.LoadScene(fridgeMinigameScene);
            else
                SceneManager.LoadScene(fridgeScene);
        }
    }

    void OnDisable()
    {
        isOpening = false;

        // Ferma tutte le coroutine per evitare errori
        StopAllCoroutines();
    }
}