using System;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;
    private GunController _gunController;
    private Rigidbody2D _rb;
    private float _startTime;
    private float _initialVelocity;

    public int minVelocity = 4;

    // Track active bullets count
    public static int activeBulletCount;

    public bool noDestroy;

    public bool isShrunk;

    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        _gunController = FindObjectOfType<GunController>();
        _rb = GetComponent<Rigidbody2D>();
        _startTime = Time.time;
        _initialVelocity = _rb.velocity.magnitude;

        // Increment activeBulletCount to track the number of bullets in the scene
        activeBulletCount++;
    }

    void Update()
    {
        if (gameObject.name == "Bullet(Clone)")
        {
            gameObject.GetComponent<SpriteRenderer>().color = getBulletColor();
        }
        
        if (_rb.velocity.magnitude < minVelocity && this.name != "idleGhost")
        {
            
            Destroy(gameObject);
        }
    }

    Color getBulletColor()
    {
        float curTime = Time.time;
        float timeElapsed = curTime - _startTime;
        float timePercentage = (timeElapsed / _gunController.destroyBulletTime) * 100;
        float speedPercentage = ((_initialVelocity - _rb.velocity.magnitude) / (_initialVelocity - minVelocity)) * 100;
        float colorPercentage = Mathf.Max(timePercentage, speedPercentage);
        float r = 1 - (colorPercentage / 100);
        return new Color(r, 1, 1, 1);
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
                // Destroy(gameObject);
                break;
            case "GhostBall":
                GhostBallCollisionAnalytics(thisName);
                // Destroy(gameObject);
                break;

            case "Ground":
                // Destroy(gameObject);
                break;

            case "Pocket":
                Destroy(gameObject);
                break;

            case "GhostWall":
                if (thisName == "Bullet(Clone)")
                {
                    _analyticsManager.ld.ghostWallContact++;
                    _analyticsManager.LogAnalytics();
                    Destroy(gameObject);
                }

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
            if (_gunController != null && gameObject != null)
            {
                _gunController.SaveBulletPosition(gameObject.transform.position);
            }
        }

        
        if (gameObject != null && gameObject.name == "activeGhost")
        {
            
            if (_gunController.lastGhost == null)
            {
                if(GunController.countofGhosts < 1)
                {
                    _gunController.TurnOffShrink();
                }
               
            }

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

    void GhostBallCollisionAnalytics(string thisName)
    {
        switch (thisName)
        {
            case "Bullet(Clone)":
                _analyticsManager.ld.ballsKnockedByPlayer++;
                _analyticsManager.ld.ghostBallPlayerCollisions++;
                break;
            case "activeGhost":
                _analyticsManager.ld.ballsKnockedByGhost++;
                _analyticsManager.ld.ghostBallGhostCollisions++;
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