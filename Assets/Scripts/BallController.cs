using UnityEngine;

public class BallController : MonoBehaviour
{
    public BallManager ballManager;
    
    // Start is called before the first frame update
    void Start()
    {
        ballManager = FindObjectOfType<BallManager>();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pocket"))
        {
            ballManager.HandleBallCollision(gameObject);
            Destroy(gameObject);
        }
    }
}