using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;
    public int minVelocity = 4;
    // Track active bullets count
    public static int activeBulletCount = 0;

    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        
        // Increment activeBulletCount to track the number of bullets in the scene
        activeBulletCount++;
    }
    
    void Update()
    {
        if (this.GetComponent<Rigidbody2D>().velocity.magnitude < minVelocity && this.name!="idleGhost"){
            Destroy(gameObject);
        }
    }

    //Detect collisions between the appropriate surfaces
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(this.name + "COLLISION WITH:" + collision.gameObject.name);
        //Check for a collision with Ball
        if (collision.gameObject.tag == "Ball" || collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Pocket")
        {
            Destroy(gameObject);
        }

        // Update stats if bullet collides with another bullet
        if (collision.gameObject.name == "activeGhost")
        {
             _analyticsManager.ld.bulletCollisions++;
             _analyticsManager.LogAnalytics();
        }
    }
    
    // Decrement activeBulletCount when the bullet is destroyed
    void OnDestroy()
    {
        activeBulletCount--;
    }
}
