using UnityEngine;
using static GameConstants;

public class LevelData
{
    public string gameSessionId;
    public string levelSessionId;
    public int shotsTaken;
    // Score analytics
    public int[] ballsPerPocket = new int[maxPocketCount];
    public int totalScore;
    public int levelScore;
    // Balls knocked analytics
    public int ballsKnockedOff;
    public int ballsKnockedByPlayer;
    public int ballsKnockedByGhost;
    // Barrier interaction analytics
    public int barrierPlayerCollisions;
    public int barrierGhostCollisions;
    // Ghost wall analytics

    public int ghostBallPlayerCollisions;
    public int ghostBallGhostCollisions;
    // Ghost ball "collision" analytics
    public int ghostWallContact;
    // Glass shelf analytics
    public int glassShelfPassthroughs;
    public int glassShelfCollisions;

    public int bulletCollisions;
    public long timeTaken;
    public int currentLevel;
    public string levelName;
    public LevelState levelState;
    public long timeStarted;
    public long timeUpdated;
    public ShotData[] shots = new ShotData[maxShotCount];
    public int powerup;

    public int curvedShotsTaken;
    // public int curvedShotsUtilizedByPlayer;
    // public int curvedShotsUtilizedByGhost;
}

public class AnalyticsManager : MonoBehaviour
{
    private GameManager _gameManager;
    public LevelData ld;

    private const string DatabaseURL = "https://lastcallstudios-c8991-default-rtdb.firebaseio.com/";

    void Start()
    {
        ld = new LevelData();
        _gameManager = FindObjectOfType<GameManager>();
        ld.gameSessionId = _gameManager.GetGameSessionId();
        ld.levelSessionId = System.Guid.NewGuid().ToString();
        ld.levelState = LevelState.InProgress;
        ld.timeStarted = System.DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    // Method to log analytics to Firebase Realtime Database
    public void LogAnalytics()
    {
        if (ld.gameSessionId == null || ld.levelSessionId == null)
        {
            Debug.LogWarning("Game session ID or level session ID is null");
            return;
        }
        ld.timeUpdated = System.DateTimeOffset.Now.ToUnixTimeSeconds();
        ld.timeTaken = ld.timeUpdated - ld.timeStarted;
        string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(ld);
        string url = $"{DatabaseURL}analytics_v6/{ld.levelSessionId}.json";
        StartCoroutine(_gameManager.PushDataCoroutine(url, jsonPayload));

        //Debug.Log(jsonPayload);
    }
}
