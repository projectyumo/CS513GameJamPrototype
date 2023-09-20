using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (this.GetComponent<Rigidbody2D>().velocity.magnitude < 4){
        //     Debug.Log("Test");
        //     Destroy(gameObject);
        // }

    }

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter2D(Collision2D collision)
    {

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.name.Substring(0, 4) == "Ball")
        {
            Destroy(gameObject);
        }
    }
}
