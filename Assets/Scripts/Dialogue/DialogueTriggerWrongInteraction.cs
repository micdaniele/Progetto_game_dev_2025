using UnityEngine;

public class DialogueTriggerWrongInteraction : MonoBehaviour
{
    [Header("Dialogue Setup")]
    public Dialogue dialogue;

    [Header("Trigger Settings")]
    public TriggerType triggerType = TriggerType.OnInteract; //tipo di trigger per interaggire con l'oggetto
    public KeyCode interactKey = KeyCode.E; //tasto per interaggire con l'oggetto
    public string playerTag = "Player"; //tag per chi può interagire con l'oggetto

    [Header("Recipe Check")]
    public bool requiresNoRecipeSelected = false; //il dialogo appare solo se non è stata ancora scelta una ricetta

    private bool playerNearby = false; //check per quando far apparire il promt


    //enum utili per varie interazioni
    public enum TriggerType
    {
        OnStart,
        OnInteract,
        OnCollision,
        Manual
    }

    //dialogo parte appena parte la scena
    void Start()
    {
        if (triggerType == TriggerType.OnStart)
            TriggerDialogue();
    }

    //dialogo partequando il player è nelle vicinanze ed interagisce
    void Update()
    {
        if (triggerType == TriggerType.OnInteract && playerNearby)
        {
            if (Input.GetKeyDown(interactKey))
                TriggerDialogue();
        }
    }

    //dialogo che appare se entri in un'area specifica
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (triggerType == TriggerType.OnCollision)
            TriggerDialogue();
        else
            playerNearby = true;
    }

    //dialogo che appare se esci da un'area specifica
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            playerNearby = false;
    }

    //dialogo che appare se interagisci con un oggetto
    public void TriggerDialogue()
    {
        // Controlla se è richiesto che non ci sia una ricetta selezionata
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