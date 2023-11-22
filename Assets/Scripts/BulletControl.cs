using UnityEngine;
using UnityEngine.UI;

public class BulletControl : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;
    private GunController _gunController;
    private Rigidbody2D _rb;
    private float _startTime;
    private float _initialVelocity;
    private SpriteRenderer _spriteRenderer;
    private Image _healthCircle;
    private GameObject _healthCircleObject;

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

        // Set the bullet health loader
        SetBulletHealthLoader();
    }

    void SetBulletHealthLoader()
    {
        // Create a new UI Image element under the Canvas
        _healthCircleObject = new GameObject("HealthCircle");
        _healthCircleObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
        _healthCircle = _healthCircleObject.AddComponent<Image>();
        // Change the image type to filled
        _healthCircle.type = Image.Type.Filled;

        // Set the sprite for _healthCircle
        Sprite loaderSprite = Resources.Load<Sprite>("Sprites/circle_loader");
        _healthCircle.sprite = loaderSprite;
        _healthCircle.color = Color.green;

        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if (_spriteRenderer.sprite != null && _healthCircle.sprite != null && Camera.main != null)
        {
            // Set the size of the _healthCircle to match the size of the bullet sprite
            RectTransform rectTransform = _healthCircle.GetComponent<RectTransform>();
            Vector3 bulletBounds = _spriteRenderer.sprite.bounds.size;
            Vector3 bulletScale = gameObject.transform.localScale;
            Vector3 bulletWorldSize = new Vector3(
                bulletBounds.x * bulletScale.x,
                bulletBounds.y * bulletScale.y,
                bulletBounds.z * bulletScale.z);

            // Convert the bullet size from world units to viewport units
            Vector2 bulletViewportSize = Camera.main.WorldToViewportPoint(bulletWorldSize) - Camera.main.WorldToViewportPoint(Vector2.zero);

            // Convert the viewport units to rectTransform sizeDelta units
            Vector2 canvasSize = ((RectTransform)GameObject.Find("Canvas").transform).sizeDelta;
            Vector2 bulletSizeInCanvasUnits = new Vector2(bulletViewportSize.x * canvasSize.x, bulletViewportSize.y * canvasSize.y);

            float thickness = 1.2f;
            
            // Set the sizeDelta of the health circle
            rectTransform.sizeDelta = bulletSizeInCanvasUnits * thickness;

            // Initially hide the health circle
            _healthCircleObject.SetActive(false);
        }
    }

    void Update()
    {
        SetBulletLife();

        if (_rb.velocity.magnitude < minVelocity && this.name != "idleGhost" && _rb.gravityScale == 0)
        {
            Destroy(gameObject);
        }
    }

    void SetBulletLife()
    {
        float curTime = Time.time;
        float timeElapsed = curTime - _startTime;
        float timePercentage = (timeElapsed / _gunController.destroyBulletTime) * 100;
        float speedPercentage = ((_initialVelocity - _rb.velocity.magnitude) / (_initialVelocity - minVelocity)) * 100;
        float life = Mathf.Max(timePercentage, speedPercentage);
        if (_rb.gravityScale != 0){
          life = timePercentage;
        }

        if (_healthCircle != null)
        {
            _healthCircle.fillAmount = (100 - life) / 100;
            if (Camera.main != null && _healthCircleObject != null)
            {
                _healthCircleObject.SetActive(true);
                Vector3 bulletScreenPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                _healthCircle.transform.position = bulletScreenPosition;
            }
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

        if (_healthCircleObject != null)
        {
            Destroy(_healthCircleObject);
        }

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
                    _gunController.TurnOffShrinkTutorial();
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
