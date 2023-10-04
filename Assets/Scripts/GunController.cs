using UnityEngine;

public class GunController : MonoBehaviour
{

    public GameObject bulletObj;
    private float _bulletSpeed = 50f;

    void Update()
    {
        // Rotation logic
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        if (Input.GetMouseButtonDown(0) && (GameObject.Find("Bullet(Clone)") == null) )
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Instantiate bullet and set its direction
        GameObject bullet = Instantiate(bulletObj, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // The direction from the weapon to the mouse
        Vector2 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

        bulletRb.velocity = shootDirection * _bulletSpeed;
    }
}
