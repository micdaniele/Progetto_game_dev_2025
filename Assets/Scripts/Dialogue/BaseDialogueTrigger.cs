using UnityEngine;

// Classe base astratta per tutti i trigger di dialogo e gestisce la logica comune: interazione con player, prompt UI, trigger 2D.

public abstract class BaseDialogueTrigger : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";
    public GameObject promptUI;

    protected bool playerInside = false;
    protected bool dialogueTriggered = false;

    protected virtual void Start()
    {
        // Forza lo stato iniziale corretto
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    protected virtual void Update()
    {
        // Se il player non è dentro il trigger, non fare nulla
        if (!playerInside) return;

        // Se non può triggerare il dialogo, esci
        if (!CanTriggerDialogue()) return;

        // Gestisci l'input per triggerare il dialogo
        if (Input.GetKeyDown(interactKey))
        {
            TriggerDialogue();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Se non può triggerare il dialogo, non mostrare il prompt
        if (!CanTriggerDialogue()) return;

        playerInside = true;
        if (promptUI != null)
            promptUI.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInside = false;
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    // Metodo astratto da implementare nelle classi derivate. Contiene la logica specifica per triggerare il dialogo.
    protected abstract void TriggerDialogue();


    // Metodo astratto da implementare nelle classi derivate.
    //Determina se il dialogo può essere triggerato in questo momento. True se il dialogo può essere triggerato</returns>
    protected abstract bool CanTriggerDialogue();


    // metodo helper per mostrare un dialogo tramite il DialogueManager.
    protected void ShowDialogue(Dialogue dialogue)
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("[BaseDialogueTrigger] DialogueManager non trovato!");
            return;
        }

        if (dialogue == null)
        {
            Debug.LogWarning("[BaseDialogueTrigger] Dialogo nullo!");
            return;
        }

        // Nascondi il prompt
        if (promptUI != null)
            promptUI.SetActive(false);

        DialogueManager.Instance.StartDialogue(dialogue);
    }

    // metodo helper per segnare un dialogo come completato nel GameManager.
    protected void MarkDialogueAsCompleted(Dialogue dialogue)
    {
        if (dialogue != null && dialogue.showOnlyOnce && GameManager.Instance != null)
        {
            GameManager.Instance.CompleteTask("Dialogue_" + dialogue.dialogueID);
            dialogueTriggered = true;
        }
    }


    // metodo helper per nascondere il prompt UI.
    protected void HidePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }
}