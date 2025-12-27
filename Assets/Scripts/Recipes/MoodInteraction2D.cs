using UnityEngine;

public class MoodInteraction2D : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptUI;
    public GameObject moodWindow;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    private bool playerInside = false;
    private bool moodOpened = false;

    void Start()
    {
        // FORZIAMO lo stato iniziale corretto
        if (promptUI != null) promptUI.SetActive(false);
        if (moodWindow != null) moodWindow.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (moodOpened) return;

        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            promptUI.SetActive(true); // SOLO Press E
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (moodOpened) return;

        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            promptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (!playerInside || moodOpened) return;

        if (Input.GetKeyDown(interactKey))
        {
            OpenMoodWindow();
        }
    }

    void OpenMoodWindow()
    {
        moodOpened = true;

        // Nasconde il prompt
        if (promptUI != null)
            promptUI.SetActive(false);

        // ORA mostra la MoodWindow
        if (moodWindow != null)
            moodWindow.SetActive(true);

        // Blocca il gioco
        Time.timeScale = 0f;

        // Mouse per UI
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("[MoodInteraction2D] Press E ? MoodWindow aperta");
    }
}
