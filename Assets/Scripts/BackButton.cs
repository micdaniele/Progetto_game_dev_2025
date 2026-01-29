using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public string kitchenSceneName = "Kitchen2";
    public AudioClip button_click;
    public AudioClip door_close;


    // Aggiungiamo un riferimento all'AudioSource
    public AudioSource audioSource;
    // Variabile per personalizzare il delay dall'inspector
    public float delaySecondAudio = 1.5f;

    public void OnButtonClick()
    {
        // Avvia la Coroutine per gestire la sequenza
        StartCoroutine(PlayAudioSequence());
    }

    System.Collections.IEnumerator PlayAudioSequence()
    {
        // 1. Riproduce il primo suono immediatamente
        audioSource.PlayOneShot(button_click);

        // 2. Attende il tempo stabilito
        yield return new WaitForSeconds(delaySecondAudio);

        // 3. Riproduce il secondo suono
        audioSource.PlayOneShot(door_close);

        Debug.Log("Secondo audio riprodotto dopo " + delaySecondAudio + " secondi.");
    }

    public void GoBackToKitchen()
    {
        if (button_click != null)
            AudioSource.PlayClipAtPoint(button_click, Camera.main.transform.position);
            
        Debug.Log("[BackButton] Torno alla cucina");
        SceneManager.LoadScene(kitchenSceneName);
    }
}
