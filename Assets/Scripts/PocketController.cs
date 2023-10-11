using UnityEngine;

public class PocketController : MonoBehaviour
{
    public LevelManager levelManager;
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            levelManager.AddPoints(gameObject.name);
        }
    }
}
