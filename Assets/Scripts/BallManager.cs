using UnityEngine;

public class BallManager : MonoBehaviour
{
    public int ballCount;
    private AnalyticsManager _analyticsManager;
    private LevelManager _levelManager;
    
    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        _levelManager = FindObjectOfType<LevelManager>();
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
            _levelManager.ShowGameOverText("You Win!");
        }
    }
}
