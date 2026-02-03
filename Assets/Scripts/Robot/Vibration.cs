using UnityEngine;

public class VibrationUI : MonoBehaviour
{
    public float intensity = 1f;
    public float duration = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioClip clickSound; // Suono quando clicchi

    private RectTransform rectTransform;
    private Vector2 startPos;
    private float timer;
    private bool isVibrating;
    private AudioSource audioSource;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;

        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!isVibrating) return;

        timer += Time.deltaTime;

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

        // RIPRODUCE IL SUONO con la durata specificata
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