using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AnalyticsManager : MonoBehaviour
{
    // Unique ID for each game session
    private readonly string _gameSessionId = System.Guid.NewGuid().ToString();

    // Number of shots taken (total)
    public int shotsTaken = 0;

    // Number of shots taken (per level)
    public int shotsTakenLevel = 0;

    // Number of shots taken (per ball)
    public int shotsTakenBall = 0;

    // Number of balls knocked off (total)
    public int ballsKnockedOff = 0;

    // Number of bullet collisions (total)
    public int bulletCollisions = 0;

    // Time taken to complete game
    public float timeTakenGame = 0;

    private const string DatabaseURL = "https://lastcallstudios-c8991-default-rtdb.firebaseio.com/";

    // Method to log analytics to Firebase Realtime Database
    public void LogAnalytics()
    {
        string jsonPayload = JsonUtility.ToJson(this);
        string url = $"{DatabaseURL}analytics/{_gameSessionId}.json";

        StartCoroutine(PushDataCoroutine(url, jsonPayload));
    }

    IEnumerator PushDataCoroutine(string url, string jsonPayload)
    {
        // Using PUT to overwrite data at the specified location or create it if not existent
        using (var request = new UnityWebRequest(url, "PUT"))
        {
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
            }
        }
    }
}