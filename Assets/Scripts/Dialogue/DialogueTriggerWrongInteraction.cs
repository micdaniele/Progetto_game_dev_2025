using UnityEngine;

public class DialogueTriggerWrongInteraction : MonoBehaviour
{
    [Header("Dialogue Setup")]
    public Dialogue dialogue;

    [Header("Trigger Settings")]
    public TriggerType triggerType = TriggerType.OnInteract;
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Header("Recipe Check")]
    public bool requiresNoRecipeSelected = false; //il dialogo appare SOLO se NON è stata ancora scelta una ricetta

    private bool playerNearby = false;

    public enum TriggerType
    {
        OnStart,
        OnInteract,
        OnCollision,
        Manual
    }

    void Start()
    {
        if (triggerType == TriggerType.OnStart)
            TriggerDialogue();
    }

    void Update()
    {
        if (triggerType == TriggerType.OnInteract && playerNearby)
        {
            if (Input.GetKeyDown(interactKey))
                TriggerDialogue();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (triggerType == TriggerType.OnCollision)
            TriggerDialogue();
        else
            playerNearby = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            playerNearby = false;
    }

    public void TriggerDialogue()
    {
        // Controlla se è richiesto che NON ci sia una ricetta selezionata
        if (requiresNoRecipeSelected)
        {
            if (GameManager.Instance != null && GameManager.Instance.HasValidSelection())
            {
                //Debug.Log($"[DialogueTrigger] Dialogo '{dialogue.dialogueID}' NON mostrato perché è già stata scelta una ricetta");
                return; // Non mostrare il dialogo se c'è già una ricetta selezionata
            }
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);

            if (dialogue.showOnlyOnce && GameManager.Instance != null)
                GameManager.Instance.CompleteTask("Dialogue_" + dialogue.dialogueID);
        }
    }
}