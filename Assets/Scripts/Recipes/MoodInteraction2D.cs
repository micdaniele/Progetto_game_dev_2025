using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public class MoodInteraction2D : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptUI; //prompt "press E"
    public GameObject moodWindow; //finestra per la selezione del mood

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;//tasto per la interazione
    public string playerTag = "Player";

    //variabili per vedere se il player è dentro il trigger e se la MoodWindow è aperta
    private bool playerInside = false;
    private bool moodOpened = false;

    void Start()
    {
        // forziamo lo stato iniziale corretto
        if (promptUI != null) promptUI.SetActive(false);
        if (moodWindow != null) moodWindow.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (moodOpened) return;//Se la finestra è già aperta esce dalla funzione

        //se il player entra segna che è dentro e mostra il promt
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (moodOpened) return;

        //quando esci il prompt sparisce l’interazione non è più possibile
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            promptUI.SetActive(false);
        }
    }

    void Update()
    {
        //il player deve essere nel trigger e la finestra non deve essere già aperta
        if (!playerInside || moodOpened) return;

        if (Input.GetKeyDown(interactKey))
        {
            OpenMoodWindow();
        }
    }

    void OpenMoodWindow()
    {
        //Blocca qualsiasi interazione futura
        moodOpened = true;

        // Nasconde il prompt
        if (promptUI != null)
            promptUI.SetActive(false);

        // mostra la MoodWindow
        if (moodWindow != null)
            moodWindow.SetActive(true);

        // Blocca il gioco
        Time.timeScale = 0f;

        // attiva il mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //Debug.Log("[MoodInteraction2D] Press E ? MoodWindow aperta");
    }
}
