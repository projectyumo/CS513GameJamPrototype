using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public BallManager ballManager;
    
    // Start is called before the first frame update
    void Start()
    {
        ballManager = FindObjectOfType<BallManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pit"))
        {
            Debug.Log(collision.gameObject.tag);
            ballManager.HandleBallCollision(gameObject);
        }
    }
}
