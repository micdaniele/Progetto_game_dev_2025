using UnityEngine;
using UnityEngine.UI;

public class GameManagerFlappyFood : MonoBehaviour
{
   
    private int score;
    
    public void GameOver()
    {
        Debug.Log("Game Over");

    }

    public void IncreaseScore()
    {
        score++;
    }
}
