using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Setup")]
    public Dialogue dialogue;

    [Header("Trigger Settings")]
    public TriggerType triggerType = TriggerType.OnInteract;
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

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
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);

            if (dialogue.showOnlyOnce && GameManager.Instance != null)
                GameManager.Instance.CompleteTask("Dialogue_" + dialogue.dialogueID);
        }
    }
}
