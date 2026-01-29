using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Complete task Dialogue")]
    public Dialogue dialogue;  // Dialogo di quando le task sono completate

    [Header("Task Requirements")]
    public bool requireTaskCompletion = true;
    public string[] requiredTasks;  // Lista delle task richieste

    [Header("Incomplete task Dialogue")]
    public Dialogue tasksNotCompletedDialogue;  // Dialogo se le task non sono completate

    [Header("Scene Loading")]
    public bool loadSceneAfterTasksCompleted = false;  // Attiva il caricamento scena
    public string robotScene="Robot";  // Nome della scena da caricare
    public float delayBeforeSceneLoad = 0.5f;  // Delay prima di caricare la scena

    [Header("UI")]
    public GameObject promptUI;  

    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";


    private bool playerInside = false;
    private bool dialogueTriggered = false;
    private bool isWaitingForDialogueEnd = false;

    public enum TriggerType
    {
        OnStart,
        OnInteract,
        OnCollision,
        Manual
    }

    void Start()
    {
        // forza lo stato iniziale corretto
        if (promptUI != null)
            promptUI.SetActive(false);

    }

    void Update()
    {
        // Controlla se il dialogo è finito, se lo è carica la scena
        if (isWaitingForDialogueEnd)
        {
            CheckDialogueEndAndLoadScene();
        }

        // Se il player non è dentro o il dialogo è già partito, non fare nulla
        if (!playerInside) return;

        // Se il dialogo è già stato mostrato e showOnlyOnce è attivo, non permettere di riattivarlo
        if (dialogueTriggered && dialogue != null && dialogue.showOnlyOnce)
            return;

            if (Input.GetKeyDown(interactKey))
            {
                TriggerDialogue();
            }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Se il dialogo è già stato mostrato e showOnlyOnce è attivo, non fare nulla
        if (dialogueTriggered && dialogue != null && dialogue.showOnlyOnce)
            return;
  
            // Mostra il prompt per premere E
            playerInside = true;
            if (promptUI != null)
                promptUI.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Se il dialogo è già stato mostrato, non nascondere il prompt
        if (dialogueTriggered && dialogue != null && dialogue.showOnlyOnce)
            return;

        playerInside = false;
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    public void TriggerDialogue()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("[DialogueTrigger] DialogueManager non trovato!");
            return;
        }

        // Nascondi il prompt
        if (promptUI != null)
            promptUI.SetActive(false);

        // controllo le task
            bool allTasksCompleted = AreAllTasksCompleted();

            if (allTasksCompleted)
            {
                // Tutte le task completate mostra il dialogo principale
                if (dialogue != null)
                {
                    Debug.Log("[DialogueTrigger] Tutte le task completate, mostro dialogo principale");
                    DialogueManager.Instance.StartDialogue(dialogue);

                    // Se bisogna caricare una scena dopo, inizia a controllare quando il dialogo finisce
                    if (loadSceneAfterTasksCompleted && !string.IsNullOrEmpty(robotScene))
                    {
                        isWaitingForDialogueEnd = true;
                        Debug.Log($"[DialogueTrigger] Aspetto la fine del dialogo per caricare '{robotScene}'");
                    }
                }
            }
            else
            {
                // Task NON completate mostra dialogo alternativo
                if (tasksNotCompletedDialogue != null)
                {
                    Debug.Log("[DialogueTrigger] Task non completate, mostro dialogo alternativo");
                    DialogueManager.Instance.StartDialogue(tasksNotCompletedDialogue);
                }
                else
                {
                    Debug.LogWarning("[DialogueTrigger] tasksNotCompletedDialogue non assegnato!");
                }

                // NON segna come completato se le task non sono finite
                return;
            }

        // Segna come completato se showOnlyOnce è attivo
        if (dialogue != null && dialogue.showOnlyOnce)
        {
            dialogueTriggered = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteTask("Dialogue_" + dialogue.dialogueID);
            }

            Debug.Log($"[DialogueTrigger] Dialogo '{dialogue.dialogueID}' completato (showOnlyOnce)");
        }
    }

    // Controlla se il dialogo è finito e carica la scena
    private void CheckDialogueEndAndLoadScene()
    {
        // Controlla se il DialogueManager ha finito di mostrare il dialogo
        if (DialogueManager.Instance != null)
        {
            // NOTA: Questa parte dipende dal tuo DialogueManager
            // Potresti dover aggiungere un metodo pubblico tipo IsDialogueActive()
            // Per ora assumo che ci sia un pannello che si disattiva quando il dialogo finisce

            // Opzione 1: Se DialogueManager ha un metodo IsDialogueActive()
            // if (!DialogueManager.Instance.IsDialogueActive())

            // Opzione 2: Se DialogueManager ha un pannello pubblico
            // if (DialogueManager.Instance.dialoguePanel != null && !DialogueManager.Instance.dialoguePanel.activeSelf)

            // Opzione 3: Usa una coroutine con un delay fisso (soluzione temporanea)
            StartCoroutine(LoadSceneAfterDelay());
            isWaitingForDialogueEnd = false;
        }
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        // Aspetta che il dialogo finisca (puoi modificare questa logica)
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        // Verifica che il dialogo sia effettivamente finito
        // Se il tuo DialogueManager ha un modo per controllarlo, usalo qui

        Debug.Log($"[DialogueTrigger] Caricamento scena '{robotScene}'...");

        // Salva la posizione del player se necessario
        if (GameManager.Instance != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                GameManager.Instance.SavePlayerPosition(player.transform.position);
            }
        }

        // Carica la scena
        SceneManager.LoadScene(robotScene);
    }

    // METODO PUBBLICO: Chiamalo dal DialogueManager quando il dialogo finisce
    public void OnDialogueEnd()
    {
        if (isWaitingForDialogueEnd && loadSceneAfterTasksCompleted)
        {
            StartCoroutine(LoadSceneAfterDelay());
            isWaitingForDialogueEnd = false;
        }
    }

    // Controlla se tutte le task richieste sono state completate
    private bool AreAllTasksCompleted()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[DialogueTrigger] GameManager non trovato!");
            return false;
        }

        if (requiredTasks == null || requiredTasks.Length == 0)
        {
            Debug.LogWarning("[DialogueTrigger] Nessuna task richiesta impostata!");
            return false;
        }

        foreach (string taskName in requiredTasks)
        {
            if (!GameManager.Instance.IsTaskCompleted(taskName))
            {
                Debug.Log($"[DialogueTrigger] Task '{taskName}' non ancora completata");
                return false;
            }
        }

        Debug.Log("[DialogueTrigger] Tutte le task richieste sono completate!");
        return true;
    }

    // Metodo pubblico per debug
    public void CheckTaskStatus()
    {
        if (!requireTaskCompletion)
        {
            Debug.Log("[DialogueTrigger] Controllo task disabilitato");
            return;
        }

        Debug.Log("=== TASK STATUS ===");
        foreach (string taskName in requiredTasks)
        {
            bool completed = GameManager.Instance != null && GameManager.Instance.IsTaskCompleted(taskName);
            Debug.Log($"Task '{taskName}': {(completed ? " Completata" : " Non completata")}");
        }
        Debug.Log("==================");
    }
}