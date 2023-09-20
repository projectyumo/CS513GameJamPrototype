using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BallManager : MonoBehaviour
{
    public Queue<GameObject> ballQueue = new Queue<GameObject>();
    public float minSize = 1f;
    public float sizeReductionRate = 0.1f;

    private GameObject currentBall;
    public TextMeshProUGUI gameOverText;

    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        
        // Initialize the ball queue with all child balls and deactivate them
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject ball = transform.GetChild(i).gameObject;
            ballQueue.Enqueue(ball);
            ball.SetActive(false);
        }

        // Activate the first ball
        if (ballQueue.Count > 0)
        {
            currentBall = ballQueue.Dequeue();
            currentBall.SetActive(true);
        }
    }

    void Update()
    {
        // Reduce the size of the current ball
        if (currentBall != null)
        {
            currentBall.transform.localScale -= new Vector3(sizeReductionRate, sizeReductionRate, 0) * Time.deltaTime;

            // Check if the size has reached the minimum size
            if (currentBall.transform.localScale.x <= minSize)
            {
                Destroy(currentBall);
                // Player loses
                gameOverText.gameObject.SetActive(true);
                gameOverText.text = "Game Over!";
            }
        }
    }
    
    public void HandleBallCollision(GameObject ball)
    {
        if (ball == currentBall)
        {
            // Destroy the current ball
            Destroy(currentBall);

            // Set the next ball as the current ball
            if (ballQueue.Count > 0)
            {
                currentBall = ballQueue.Dequeue();
                currentBall.SetActive(true);
            }
            else
            {
                currentBall = null;
                // All balls are knocked off
                gameOverText.gameObject.SetActive(true);
                gameOverText.text = "You Win!";
            }
        }
    }
}
