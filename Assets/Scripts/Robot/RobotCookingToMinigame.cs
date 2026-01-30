using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotCookingToMinigame : MonoBehaviour
{
    [SerializeField] private float waitTime = 5f; // deve combaciare con la durata dell'animazione

    private void Start()
    {
        StartCoroutine(GoToMinigame());
    }

    private System.Collections.IEnumerator GoToMinigame()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("FlappyFood");
    }
}
