using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject bulletObj;
    public GameObject ghostBulletObj;
    public GameObject playerObj;

    private AnalyticsManager _analyticsManager;
    private SpriteRenderer spriteRenderer;
    public LevelManager levelManager;
    private PlayerController _playerController;
    private int destroyTime = 5;

    // Queue of active ghost players. Used to keep track of the ghost players.
    public Queue<GameObject> ghostPlayers = new Queue<GameObject>();

    // Tracks if the current bullet shot is the last bullet among remaining shots.
    private bool isLastBullet;

    // Tracks the previous bullet positions when the bullet disappears.
    public Queue<Vector3> prevBulletPositions = new Queue<Vector3>();

    // Specifies player or ghost player is in focus
    private bool _isPlayer = true;

    // Store the colliders of all the glass shelves
    private List<Collider2D> _glassShelfColliders = new List<Collider2D>();

    // Store the colliders of all the ghost balls
    private List<Collider2D> _ghostBallColliders = new List<Collider2D>();

    // Store the colliders of all the regular balls
    private List<Collider2D> _ballColliders = new List<Collider2D>();

    private float bulletSpeed;
    public float minBulletSpeed = 5f;
    public float maxBulletSpeed = 40f;
    private float chargeRate = 50f;
    private float maxCharge = 100f;
    private float minCharge = 0f;
    private float currentCharge;
    private string spritePath = "Assets/Icons/aim_pointer.png";
    private int numTrajectoryPoints = 50;
    private bool showTrajectory = false;
    private bool isChargeIncreasing = true;
    private bool isFirstGhostPlayer = true;

    private bool useCurvedTrajectory;
    private SpriteRenderer parentSpriteRenderer;

    Rigidbody2D rb;
    LineRenderer lr;
    public float bounciness = 0.75f; // Represents how much energy is preserved on bounce (0-1)
    public LayerMask collisionMask; // Layer mask to detect ground or other objects to bounce off
    public int maxBounces = 3;

    public bool canPerformAction = true;

    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        levelManager = FindObjectOfType<LevelManager>();
        _playerController = FindObjectOfType<PlayerController>();
        Transform gun = this.transform.Find("Gun");
        spriteRenderer = gun.GetComponent<SpriteRenderer>();

        if (levelManager.featureFlags.projectile)
        {
            // Sprite renderer to toggle projectile shot.
            parentSpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
            parentSpriteRenderer.drawMode = SpriteDrawMode.Sliced;
        }

        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();

        Texture2D tex = CreateDashedTexture(32, 1, Color.white, 0.5f); // 32x1 pixel texture with 50% filled
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.mainTexture = tex;
        lr.material = mat;

        // Adjust tiling to make dashes appear along the length of the line
        lr.material.mainTextureScale = new Vector2(10, 1);

        // Get all the glass shelf colliders
        foreach (GameObject glassShelf in GameObject.FindGameObjectsWithTag("GlassShelf"))
        {
            _glassShelfColliders.Add(glassShelf.GetComponent<Collider2D>());
        }

        // Get all the ghost ball colliders
        foreach (GameObject ghostBall in GameObject.FindGameObjectsWithTag("GhostBall"))
        {
            _ghostBallColliders.Add(ghostBall.GetComponent<Collider2D>());
        }

        // Get all the regular ball colliders
        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
        {
            _ballColliders.Add(ball.GetComponent<Collider2D>());
        }
        // Disable the colliders between regular balls and ghost balls
        foreach (Collider2D ballCollider in _ballColliders)
        {
            foreach (Collider2D ghostBallCollider in _ghostBallColliders)
            {
                Physics2D.IgnoreCollision(ballCollider, ghostBallCollider, true);
            }
        }

    }

    void Update()
    {
        CheckMouseHover();
        if (levelManager.featureFlags.projectile){
          showTrajectory = true;

        }
        // Get mouse rotation input
        if (_isPlayer)
        {
            transform.rotation = GetRotation();
        }
        else if (ghostPlayers.Count > 0)
        {
            GameObject ghostPlayer = ghostPlayers.Peek();
            ghostPlayer.transform.rotation = transform.rotation;
        }

        if (showTrajectory && _playerController.GetPlayerMovement()) DisplayTrajectory();

        if (levelManager.bulletCount == 0)
        {
            isLastBullet = true;
        }

        // Toggle between curved shot and straight shot only if the levelManager has enabled this feature.
        // TODO: Tracking whether current player is ghost vs player is a hack...
        if (showTrajectory && Input.GetKeyDown(KeyCode.Space) && _playerController.GetPlayerMovement() && !transform.parent.Find("SmilingGhostIcon").gameObject.activeInHierarchy) {
            useCurvedTrajectory = !useCurvedTrajectory;
            if (parentSpriteRenderer!=null){
              // Speed changed for projectile shot to improve efficacy of mechanic
              // Loads sprites for user knowledge of which shot they're on
              if (parentSpriteRenderer.sprite.name == "curved-shot-sprite"){
                maxBulletSpeed = 30f;
                parentSpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/straight-shot-sprite");
              } else{
                maxBulletSpeed = 35f;
                parentSpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Curved-shot-sprite");
              }
              parentSpriteRenderer.size = new Vector2(1f, 1f);
            }
        }

        // While mouse is being held down, shot will charge
        if (Input.GetMouseButton(0) && (GameObject.Find("Bullet(Clone)") == null) && canPerformAction)
        {
            // Set Charge
            //NOTE:KP: Set Charge has been updated to update the current speed at the same time so that trajectories can be updated.
            SetCharge();
            SetSpritePathOnCharge();
            if (_isPlayer)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>(spritePath);
            }

            // When mouse is released, and there is charge
        }
        else if (Input.GetMouseButtonUp(0) && currentCharge > 0f && canPerformAction)
        {
            // Don't want players to waste shot because they didn't know they needed to charge it
            // Let bullets shoot at slightly above the destroy speed to make sure blanks aren't shot.
            bulletSpeed = maxBulletSpeed * currentCharge / maxCharge;
            bulletSpeed = Mathf.Max(bulletSpeed, 0);
            currentCharge = 0f;
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/aim_pointer");

            // Shoot bullet if it is charged enough
            if (bulletSpeed >= minBulletSpeed)
            {
                Shoot();
                lr.positionCount = 0;
                if (levelManager.featureFlags.projectile)
                {
                    // Reset curved trajectory so that player doesn't accidentally use it on ghost bullet
                    parentSpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/straight-shot-sprite");
                    useCurvedTrajectory = false;
                }
            }
        }
    }

    private void CheckMouseHover()
    {
        // Convert mouse position to world point
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);  // A zero direction means it will only check the exact point

        // For debugging: Draw a line in the Scene view from the camera to the mouse position
        // Debug.DrawLine(Camera.main.transform.position, mousePosition, Color.red, 1.0f);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Ceiling"))
            {
                // Debug.Log("Hit the ceiling!");  // Log a message when the ceiling is hit
                canPerformAction = false;
            }
            else
            {
                canPerformAction = true;
            }
        }
        else
        {
            canPerformAction = true;
        }
    }



    void SetCharge()
    {
        // Increase phase
        if (isChargeIncreasing)
        {
            currentCharge += chargeRate * Time.deltaTime;
            if (currentCharge >= maxCharge)
            {
                currentCharge = maxCharge;
                isChargeIncreasing = false;
            }
        }
        // Decrease phase
        else
        {
            currentCharge -= chargeRate * Time.deltaTime;
            if (currentCharge <= minCharge)
            {
                currentCharge = minCharge;
                isChargeIncreasing = true;
            }
        }
        currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
        bulletSpeed = maxBulletSpeed*currentCharge/maxCharge;
        bulletSpeed = Mathf.Max(bulletSpeed, 0);
    }

    void SetSpritePathOnCharge()
    {
        if (currentCharge <= 14)
        {
            spritePath = "Sprites/aim_pointer";
        }
        else if (currentCharge <= 28)
        {
            spritePath = "Sprites/aim_pointer_charge_1";
        }
        else if (currentCharge <= 42)
        {
            spritePath = "Sprites/aim_pointer_charge_2";
        }
        else if (currentCharge <= 56)
        {
            spritePath = "Sprites/aim_pointer_charge_3";
        }
        else if (currentCharge <= 70)
        {
            spritePath = "Sprites/aim_pointer_charge_4";
        }
        else if (currentCharge <= 84)
        {
            spritePath = "Sprites/aim_pointer_charge_5";
        }
        else
        {
            spritePath = "Sprites/aim_pointer_charge_6";
        }
    }

    // Get mouse rotation input
    Quaternion GetRotation()
    {
        // Rotation logic
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, rotationZ);
    }

    void CreateGhostPlayer()
    {
        // Get previous bullet disappear position
        Vector3 prevShotPosition = prevBulletPositions.Dequeue();
        // Instantiate ghost player with previous shot disappear position
        GameObject ghostPlayer = Instantiate(playerObj, prevShotPosition, Quaternion.identity);
        ghostPlayer.name = "ghostPlayer";

        // Change the color of the ghost player
        ghostPlayer.GetComponent<SpriteRenderer>().color = new Color(0, 1, 1, 0.7f);
        ghostPlayer.GetComponent<SpriteRenderer>().sortingOrder = 5;

        // Set the SmilingGhostIcon active
        GameObject ghostIcon = ghostPlayer.transform.Find("SmilingGhostIcon").gameObject;
        ghostIcon.SetActive(true);

        //  Need to remove the script from ghost player or else it will just follow the user controls.
        PlayerController playerScript = ghostPlayer.GetComponent<PlayerController>();
        Destroy(playerScript);

        // Track ghostPlayer objects
        ghostPlayers.Enqueue(ghostPlayer);
    }

    public void SaveBulletPosition(Vector3 bulletPosition)
    {
        prevBulletPositions.Enqueue(bulletPosition);
        // Create ghost player only if it is not the last bullet.
        if (!isLastBullet)
        {
            // FEATURE_FLAG_CONTROL: Core Mechanic
            if (levelManager.featureFlags.coreMechanic)
            {
                CreateGhostPlayer();
                if (isFirstGhostPlayer)
                {
                    // Hook for tutorial
                    levelManager.ShowGhostPlayerTutorialText();

                    isFirstGhostPlayer = false;
                }
            }
        }
    }

    // Get mouse shoot direction
    Vector2 GetShootDirection(Vector3 position)
    {
        Vector2 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - position);
        shootDirection.Normalize();
        return shootDirection;
    }

    void ShootBullet(bool isGhost)
    {
        GameObject go; // Player or Ghost
        GameObject bo; // Bullet or Ghost Bullet

        if (isGhost)
        {
            go = ghostPlayers.Dequeue();
            bo = ghostBulletObj;
        }
        else
        {
            go = gameObject;
            bo = bulletObj;
        }

        // Instantiate bullet and set its direction
        GameObject bullet = Instantiate(bo, go.transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // Ignore collision between ghost bullet and glass shelves
        var bulletCollider = bullet.GetComponent<CircleCollider2D>();
        if (isGhost)
        {
            foreach (Collider2D glassShelfCollider in _glassShelfColliders)
            {
                Physics2D.IgnoreCollision(bulletCollider, glassShelfCollider, true);
            }
        }
        else // Ignore collision between player bullet and ghost balls
        {
            foreach (Collider2D ghostBallCollider in _ghostBallColliders)
            {
                if (ghostBallCollider)
                {
                    Physics2D.IgnoreCollision(bulletCollider, ghostBallCollider, true);
                }
            }
        }

        // The direction from the weapon to the mouse
        Vector2 shootDirection = GetShootDirection(go.transform.position);
        bulletRb.velocity = shootDirection * bulletSpeed;

        // To create curvature, create gravity for object if toggled on
        if (useCurvedTrajectory) {
          bulletRb.gravityScale = 3;
        } else {
          bulletRb.gravityScale = 0;
        }

        var shotDetail = new ShotDetails
            { Position = transform.position, Direction = shootDirection, Velocity = shootDirection * bulletSpeed };

        if (isGhost)
        {
            bullet.name = "activeGhost";
            // layer 8: ghostBullet, activates the collision properties of the ball
            bullet.layer = 8;

            // Destroy ghost player
            Destroy(go);
        }

        // Log Shot data analytics
        ShotData shotData = new ShotData(
            isGhost,
            shotDetail.Position.x,
            shotDetail.Position.y,
            shotDetail.Position.z,
            shotDetail.Direction.x,
            shotDetail.Direction.y,
            shotDetail.Velocity.x,
            shotDetail.Velocity.y
        );
        _analyticsManager.ld.shotsTaken++;
        _analyticsManager.ld.shots[_analyticsManager.ld.shotsTaken - 1] = shotData;
        _analyticsManager.LogAnalytics();

        Destroy(bullet, destroyTime);

        // Decrement remaining shots
        levelManager.BulletCountDown();
    }

    void Shoot()
    {
        // Player shoots
        if (_isPlayer)
        {
            ShootBullet(false);
            _isPlayer = false;
            // Visual indication that its not player's turn
            playerObj.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.5f, 0.5f, 0.7f);
            _playerController.SetPlayerMovement(false);
        }

        // FEATURE_FLAG_CONTROL: Core Mechanic
        // Ghost player shoots
        if (levelManager.featureFlags.coreMechanic)
        {
            if (ghostPlayers.Count > 0)
            {
                ShootBullet(true);
                _isPlayer = true;
                // Visual indication that its player's turn
                playerObj.GetComponent<SpriteRenderer>().color = Color.white;
                _playerController.SetPlayerMovement(true);
            }
        }
        else
        {
            _isPlayer = true;
            // Visual indication that its player's turn
            playerObj.GetComponent<SpriteRenderer>().color = Color.white;
            _playerController.SetPlayerMovement(true);
        }

        useCurvedTrajectory = false;
    }

    public Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps, bool useCurvedTrajectory) {
       List<Vector2> results = new List<Vector2>();
       float timeStep = Time.fixedDeltaTime/Physics2D.velocityIterations;
       Vector2 moveStep = velocity * timeStep;
       int numBounces = 0;

       for (int i = 0; i < 1000; i++){
         moveStep = velocity * timeStep;
         if (numBounces < 2){
             float projectileRadius = 0.5f; // adjust this to your projectile's size
             RaycastHit2D hit = Physics2D.CircleCast(pos, projectileRadius, velocity, moveStep.magnitude, collisionMask);
             // if (numBounces >= maxBounces) {return results;}
             if (hit.collider != null)
             {
                 numBounces++;
                 if (numBounces == 2) break;
                 // Move directly to the collision point.
                 pos = hit.point;

                 // Reflect the velocity against the normal.
                 velocity = Vector2.Reflect(velocity, hit.normal) * bounciness;

                 // Using the remaining timeStep for the next move after reflection.
                 float remainingTime = timeStep * (1 - hit.fraction); // hit.fraction gives us the fraction of the raycast distance at which the hit occurred.
                 moveStep = velocity * remainingTime;

                 pos += moveStep;

             }
             else
             {
                 if (useCurvedTrajectory){
                   velocity += Physics2D.gravity*3*timeStep;
                 }

                 pos += moveStep;
             }
             results.Add(pos);
         }
         else {
           break;
         }
       }

       return results.ToArray();
   }




    Texture2D CreateDashedTexture(int width, int height, Color dashColor, float dashPercentage = 0.5f)
    {
        Texture2D texture = new Texture2D(width, height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inDash = (x % width) < (width * dashPercentage);
                texture.SetPixel(x, y, inDash ? dashColor : Color.clear);
            }
        }
        texture.Apply();
        return texture;
    }

    void DisplayTrajectory(){
      Vector2 startPos = (Vector2)transform.position;
      Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      Vector2 _velocity = (endPos - startPos);
      _velocity.Normalize();
      _velocity*=bulletSpeed;

      Vector2[] trajectory = Plot(rb, (Vector2)transform.position, _velocity, numTrajectoryPoints, useCurvedTrajectory);
      lr.positionCount = trajectory.Length;
      Vector3[] positions = new Vector3[trajectory.Length];
      for (int i = 0; i < positions.Length; i++) {
        positions[i] = trajectory[i];
      }
      lr.SetPositions(positions);
    }

    private class ShotDetails
    {
        public Vector3 Position;
        public Vector2 Direction;
        public Vector2 Velocity;
    }
}
