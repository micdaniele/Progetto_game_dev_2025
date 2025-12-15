using UnityEngine;

public class Ui_OptionsPanel : MonoBehaviour
{

    public void OnVOLChanged(float value)
    {
        Debug.Log("BGM Volume = " + value);
    }
}