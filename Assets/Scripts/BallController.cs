using UnityEngine;

public class BallController : MonoBehaviour
{
    public LevelManager levelManager;
    public BallManager ballManager;
    
    // Start is called before the first frame update
    void Start()
    {
        ballManager = FindObjectOfType<BallManager>();
        levelManager = FindObjectOfType<LevelManager>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pocket"))
        {
            PocketController pocket = other.gameObject.GetComponent<PocketController>();
            levelManager.AddPoints(pocket.points, pocket.pocketNumber);
            ballManager.HandleBallCollision(gameObject);
            Destroy(gameObject);
        }
    }
}