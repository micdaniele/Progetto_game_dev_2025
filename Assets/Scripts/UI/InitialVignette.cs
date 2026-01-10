// === VERSIONE DEBUG DI InitialVignette ===
// Usa questa temporaneamente per capire cosa succede

using UnityEngine;

public class InitialVignette : MonoBehaviour
{
    [Header("Vignette Settings")]
    public GameObject vignettePanel;
    public KeyCode closeKey = KeyCode.Escape;

    [Header("Player Control")]
    public bool freezePlayer = true;
    private bool vignetteShown = false;
    public bool lockCursorAfterClose = false;

    void Start()
    {
        Debug.Log("=== [InitialVignette] START CHIAMATO ===");

        // 1. Controlla se il GameManager esiste
        if (GameManager.Instance == null)
        {
            Debug.LogError("[InitialVignette] ? GAMEMANAGER È NULL!");
            ShowVignette(); // Mostra comunque
            return;
        }

        Debug.Log("[InitialVignette] ? GameManager trovato");

        // 2. Controlla se la vignetta è stata vista
        bool alreadySeen = GameManager.Instance.IsTaskCompleted("VignetteShown");
        Debug.Log($"[InitialVignette] IsTaskCompleted('VignetteShown') = {alreadySeen}");

        // 3. Stampa tutti i task completati
        GameManager.Instance.PrintCurrentState();

        if (alreadySeen)
        {
            Debug.Log("[InitialVignette] ? Vignetta già vista - la salto");

            if (vignettePanel != null)
                vignettePanel.SetActive(false);

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            gameObject.SetActive(false);
            Debug.Log("[InitialVignette] GameObject disattivato");
            return;
        }

        Debug.Log("[InitialVignette] ? Prima volta - mostro la vignetta");
        ShowVignette();
    }

    void ShowVignette()
    {
        if (vignettePanel != null)
        {
            vignettePanel.SetActive(true);
            vignetteShown = true;

            Debug.Log("[InitialVignette] Vignetta mostrata");

            if (freezePlayer)
            {
                Time.timeScale = 0f;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.LogWarning("[InitialVignette] ? vignettePanel non assegnato!");
        }
    }

    void Update()
    {
        if (vignetteShown && Input.GetKeyDown(closeKey))
        {
            CloseVignette();
        }
    }

    public void CloseVignette()
    {
        Debug.Log("=== [InitialVignette] CLOSE VIGNETTE CHIAMATO ===");

        if (vignettePanel != null && vignetteShown)
        {
            vignettePanel.SetActive(false);
            vignetteShown = false;

            Debug.Log("[InitialVignette] Vignetta chiusa");

            // Controlla GameManager
            if (GameManager.Instance != null)
            {
                Debug.Log("[InitialVignette] ? GameManager trovato, salvo lo stato...");
                GameManager.Instance.CompleteTask("VignetteShown");
                Debug.Log("[InitialVignette] ? Task 'VignetteShown' completato!");

                // Verifica immediatamente
                bool check = GameManager.Instance.IsTaskCompleted("VignetteShown");
                Debug.Log($"[InitialVignette] Verifica immediata: IsTaskCompleted = {check}");

                GameManager.Instance.PrintCurrentState();
            }
            else
            {
                Debug.LogError("[InitialVignette] ? GAMEMANAGER È NULL!");
            }

            Time.timeScale = 1f;

            if (lockCursorAfterClose)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            gameObject.SetActive(false);
            Debug.Log("[InitialVignette] GameObject disattivato");
        }
    }
}

// === AGGIUNGI ANCHE QUESTO DEBUG AL GAMEMANAGER ===
// Nel tuo GameManager.cs, modifica il metodo Awake così:

/*

*/