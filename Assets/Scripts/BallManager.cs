using UnityEngine;
using TMPro;

public class BallManager : MonoBehaviour
{
    public int ballCount;

    public TextMeshProUGUI gameOverText;

    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        ballCount = transform.childCount;
    }
    
    public void HandleBallCollision(GameObject ball)
    {
        ballCount--;
        
        // All balls are knocked off
        if (ballCount == 0)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "You Win!";
        }
    }
}
