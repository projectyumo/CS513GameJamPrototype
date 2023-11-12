using UnityEngine;

public class BallManager : MonoBehaviour
{
    public int ballCount;
    private AnalyticsManager _analyticsManager;
    private LevelManager _levelManager;
    public int movingBallsCount;
    
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
        _analyticsManager.ld.ballsKnockedOff++;
        _analyticsManager.LogAnalytics();
        
        // All balls are knocked off
        if (ballCount == 0)
        {
            _levelManager.WinCase();
        }
    }
    
    public void BallStartedMoving()
    {
        movingBallsCount++;
    }

    public void BallStoppedMoving()
    {
        movingBallsCount--;
    }
}
