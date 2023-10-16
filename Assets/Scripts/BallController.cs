using UnityEngine;

public class BallController : MonoBehaviour
{
    public BallManager ballManager;
    
    // Start is called before the first frame update
    void Start()
    {
        ballManager = FindObjectOfType<BallManager>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pocket"))
        {
            ballManager.HandleBallCollision(gameObject);
            Destroy(gameObject);
        }
    }
}