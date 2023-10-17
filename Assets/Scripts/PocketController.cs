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
}
