using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using static GameConstants;

public class LevelData
{
    public string gameSessionId;
    public string levelSessionId;
    public int shotsTaken;
    public int[] ballsPerPocket = new int[maxPocketCount];
    // Balls knocked analytics
    public int ballsKnockedOff;
    public int ballsKnockedByPlayer;
    public int ballsKnockedByGhost;
    // Barrier interaction analytics
    public int barrierPlayerCollisions;
    public int barrierGhostCollisions;
    // Ghost wall analytics
    public int ghostWallContact;
    
    public int bulletCollisions;
    public long timeTaken;
    public int currentLevel;
    public string levelName;
    public LevelState levelState;
    public long timeStarted;
    public long timeUpdated;
    public ShotData[] shots = new ShotData[maxShotCount];
    public int powerup;
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
        ld.timeUpdated = System.DateTimeOffset.Now.ToUnixTimeSeconds();
        ld.timeTaken = ld.timeUpdated - ld.timeStarted;
        string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(ld);
        string url = $"{DatabaseURL}analytics_v5/{ld.levelSessionId}.json";
        StartCoroutine(PushDataCoroutine(url, jsonPayload));
    }

    IEnumerator PushDataCoroutine(string url, string jsonPayload)
    {
        // Using PUT to overwrite data at the specified location or create it if not existent
        using var request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("Analytics logged successfully");
            Debug.Log(jsonPayload);
        }
    }
}