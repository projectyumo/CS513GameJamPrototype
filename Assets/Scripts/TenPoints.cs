using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenPoints : MonoBehaviour
{
    // Start is called before the first frame update

    public LevelManager levelManager;
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            levelManager.addTenPoints();

        }
    }
}
