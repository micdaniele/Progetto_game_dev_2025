using UnityEngine;

public class VibrationUI : MonoBehaviour
{
    //variabili per l'intensità e la durata della vibrazione
    public float intensity = 0.3f;
    public float duration = 5.0f;

    [Header("Audio")]
    [SerializeField] private AudioClip clickSound; // Suono quando clicchi

    private RectTransform rectTransform;//riferimento alla UI che vibra
    private Vector2 startPos;//posizione originale, per ripristinarla
    private float timer;//per contare il tempo trascorso
    private bool isVibrating;//flag che indica se la vibrazione è attiva
    private AudioSource audioSource;//per riprodurre il suono

    void Awake()
    {
        //Salva la posizione originale
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;

        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        //Disabilita la riproduzione automatica del suono
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!isVibrating) return;//Se non vibra esce

        timer += Time.deltaTime;//Aggiorna il timer

        //Fino a duration, muove il robot casualmente nelle x e nelle y entro i limiti di intensity
        if (timer < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity; // spostamento sulle x 
            float y = Random.Range(-1f, 1f) * intensity; // spostamento sulle y
            rectTransform.anchoredPosition = startPos + new Vector2(x, y);
        }
        else
        {
            StopVibration();
        }
    }

    // Metodo per iniziare la vibrazione
    public void StartVibration()
    {
        isVibrating = true;
        timer = 0f;

        // riproduce il suono con la durata specificata
        if (clickSound != null && audioSource != null)
        {
            // Calcola il pitch per far durare il suono esattamente quanto duration
            float originalDuration = clickSound.length;
            float pitchAdjustment = originalDuration / duration;

            audioSource.pitch = pitchAdjustment;
            audioSource.PlayOneShot(clickSound);
        }
    }

    // Metodo per fermare la vibrazione
    public void StopVibration()
    {
        isVibrating = false;
        rectTransform.anchoredPosition = startPos;

        // Reset del pitch
        if (audioSource != null)
        {
            audioSource.pitch = 1f;
        }
    }
}