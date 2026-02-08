using UnityEngine;

public class InitialVignette : MonoBehaviour
{
    [Header("Vignette Settings")]
    public GameObject vignettePanel;
    public KeyCode closeKey = KeyCode.Space;

    [Header("Player Control")]
    public bool freezePlayer = true;
    private bool vignetteShown = false;
    public bool lockCursorAfterClose = false;

    void Start()
    {
        //Debug.Log("[InitialVignette] start");

        // Controlla se il GameManager esiste
        if (GameManager.Instance == null)
        {
            //Debug.LogError("[InitialVignette] ? GAMEMANAGER È NULL!");
            ShowVignette(); // Mostra comunque il pannello iniziale
            return;
        }

        //Debug.Log("[InitialVignette] ? GameManager trovato");

        // Controlla se la vignetta è stata vista
        bool alreadySeen = GameManager.Instance.IsTaskCompleted("VignetteShown");
        //Debug.Log($"[InitialVignette] IsTaskCompleted('VignetteShown') = {alreadySeen}");

        // Stampa tutti i task completati
        //GameManager.Instance.PrintCurrentState();

        if (alreadySeen)
        {
            //Debug.Log("[InitialVignette] ? Vignetta già vista - la salto");

            if (vignettePanel != null)
                vignettePanel.SetActive(false);


            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            gameObject.SetActive(false);
            //Debug.Log("[InitialVignette] GameObject disattivato");
            return;
        }

        //Debug.Log("[InitialVignette]  mostro la vignetta");
        ShowVignette();
    }

    //funzione per mostrare il pannello all'inizio
    void ShowVignette()
    {
        if (vignettePanel != null)
        {
            vignettePanel.SetActive(true);
            vignetteShown = true;

            //Debug.Log("[InitialVignette] Vignetta mostrata");

            if (freezePlayer)
            {
                Time.timeScale = 0f;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        //else
        //{
        //    Debug.LogWarning("[InitialVignette] ? vignettePanel non assegnato!");
        //}
    }

    //controlla se è stato premuto spazio per chiudere la schermata
    void Update()
    {
        if (vignetteShown && Input.GetKeyDown(closeKey))
        {
            CloseVignette();
        }
    }

    //
    public void CloseVignette()
    {
        //Debug.Log("[InitialVignette] close vignette è stato chiamato");

        if (vignettePanel != null && vignetteShown)
        {
            vignettePanel.SetActive(false);
            vignetteShown = false;

            //Debug.Log("[InitialVignette] Vignetta chiusa");

            // Controlla se esiste il GameManager
            if (GameManager.Instance != null)
            {
                //Debug.Log("[InitialVignette] ? GameManager trovato, salvo lo stato...");

                //registra che è stata chiusa la vignetta
                GameManager.Instance.CompleteTask("VignetteShown");
                //Debug.Log("[InitialVignette] ? Task 'VignetteShown' completato!");

                // Verifica immediatamente
                bool check = GameManager.Instance.IsTaskCompleted("VignetteShown");
                //Debug.Log($"[InitialVignette] Verifica immediata: IsTaskCompleted = {check}");

                //GameManager.Instance.PrintCurrentState();
            }
            //else
            //{
            //    Debug.LogError("[InitialVignette] ? GAMEMANAGER È NULL!");
            //}

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
            //Debug.Log("[InitialVignette] GameObject disattivato");
        }
    }
}
