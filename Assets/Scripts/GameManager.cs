using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager instance;

    // Unique ID for game session
    private readonly string _gameSessionId = System.Guid.NewGuid().ToString();
    public string mainMenuSceneName = "MainMenu";

    // On Awake, set up the Singleton pattern
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // If there are no more scenes to load, loop back to the first scene (main menu)
            SceneManager.LoadScene(0);
        }
    }

    // Method to load a specific scene by name
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void RestartCurrentScene()
    {
        DestroyAllItems();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public string GetGameSessionId()
    {
        return _gameSessionId;
    }

    private void DestroyAllItems()
    {
        // Find all active bullet instances in the scene
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        // Find all active bullet instances in the scene
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

        // Destroy each bullet instance
        foreach (var bullet in bullets)
        {
            Destroy(bullet);
        }

        // Destroy each ball instance
        foreach (var ball in balls)
        {
            Destroy(ball);
        }
    }

    private void DestroyAllBullets()
    {
        // Find all active bullet instances in the scene
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        // Destroy each bullet instance
        foreach (var bullet in bullets)
        {
            Destroy(bullet);
        }
    }
}
