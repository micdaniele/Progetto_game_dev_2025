using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 4)]
    public string text;

    public string speakerName;
    public float autoAdvanceDelay = 0f;
}

[System.Serializable]
public class Dialogue
{
    public string dialogueID;
    public bool showOnlyOnce = true;
    public List<DialogueLine> lines = new List<DialogueLine>();
}
