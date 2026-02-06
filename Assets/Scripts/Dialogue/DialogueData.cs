using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 4)] //questo attributo crea un campo di testo multilinea (minimo 2 righe, massimo 4) 
    public string text;

    public string speakerName;
    public float autoAdvanceDelay = 0f;
}

[System.Serializable]
public class Dialogue
{
    public string dialogueID; //Sistema di ID per identificare univocamente ogni dialogo
    public bool showOnlyOnce = true;//per gestire dialoghi che non devono ripetersi 
    public List<DialogueLine> lines = new List<DialogueLine>();//crea una lista di dialoghi
}
