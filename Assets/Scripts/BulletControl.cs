using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;
    private GunController _gunController;
    public int minVelocity = 4;
    // Track active bullets count
    public static int activeBulletCount;

    public bool noDestroy = false;

    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        _gunController = FindObjectOfType<GunController>();

        // Increment activeBulletCount to track the number of bullets in the scene
        activeBulletCount++;
    }

    void Update()
    {
        if (this.GetComponent<Rigidbody2D>().velocity.magnitude < minVelocity && this.name!="idleGhost")
        {
            Destroy(gameObject);
        }
    }

    //Detect collisions between the appropriate surfaces
    void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.gameObject;
        var otherTag = other.tag;
        var thisName = gameObject.name;

        switch (otherTag)
        {
            case "Ball":
                CaptureBallKnockedAnalytics(thisName);
                Destroy(gameObject);
                break;
            case "GhostBall":
                CaptureBallKnockedAnalytics(thisName);
                Destroy(gameObject);
                break;
            
            case "Ground":
            case "Pocket":
                Destroy(gameObject);
                break;

            case "GhostWall":
                if (thisName == "Bullet(Clone)")
                    _analyticsManager.ld.ghostWallContact++;
                    _analyticsManager.LogAnalytics();
                    Destroy(gameObject);
                break;

            case "Barrier":
                BarrierInteractionAnalytics(thisName);
                if (thisName == "activeGhost")
                    Destroy(other);
                break;
        }

        // S_TODO: Maybe not an useful analytic now?
        // Check for collision with another bullet
        if (other.name == "activeGhost" && thisName != "Bullet(Clone)")
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
        if (gameObject != null && gameObject.name == "Bullet(Clone)")
        {
            _gunController.SaveBulletPosition(gameObject.transform.position);
        }
    }

    // This analytic will help us understand how effectively is the player using the ghost.
    // High hits by player than ghost indicates that the player is not using the ghost effectively.
    void CaptureBallKnockedAnalytics(string thisName)
    {
        switch (thisName)
        {
            case "Bullet(Clone)":
                _analyticsManager.ld.ballsKnockedByPlayer++;
                break;
            case "activeGhost":
                _analyticsManager.ld.ballsKnockedByGhost++;
                break;
        }
        
        _analyticsManager.LogAnalytics();
    }

    // This analytic will help us understand how often players are targeting or accidentally hitting the Barrier.
    // High hits by player indicates player confusion, depending on the level design.
    void BarrierInteractionAnalytics(string thisName)
    {
        switch (thisName)
        {
            case "Bullet(Clone)":
                _analyticsManager.ld.barrierPlayerCollisions++;
                break;
            case "activeGhost":
                _analyticsManager.ld.barrierGhostCollisions++;
                break;
        }
        
        _analyticsManager.LogAnalytics();
    }
}
