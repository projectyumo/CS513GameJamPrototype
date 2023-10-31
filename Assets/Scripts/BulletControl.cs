using System;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;
    private GunController _gunController;
    public int minVelocity = 4;
    // Track active bullets count
    public static int activeBulletCount = 0;
    private int totalBounces = 0;

    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        _gunController = FindObjectOfType<GunController>();

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
        totalBounces++;
        //Check for a collision with Ball
        if (collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pocket"))
        {
            Destroy(gameObject);
        }

        if (gameObject.name == "Bullet(Clone)" && collision.gameObject.CompareTag("GhostWall")) {
            Destroy(gameObject);
        }
        // if (totalBounces > 4){
        //     Destroy(gameObject);
        // }
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

        // Save position of the bullet when it is destroyed for the echo shot player
        if (gameObject.name == "Bullet(Clone)")
        {
            _gunController.SaveBulletPosition(gameObject.transform.position);
        }
    }
}
