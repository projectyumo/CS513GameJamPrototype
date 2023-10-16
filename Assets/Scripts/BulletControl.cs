using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;
    public int destroyTime = 7;
    public int minVelocity = 4;

    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
    }
    void Update()
    {
        Destroy(gameObject, destroyTime);
        if (this.GetComponent<Rigidbody2D>().velocity.magnitude < minVelocity && this.name!="idleGhost"){
            Destroy(gameObject);
        }
    }

    //Detect collisions between the appropriate surfaces
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISION WITH:" + collision.gameObject.name);
        //Check for a collision with Ball
        if (collision.gameObject.name.Substring(0, 4) == "Ball" || collision.gameObject.name == "Floor" || collision.gameObject.name.Substring(0, 4) == "Pock")
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
