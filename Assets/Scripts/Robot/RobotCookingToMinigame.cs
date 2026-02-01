using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotCookingToMinigame : MonoBehaviour
{
    public Sprite[] robotSprites;        // sprite robot in ordine
    public AudioClip clickSound;         // suono al cambio
    public GameObject[] startPanels;     // pannelli iniziali

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private int currentIndex = 0;
    private bool hasStarted = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // sprite iniziale
        spriteRenderer.sprite = robotSprites[0];

        // pannelli visibili all'inizio
        foreach (GameObject panel in startPanels)
        {
            if (panel != null)
                panel.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // primo spazio: chiude pannelli
            if (!hasStarted)
            {
                hasStarted = true;

                foreach (GameObject panel in startPanels)
                {
                    if (panel != null)
                        panel.SetActive(false);
                }

                return; // IMPORTANTISSIMO
            }

            // spazi successivi: cambia sprite
            CambiaSprite();
        }
    }

    void CambiaSprite()
    {
        currentIndex++;

        if (currentIndex < robotSprites.Length)
        {
            spriteRenderer.sprite = robotSprites[currentIndex];

            if (clickSound != null && audioSource != null)
                audioSource.PlayOneShot(clickSound);
        }
        else
        {
            SceneManager.LoadScene("FlappyFood");
        }
    }
}
