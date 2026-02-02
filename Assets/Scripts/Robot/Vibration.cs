using UnityEngine;

public class VibrationUI : MonoBehaviour
{
    public float intensity = 1f;   
    public float duration = 0.3f; 

    private RectTransform rectTransform;
    private Vector2 startPos;
    private float timer;
    private bool isVibrating;

    //Attiva la vibrazione quando si clicca il bottone
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    
    void Update()
    {
        if (!isVibrating) return;

        timer += Time.deltaTime;

        if (timer < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;//spostamento sulle x 
            float y = Random.Range(-1f, 1f) * intensity;//spostamento sulle y
            rectTransform.anchoredPosition = startPos + new Vector2(x, y);
        }
        else
        {
            StopVibration();
        }
    }

    //Metodo per iniziare la vibrazione
    public void StartVibration()
    {
        isVibrating = true;
        timer = 0f;
    }

    //Metodo per fermare la vibrazione
    public void StopVibration()
    {
        isVibrating = false;
        rectTransform.anchoredPosition = startPos;
    }
}
