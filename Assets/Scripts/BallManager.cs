using UnityEngine;
using TMPro;

public class BallManager : MonoBehaviour
{
    public int ballCount;
    private AnalyticsManager _analyticsManager;
    public TextMeshProUGUI gameOverText;

    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        gameOverText.gameObject.SetActive(false);
        ballCount = transform.childCount;
    }
    
    public void HandleBallCollision(GameObject ball)
    {
        ballCount--;

        // Update stats
        _analyticsManager.ballsKnockedOff++;
        _analyticsManager.LogAnalytics();
        
        // All balls are knocked off
        if (ballCount == 0)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "You Win!";
        }
    }
}
