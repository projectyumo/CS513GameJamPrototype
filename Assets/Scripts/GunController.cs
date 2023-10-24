using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    public GameObject bulletObj;
    public GameObject ghostBulletObj;
    public GameObject playerObj;

    private Queue<ShotDetails> _previousShots = new Queue<ShotDetails>();
    private AnalyticsManager _analyticsManager;
    private SpriteRenderer spriteRenderer;
    public LevelManager levelManager;
    private int destroyTime = 5;
    public Color ghostPlayerColor = new(0.572549f, 0.7764707f, 0.7764707f, 0.7f);
    // Queue of active ghost players. Used to keep track of the ghost players that are currently active.
    // So that they can be referenced from here when they have to be destroyed.
    public Queue<GameObject> activeGhostPlayers = new Queue<GameObject>();
    // Queue of idle bullets. Used to keep track of the bullets that are currently idle.
    public Queue<GameObject> idleGhostBullets = new Queue<GameObject>();
    private List<ShotDetails> _previousShotsList = new List<ShotDetails>();
    // Tracks if the current bullet shot is the last bullet among remaining shots.
    private bool isLastBullet = false;

    private float bulletSpeed = 0f;
    public float minBulletSpeed = 10f;
    public float maxBulletSpeed = 30f;
    private float chargeRate = 50f;
    private float maxCharge = 100f;
    private float currentCharge = 0f;
    private string spritePath = "Assets/Icons/aim_pointer.png";
    private int numTrajectoryPoints = 50;
    private bool showTrajectory = true;
    private int numGhosts = 2;
    private int activeGhosts = 0;

    Rigidbody2D rb;
    LineRenderer lr;
    public float bounciness = 0.75f; // Represents how much energy is preserved on bounce (0-1)
    public LayerMask collisionMask; // Layer mask to detect ground or other objects to bounce off
    public int maxBounces = 3;

    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        levelManager = FindObjectOfType<LevelManager>();
        Transform gun = this.transform.Find("Gun");
        spriteRenderer = gun.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();

        Texture2D tex = CreateDashedTexture(32, 1, Color.white, 0.5f); // 32x1 pixel texture with 50% filled
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.mainTexture = tex;
        lr.material = mat;

        // Adjust tiling to make dashes appear along the length of the line
        lr.material.mainTextureScale = new Vector2(10, 1);

        Debug.Log("LEVEL NAME:" + levelManager.levelName);
        if (levelManager.levelName == "Sandbox"){
            showTrajectory = true;
            numGhosts = 2;
        }

    }

    void Update()
    {
        // Rotation logic
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        if (showTrajectory) DisplayTrajectory();

        // DEBUG STATEMENT
        // if (currentCharge > 0f){ Debug.Log("Current Charge" + currentCharge); }

        if (levelManager.bulletCount == 1)
        {
            isLastBullet = true;
        }

        // While mouse is being held down, shot will charge
        if (Input.GetMouseButton(0) && (GameObject.Find("Bullet(Clone)") == null) && (GameObject.Find("activeGhost") == null) )
        {
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
            if (currentCharge <= 14){
              spritePath = "Sprites/aim_pointer";
            } else if (currentCharge <= 28){
              spritePath = "Sprites/aim_pointer_charge_1";
            } else if (currentCharge <= 42){
              spritePath = "Sprites/aim_pointer_charge_2";
            } else if (currentCharge <= 56){
              spritePath = "Sprites/aim_pointer_charge_3";
            } else if (currentCharge <= 70){
              spritePath = "Sprites/aim_pointer_charge_4";
            } else if (currentCharge <= 84){
              spritePath = "Sprites/aim_pointer_charge_5";
            } else {
              spritePath = "Sprites/aim_pointer_charge_6";
            }
            spriteRenderer.sprite = Resources.Load<Sprite>(spritePath);
        // When mouse is released, and current charge
        } else if (Input.GetMouseButtonUp(0) && currentCharge > 0f){
            _analyticsManager.ld.shotsTaken++;
            _analyticsManager.LogAnalytics();

            // Don't want players to waste shot because they didn't know they needed to charge it
            // Let bullets shoot at slightly above the destroy speed to make sure blanks aren't shot.
            bulletSpeed = maxBulletSpeed*currentCharge/maxCharge;
            bulletSpeed = Mathf.Max(bulletSpeed, 0);
            currentCharge = 0f;
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/aim_pointer");

            if (bulletSpeed >= minBulletSpeed){
              // Shoot and create Echo.
              Shoot();

              // Create echo only if it is not the last bullet.
              if (!isLastBullet)
              {
                  CreateEcho();
              }
            }

        }
    }

    void CreateEcho()
    {
            GameObject ghostBullet = Instantiate(ghostBulletObj, transform.position, Quaternion.identity);
            ghostBullet.name = "idleGhost";

            // 3: Player, changing layer=3 will give the ghostBullet the same collision properties
            // as the player. Idle ghostBullets should not collide with any balls.
            ghostBullet.layer = 3;

            // // TODO: Need to enqueue as many times as the number of idle ghost bullets instantiated.
            // idleGhostBullets.Enqueue(ghostBullet);

            var ghostPlayerName = "ghostPlayer"+activeGhosts;
            GameObject ghostPlayer = Instantiate(playerObj, transform.position, Quaternion.identity);
            ghostPlayer.name = ghostPlayerName;
            ghostPlayer.transform.Find("AimPointer").transform.Find("Gun").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);

            //  Need to remove the script from ghost player or else it will just follow the user controls.
            PlayerController playerScript = ghostPlayer.GetComponent<PlayerController>();
            GunController gunScript = ghostPlayer.transform.Find("AimPointer").GetComponent<GunController>();
            Destroy(playerScript);
            Destroy(gunScript);

            // Change the color of the ghost player
            ghostPlayer.GetComponent<SpriteRenderer>().color = ghostPlayerColor;
            ghostPlayer.GetComponent<Renderer>().sortingOrder = 5;

            // Track ghostPlayer objects
            // TODO: Need to enqueue as many times as the number of ghost players instantiated.
            // activeGhostPlayers.Enqueue(ghostPlayer);
            activeGhosts++;
            if (activeGhosts > numGhosts) {
              Destroy(GameObject.Find("ghostPlayer"+(activeGhosts-numGhosts-1)));
            }
            // // Remove any existing ghost players
            // while (activeGhostPlayers.Count >= numGhosts)
            // {
            //     GameObject existingGhostPlayer = activeGhostPlayers.Dequeue();
            //     Destroy(existingGhostPlayer);
            // }
    }

    void Shoot()
    {
        List<ShotDetails> tempShots = new List<ShotDetails>();

        int shotsToRecreate = Math.Min(numGhosts, _previousShotsList.Count);
        // Create echo only if it is not the last bullet.
        for (int i = 0; i < shotsToRecreate; i++)
        {
            var shot = _previousShotsList[i];

            GameObject ghostBullet = GameObject.Find("idleGhost"+i);
            Destroy(GameObject.Find("idleGhost"));
            if (ghostBullet)
            {
                Rigidbody2D ghostBulletRb = ghostBullet.GetComponent<Rigidbody2D>();
                ghostBulletRb.velocity = shot.Velocity;
                ghostBullet.name = "activeGhost" + (i+1).ToString();  // unique names for each ghost
                Destroy(ghostBullet, destroyTime);
                ghostBullet.layer = 8; // activates the collision properties of the ball
                // idleGhostBullets.Dequeue();
            }

            // tempShots.Add(shot);
          }


        // Instantiate bullet and set its direction
        GameObject bullet = Instantiate(bulletObj, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // The direction from the weapon to the mouse
        Vector2 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        shootDirection.Normalize();

        bulletRb.velocity = shootDirection * bulletSpeed;
        Destroy(bullet, destroyTime);

        // Save this shot
        var shotDetail = new ShotDetails { Position = transform.position, Direction = shootDirection, Velocity = shootDirection * bulletSpeed };
        _previousShotsList.Add(shotDetail);

        // Ensure the list only keeps the last 3 shots
        while (_previousShotsList.Count > numGhosts)
        {
            _previousShotsList.RemoveAt(0);
        }

        if (!isLastBullet)
        {
          for (int i = 0; i < _previousShotsList.Count; i++)
          {
              GameObject ghostBullet = Instantiate(ghostBulletObj, _previousShotsList[i].Position, Quaternion.identity);
              ghostBullet.name = "idleGhost" + i;
              // 3: Player, changing layer=3 will give the ghostBullet the same collision properties
              // as the player. Idle ghostBullets should not collide with any balls.
              ghostBullet.layer = 3;
              Debug.Log("Bullet instantiated at " + _previousShotsList[i].Position);
          }
        }
        Debug.Log("LIST COUNT: " + _previousShotsList.Count);

        ShotData shotData = new ShotData(
            shotDetail.Position.x,
            shotDetail.Position.y,
            shotDetail.Position.z,
            shotDetail.Direction.x,
            shotDetail.Direction.y,
            shotDetail.Velocity.x,
            shotDetail.Velocity.y
        );
        _analyticsManager.ld.shots[_analyticsManager.ld.shotsTaken - 1] = shotData;
        _analyticsManager.LogAnalytics();

        // Decrement remaining shots
        levelManager.BulletCountDown();
    }

    public Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps) {
         List<Vector2> results = new List<Vector2>();
        float timeStep = Time.fixedDeltaTime/Physics2D.velocityIterations;
        Vector2 moveStep = velocity * timeStep;
        int numBounces = 0;

        for (int i = 0; i < steps; i++){
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
      Vector2 _velocity = (endPos - startPos)*50f;

      Vector2[] trajectory = Plot(rb, (Vector2)transform.position, _velocity, numTrajectoryPoints);
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
        public int GhostNumber;
    }
}
