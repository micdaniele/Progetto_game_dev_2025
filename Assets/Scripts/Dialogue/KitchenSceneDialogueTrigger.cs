using UnityEngine;

public class KitchenSceneDialogueTrigger : MonoBehaviour
{
    [Header("Dialogues After Single Task")] //ckeck se sono stati completati i minigame
    [SerializeField] private Dialogue memoryCompletedDialogue;
    [SerializeField] private Dialogue fridgeCompletedDialogue;

    [Header("Dialogues After Both Tasks")]
    [SerializeField] private Dialogue bothTasksCompletedDialogue;

    //variabili per vedere quali dialoghi mostrare
    private bool hasShownMemoryDialogue = false;
    private bool hasShownFridgeDialogue = false;
    private bool hasShownBothTasksDialogue = false;

    void Start()
    {
        // Controlla appena parte la scena quali task sono state completate e mostra i dialoghi
        CheckAndShowDialogues();
    }


    //controlla quali dialoghi mostrare in base alle "task" completate
    void CheckAndShowDialogues()
    {
        if (GameManager.Instance == null || DialogueManager.Instance == null)
        {
            //Debug.LogWarning("[KitchenSceneDialogueTrigger] GameManager o DialogueManager non trovato!");
            return;
        }

        //controlla se i due minigame sono stati completati
        bool memoryCompleted = GameManager.Instance.IsTaskCompleted("Memory");
        bool fridgeCompleted = GameManager.Instance.IsTaskCompleted("FridgeMinigame");

        // Dialogo se entrambe le task completate
        if (memoryCompleted && fridgeCompleted && !hasShownBothTasksDialogue)
        {
            if (bothTasksCompletedDialogue != null)
            {
                //Debug.Log("[KitchenSceneDialogueTrigger] Mostrando dialogo per entrambe le task completate");
                DialogueManager.Instance.StartDialogue(bothTasksCompletedDialogue);
                hasShownBothTasksDialogue = true;
            }
        }
        // Dialogo se solo il Memory è stato completato
        else if (memoryCompleted && !fridgeCompleted && !hasShownMemoryDialogue)
        {
            if (memoryCompletedDialogue != null)
            {
                //Debug.Log("[KitchenSceneDialogueTrigger] Mostrando dialogo Memory completato (Frigo ancora da fare)");
                DialogueManager.Instance.StartDialogue(memoryCompletedDialogue);
                hasShownMemoryDialogue = true;
            }
        }
        // Dialogo se solo il minigame del frigo è stato completato
        else if (!memoryCompleted && fridgeCompleted && !hasShownFridgeDialogue)
        {
            if (fridgeCompletedDialogue != null)
            {
                //Debug.Log("[KitchenSceneDialogueTrigger] Mostrando dialogo Frigo completato (Memory ancora da fare)");
                DialogueManager.Instance.StartDialogue(fridgeCompletedDialogue);
                hasShownFridgeDialogue = true;
            }
        }
    }

    // Metodo pubblico perchè torniamo più volte nella scena
    public void RecheckDialogues()
    {
        CheckAndShowDialogues();
    }

    // Flag utili per testare
    //public void ResetDialogueFlags()
    //{
    //    hasShownMemoryDialogue = false;
    //    hasShownFridgeDialogue = false;
    //    hasShownBothTasksDialogue = false;
    //    Debug.Log("[KitchenSceneDialogueTrigger] Flag dialoghi resettati");
    //}
}