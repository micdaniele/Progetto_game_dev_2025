using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text speakerNameText;
    public GameObject spacePrompt;

    [Header("Settings")]
    public float typewriterSpeed = 0.05f;
    public bool useTypewriterEffect = true;

    private Queue<DialogueLine> currentDialogueLines;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private Coroutine typingCoroutine;
    private string currentFullText = "";

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
                CompleteTyping();
            else
                DisplayNextLine();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueActive = true;
        currentDialogueLines = new Queue<DialogueLine>(dialogue.lines);

        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;

        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        if (currentDialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogueLines.Dequeue();

        if (!string.IsNullOrEmpty(line.speakerName))
        {
            speakerNameText.gameObject.SetActive(true);
            speakerNameText.text = line.speakerName + ":";
        }
        else
        {
            speakerNameText.gameObject.SetActive(false);
        }

        currentFullText = line.text;

        if (useTypewriterEffect)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(line.text, line.autoAdvanceDelay));
        }
        else
        {
            dialogueText.text = line.text;
        }

        spacePrompt.SetActive(true);
    }

    IEnumerator TypeText(string text, float autoAdvanceDelay)
    {
        isTyping = true;
        dialogueText.text = "";
        spacePrompt.SetActive(false);

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typewriterSpeed);
        }

        isTyping = false;
        spacePrompt.SetActive(true);

        if (autoAdvanceDelay > 0)
        {
            yield return new WaitForSecondsRealtime(autoAdvanceDelay);
            DisplayNextLine();
        }
    }

    void CompleteTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentFullText;
        isTyping = false;
        spacePrompt.SetActive(true);
    }

    void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
