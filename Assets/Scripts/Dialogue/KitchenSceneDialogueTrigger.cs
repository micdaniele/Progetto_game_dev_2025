using UnityEngine;


//Trigger condizionale per dialoghi basati sul completamento di task.
// Mostra dialoghi diversi all'avvio della scena in base alle combinazioni di task completate.
// Specifico per la scena Kitchen con i minigame Memory e Frigo.

public class KitchenSceneDialogueTrigger : MonoBehaviour
{
    [Header("Dialogues After Single Task")]
    [Tooltip("Dialogo mostrato quando solo il Memory è completato")]
    [SerializeField] private Dialogue memoryCompletedDialogue;

    [Tooltip("Dialogo mostrato quando solo il Frigo è completato")]
    [SerializeField] private Dialogue fridgeCompletedDialogue;

    [Header("Dialogues After Both Tasks")]
    [Tooltip("Dialogo mostrato quando entrambe le task sono completate")]
    [SerializeField] private Dialogue bothTasksCompletedDialogue;

    // Flags per tracciare quali dialoghi sono già stati mostrati
    private bool hasShownMemoryDialogue = false;
    private bool hasShownFridgeDialogue = false;
    private bool hasShownBothTasksDialogue = false;

    void Start()
    {
        // Controlla appena parte la scena quali task sono state completate e mostra i dialoghi
        CheckAndShowDialogues();
    }

    // Metodo pubblico per ri-controllare i dialoghi.
    public void RecheckDialogues()
    {
        CheckAndShowDialogues();
    }

    // Controlla quali task sono completate e mostra il dialogo appropriato.
    private void CheckAndShowDialogues()
    {
        if (GameManager.Instance == null || DialogueManager.Instance == null)
        {
            //Debug.LogWarning("[KitchenSceneDialogueTrigger] GameManager o DialogueManager non trovato!");
            return;
        }

        // Controlla se i due minigame sono stati completati
        bool memoryCompleted = GameManager.Instance.IsTaskCompleted("Memory");
        bool fridgeCompleted = GameManager.Instance.IsTaskCompleted("FridgeMinigame");

        // Dialogo se entrambe le task completate
        if (memoryCompleted && fridgeCompleted && !hasShownBothTasksDialogue)
        {
            ShowDialogue(bothTasksCompletedDialogue, ref hasShownBothTasksDialogue, "entrambe le task");
        }
        // Dialogo se solo il Memory è stato completato
        else if (memoryCompleted && !fridgeCompleted && !hasShownMemoryDialogue)
        {
            ShowDialogue(memoryCompletedDialogue, ref hasShownMemoryDialogue, "Memory");
        }
        // Dialogo se solo il minigame del Frigo è stato completato
        else if (!memoryCompleted && fridgeCompleted && !hasShownFridgeDialogue)
        {
            ShowDialogue(fridgeCompletedDialogue, ref hasShownFridgeDialogue, "Frigo");
        }
    }

 
    // metodo helper per mostrare un dialogo e aggiornare il flag corrispondente.

    private void ShowDialogue(Dialogue dialogue, ref bool shownFlag, string taskName)
    {
        if (dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
            shownFlag = true;
        }
        //else
        //{
        //    Debug.LogWarning($"[KitchenSceneDialogueTrigger] Dialogo per '{taskName}' non assegnato!");
        //}
    }


    // Resetta i flag dei dialoghi mostrati. Utile per testing o per permettere di rivedere i dialoghi.

    //public void ResetDialogueFlags()
    //{
    //    hasShownMemoryDialogue = false;
    //    hasShownFridgeDialogue = false;
    //    hasShownBothTasksDialogue = false;
    //    Debug.Log("[KitchenSceneDialogueTrigger] Flag dialoghi resettati");
    //}
}