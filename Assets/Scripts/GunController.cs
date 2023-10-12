using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{

    public GameObject bulletObj;
    public GameObject ghostBulletObj;
    public GameObject playerObj;
    private float _bulletSpeed = 15f;
    private Queue<ShotDetails> _previousShots = new Queue<ShotDetails>();
    private AnalyticsManager _analyticsManager;
    public LevelManager levelManager;

    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    void Update()
    {
        // Rotation logic
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        if (Input.GetMouseButtonDown(0) && (GameObject.Find("Bullet(Clone)") == null) && (GameObject.Find("activeGhost") == null) )
        {
            _analyticsManager.shotsTaken++;
            _analyticsManager.LogAnalytics();
            Shoot();
            CreateEcho();
        }
    }

    void CreateEcho()
    {
            GameObject ghostBullet = Instantiate(ghostBulletObj, transform.position, Quaternion.identity);
            ghostBullet.name = "idleGhost";

            // 3: Player, changing layer=3 will give the ghostBullet the same collision properties
            // as the player. Idle ghostBullets should not collide with any balls.
            ghostBullet.layer = 3;

            GameObject existingGhostPlayer = GameObject.Find("ghostPlayer");
            if (existingGhostPlayer != null) {
                Destroy(existingGhostPlayer);
            }
            GameObject ghostPlayer = Instantiate(playerObj, transform.position, Quaternion.identity);
            ghostPlayer.name = "ghostPlayer";
            //  Need to remove the script from ghost player or else it will just follow the user controls.
            PlayerController playerScript = ghostPlayer.GetComponent<PlayerController>();
            GunController gunScript = ghostPlayer.transform.Find("AimPointer").GetComponent<GunController>();
            Destroy(playerScript);
            Destroy(gunScript);
    }

    void Shoot()
    {
        // If queue is not empty, reshoot the previous shot
        if (_previousShots.Count > 0)
        {
            var shot = _previousShots.Dequeue();

            GameObject ghostBullet = GameObject.Find("idleGhost");
            if (ghostBullet){
              Rigidbody2D ghostBulletRb = ghostBullet.GetComponent<Rigidbody2D>();
              ghostBulletRb.velocity = shot.direction * _bulletSpeed;
              ghostBullet.name = "activeGhost";

              // 8: ghostBullet, activates the collision properties of the ball
              ghostBullet.layer = 8;
            }
        }

        // Instantiate bullet and set its direction
        GameObject bullet = Instantiate(bulletObj, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // The direction from the weapon to the mouse
        Vector2 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        shootDirection.Normalize();

        bulletRb.velocity = shootDirection * _bulletSpeed;

        // Save this shot
        _previousShots.Enqueue(new ShotDetails { position = transform.position, direction = shootDirection });
        levelManager.BulletCountDown();
    }

    private class ShotDetails
    {
        public Vector3 position;
        public Vector2 direction;
    }
}
