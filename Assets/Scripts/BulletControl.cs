using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    void Start()
    {

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
        if (collision.gameObject.name.Substring(0, 4) == "Ball")
        {
            Destroy(gameObject);
        }
    }
}
