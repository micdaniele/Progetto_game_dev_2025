using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

public class BackButton : MonoBehaviour
{
    [Header("Scene")]
    public string kitchenSceneName = "Kitchen2";

    [Header("Audio")]
    public AudioClip buttonClickSound;
    public AudioClip closeSound;

    [Header("Settings")]
    public bool waitForSounds = true;//Aspetta che finiscano i suoni prima di caricare la scena
    public float delayBetweenSounds = 0.1f; //Delay tra il suono del click ed del ritorno in cucina

    public void GoBackToKitchen()
    {
        //Debug.Log("[BackButton] Torno alla cucina");

        // Avvia la sequenza audio
        StartCoroutine(PlaySoundsAndLoadScene());
    }

    IEnumerator PlaySoundsAndLoadScene()
    {
        float totalDelay = 0f;

        // Suono del click del bottone
        if (buttonClickSound != null)
        {
            AudioSource.PlayClipAtPoint(buttonClickSound, Camera.main.transform.position);
            //Debug.Log("[BackButton] Click");

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

        // Suono frigo/dispenza che si chiude
        if (closeSound != null)
        {
            AudioSource.PlayClipAtPoint(closeSound, Camera.main.transform.position);

            if (waitForSounds)
            {
                // Aspetta che finisca il suono
                yield return new WaitForSeconds(closeSound.length);
            }
        }

        // Carica la scena
        //Debug.Log("[BackButton] Carico Kitchen2");
        SceneManager.LoadScene(kitchenSceneName);
    }
}