using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager _instance;
    
    // Unique ID for game session
    private readonly string _gameSessionId = System.Guid.NewGuid().ToString();
    public string mainMenuSceneName = "MainMenu";
    public int totalScore;

    // On Awake, set up the Singleton pattern
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // This object will persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicate GameManagers
        }
    }

    // Method to load the next scene
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        SceneManager.LoadScene(nextSceneIndex < SceneManager.sceneCountInBuildSettings ? nextSceneIndex : 0);
    }

    // Method to load a specific scene by name
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level + 1);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    public void RestartCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    
    public string GetGameSessionId()
    {
        return _gameSessionId;
    }
    
    public IEnumerator PushDataCoroutine(string url, string jsonPayload)
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
        // else
        // {
        //     Debug.Log("Analytics logged successfully");
        //     Debug.Log(jsonPayload);
        // }
    }
}