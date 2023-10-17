using UnityEngine;

public class PocketController : MonoBehaviour
{
    public LevelManager levelManager;
    public int points;
    public int pocketNumber;
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        // Get the points for this pocket
        points = levelManager.GetPocketPoints(gameObject.name);
        pocketNumber = levelManager.GetPocketNumber(gameObject.name);
    }
    
    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("Ball"))
    //     {
    //         levelManager.AddPoints(points, pocketNumber);
    //         Destroy(other.gameObject);
    //     }
    // }
}
