using UnityEngine;

/// Trigger di dialogo flessibile con vari tipi di attivazione. E richiede condizioni specifiche
public class DialogueTriggerWrongInteraction : BaseDialogueTrigger
{
    public enum TriggerType
    {
        OnStart,      // Dialogo parte all'avvio della scena
        OnInteract,   // Dialogo parte quando il player interagisce (preme E)
        OnCollision,  // Dialogo parte quando il player entra nell'area
        Manual        // Dialogo triggerato manualmente tramite codice
    }

    [Header("Dialogue Setup")]
    public Dialogue dialogue;

    [Header("Trigger Settings")]
    public TriggerType triggerType = TriggerType.OnInteract;

    [Header("Recipe Check")]
    public bool requiresNoRecipeSelected = false; // Il dialogo appare solo se non è stata ancora scelta una ricetta

    protected override void Start()
    {
        base.Start();

        // Dialogo parte appena parte la scena
        if (triggerType == TriggerType.OnStart)
            TriggerDialogue();
    }

    protected override void Update()
    {
        // Solo se il tipo è OnInteract usa la logica della classe base
        if (triggerType == TriggerType.OnInteract)
        {
            base.Update();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Se il tipo è OnCollision, triggera immediatamente il dialogo
        if (triggerType == TriggerType.OnCollision)
        {
            TriggerDialogue();
        }
        else if (triggerType == TriggerType.OnInteract)
        {
            // Usa la logica della classe base per OnInteract
            base.OnTriggerEnter2D(other);
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (triggerType == TriggerType.OnInteract)
        {
            base.OnTriggerExit2D(other);
        }
    }

    protected override bool CanTriggerDialogue()
    {
        // Controlla se è richiesto che non ci sia una ricetta selezionata
        if (requiresNoRecipeSelected)
        {
            if (GameManager.Instance != null && GameManager.Instance.HasValidSelection())
            {
                return false; // Non mostrare il dialogo se c'è già una ricetta selezionata
            }
        }

        // Se showOnlyOnce è attivo e il dialogo è già stato mostrato, non triggerare
        if (dialogueTriggered && dialogue != null && dialogue.showOnlyOnce)
            return false;

        return true;
    }

    protected override void TriggerDialogue()
    {
        // Controlla se può triggerare il dialogo
        if (!CanTriggerDialogue())
            return;

        if (dialogue != null)
        {
            ShowDialogue(dialogue);
            MarkDialogueAsCompleted(dialogue);
        }
    }
}