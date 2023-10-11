using UnityEngine;

public class PointsController : MonoBehaviour
{
    public LevelManager levelManager;
    public int points = 5;
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            levelManager.AddPoints(points);
        }
    }
}
