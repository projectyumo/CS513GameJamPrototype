using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FivePoints : MonoBehaviour
{
    // Start is called before the first frame update

    public GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            gameManager.addFivePoints();

        }
    }
}
