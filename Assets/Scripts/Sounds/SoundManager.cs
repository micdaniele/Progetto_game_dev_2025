using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider; //Riferimento allo Slider UI che controlla il volume

    void Start()
    {
        //Controlla se esiste già un valore salvato per "musicVolume"
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
        {
            //Se non esiste, lo imposta a 1
            Load();
        }
    }

    public void ChangeVolume()
    {
        //Imposta il volume globale di Unity
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    private void Load()
    {
        //Recupera il volume salvato da PlayerPrefs e aggiorna lo slider
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save()
    {
        //Salva il valore corrente dello slider in PlayerPrefs
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}