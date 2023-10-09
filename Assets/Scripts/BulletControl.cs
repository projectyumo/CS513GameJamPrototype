using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;
    
    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
    }
    void Update()
    {
        if (this.GetComponent<Rigidbody2D>().velocity.magnitude < 4){
            Destroy(gameObject);
        }
    }

    //Detect collisions between the appropriate surfaces
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Check for a collision with Ball
        if (collision.gameObject.name.Substring(0, 4) == "Ball" || collision.gameObject.name.Substring(0, 5) == "Floor")
        {
            Destroy(gameObject);
        }
        
        // TODO: Update stats if bullet collides with another bullet
        // if (collision.gameObject.CompareTag("Bullet"))
        // {
        //     _analyticsManager.bulletCollisions++;
        // }
    }
}
