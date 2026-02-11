using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


// Trigger di dialogo basato sul completamento di task.
// Mostra dialoghi diversi in base al completamento delle task richieste.
// Può caricare una scena dopo aver completato tutte le task.

public class TaskRequirementDialogueTrigger : BaseDialogueTrigger
{
    [Header("Complete Task Dialogue")]
    public Dialogue dialogue; // Dialogo per quando le task sono completate

    [Header("Task Requirements")]
    public bool requireTaskCompletion = true;
    public string[] requiredTasks; // Lista delle task richieste

    [Header("Incomplete Task Dialogue")]
    public Dialogue tasksNotCompletedDialogue; // Dialogo per quando le task non sono completate

    [Header("Scene Loading")]
    public bool loadSceneAfterTasksCompleted = false; // Attiva il caricamento scena
    public string robotScene = "Robot"; // Nome della scena da caricare
    public float delayBeforeSceneLoad = 0.5f; // Delay prima di caricare la scena

    private bool isWaitingForDialogueEnd = false;

    protected override void Update()
    {
        // Controlla se il dialogo è finito, se lo è carica la scena
        if (isWaitingForDialogueEnd)
        {
            CheckDialogueEndAndLoadScene();
        }

        // Chiama l'Update della classe base per gestire l'input
        base.Update();
    }

    protected override bool CanTriggerDialogue()
    {
        // Se il dialogo è già stato mostrato e showOnlyOnce è attivo, non permettere di mostrarlo di nuovo
        if (dialogueTriggered && dialogue != null && dialogue.showOnlyOnce)
            return false;

        return true;
    }

    protected override void TriggerDialogue()
    {
        // Controlla il completamento delle task
        bool allTasksCompleted = AreAllTasksCompleted();

        if (allTasksCompleted)
        {
            // Se tutte le task sono state completate mostra il dialogo principale
            if (dialogue != null)
            {
                ShowDialogue(dialogue);

                // Controlla quando il dialogo finisce e se bisogna caricare una scena
                if (loadSceneAfterTasksCompleted && !string.IsNullOrEmpty(robotScene))
                {
                    isWaitingForDialogueEnd = true;
                }

                // Segna come completato se showOnlyOnce è attivo
                MarkDialogueAsCompleted(dialogue);
            }
        }
        else
        {
            // Se le task non sono completate mostra dialogo alternativo
            if (tasksNotCompletedDialogue != null)
            {
                ShowDialogue(tasksNotCompletedDialogue);
            }
        }
    }


    // Controlla se tutte le task richieste sono state completate.
    private bool AreAllTasksCompleted()
    {
        if (!requireTaskCompletion)
            return true;

        if (GameManager.Instance == null)
        {
            //Debug.LogWarning("[TaskRequirementDialogueTrigger] GameManager non trovato!");
            return false;
        }

        if (requiredTasks == null || requiredTasks.Length == 0)
        {
            //Debug.LogWarning("[TaskRequirementDialogueTrigger] Nessuna task richiesta impostata!");
            return false;
        }

        foreach (string taskName in requiredTasks)
        {
            if (!GameManager.Instance.IsTaskCompleted(taskName))
            {
                return false;
            }
        }

        return true;
    }


    // Controlla se il dialogo è finito e carica la scena.
    private void CheckDialogueEndAndLoadScene()
    {
        if (DialogueManager.Instance != null)
        {
            // Usa una coroutine con un delay fisso
            StartCoroutine(LoadSceneAfterDelay());
            isWaitingForDialogueEnd = false;
        }
    }


    // Carica la scena dopo un delay.
    private IEnumerator LoadSceneAfterDelay()
    {
        // Aspetta che il dialogo finisca
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        // Salva la posizione del player
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
}