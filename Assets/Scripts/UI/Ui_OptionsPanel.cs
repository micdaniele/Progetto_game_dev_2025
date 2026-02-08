using UnityEngine;

public class Ui_OptionsPanel : MonoBehaviour
{
    //funzione per riferimento grafico del volume
    public void OnVOLChanged(float value)
    {
        Debug.Log("BGM Volume = " + value);
    }
}