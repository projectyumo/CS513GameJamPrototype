using UnityEngine;

public class PocketController : MonoBehaviour
{
    public LevelManager levelManager;
    private int _points;
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        // Get the points for this pocket
        _points = levelManager.GetPocketPoints(gameObject.name);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            levelManager.AddPoints(_points);
        }
    }
}
