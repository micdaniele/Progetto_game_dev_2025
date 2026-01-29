using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BackButton : MonoBehaviour
{
    [Header("Scene")]
    public string kitchenSceneName = "Kitchen2";

    [Header("Audio")]
    public AudioClip buttonClickSound;
    public AudioClip closeSound;

    [Header("Settings")]
    [Tooltip("Aspetta che finiscano i suoni prima di caricare la scena?")]
    public bool waitForSounds = true;

    [Tooltip("Delay tra click e frigo (secondi)")]
    public float delayBetweenSounds = 0.1f;

    public void GoBackToKitchen()
    {
        Debug.Log("[BackButton] Torno alla cucina");

        // Avvia la sequenza audio
        StartCoroutine(PlaySoundsAndLoadScene());
    }

    IEnumerator PlaySoundsAndLoadScene()
    {
        float totalDelay = 0f;

        // 1. Suono del click del bottone
        if (buttonClickSound != null)
        {
            AudioSource.PlayClipAtPoint(buttonClickSound, Camera.main.transform.position);
            Debug.Log("[BackButton] Click");

            if (waitForSounds)
            {
                totalDelay += buttonClickSound.length + delayBetweenSounds;
            }
            else
            {
                totalDelay += delayBetweenSounds;
            }
        }

        // Aspetta prima di riprodurre il suono 
        yield return new WaitForSeconds(totalDelay);

        // 2. Suono frigo/dispenza che si chiude
        if (closeSound != null)
        {
            AudioSource.PlayClipAtPoint(closeSound, Camera.main.transform.position);
            Debug.Log("[BackButton] Scena chiusa");

            if (waitForSounds)
            {
                // Aspetta che finisca il suono
                yield return new WaitForSeconds(closeSound.length);
            }
        }

        // 3. Carica la scena
        Debug.Log("[BackButton] Carico Kitchen2");
        SceneManager.LoadScene(kitchenSceneName);
    }
}