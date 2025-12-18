    using UnityEngine;

public class InitialVignette : MonoBehaviour
{
    [Header("Vignette Settings")]
    [Tooltip("Il pannello della vignetta")]
    public GameObject vignettePanel;
    
    [Tooltip("Tasto per chiudere la vignetta")]
    public KeyCode closeKey = KeyCode.E;


    [Header("Player Control")]
    [Tooltip("Blocca il movimento del player durante la vignetta?")]
    public bool freezePlayer = true;
    
    private bool vignetteShown = false;
    public bool lockCursorAfterClose = false;


    void Start()
    {
        // Mostra la vignetta all'inizio
        ShowVignette();
    }
    
    void ShowVignette()
    {
        if (vignettePanel != null)
        {
            vignettePanel.SetActive(true);
            vignetteShown = true;
            
            Debug.Log("[TutorialVignette] Vignette shown");
            
            // Blocca il gioco
            if (freezePlayer)
            {
                Time.timeScale = 0f;
            }
            
            // Sblocca il cursore per cliccare
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
        }
        else
        {
            Debug.LogWarning("[TutorialVignette] Vignette panel not assigned!");
        }
    }
    
    
    void Update()
    {
        if (vignetteShown && Input.GetKeyDown(closeKey))
        {
            CloseVignette();
        }
    }
    // Metodo pubblico per chiudere la vignetta
    public void CloseVignette()
    {
        if (vignettePanel != null && vignetteShown)
        {
            vignettePanel.SetActive(false);
            vignetteShown = false;
            
            Debug.Log("[TutorialVignette] Vignette closed");
            
            // Sblocca il gioco
            Time.timeScale = 1f;

            // Blocca il cursore per il gameplay       
            //Cursor.lockState = CursorLockMode.Locked;<--Non va bene perchè dopo non fa cliccare altri pulsanti
            //Cursor.visible = false;

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
        }
    }
}
