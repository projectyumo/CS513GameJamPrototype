using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using static GameConstants;

public class LevelManager : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;

    public int bulletCount = 5;
    public int totalPoints;
    public int[] pocketPoints = new int[maxPocketCount];

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI bulletCountText;
    public TextMeshProUGUI pointText;
    public GameManager gameManager;
    public GameObject tutorial;
    public string levelName;
    public int currentLevel;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        levelName = SceneManager.GetActiveScene().name;
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        _analyticsManager.ld.currentLevel = currentLevel;
        _analyticsManager.ld.levelName = levelName;
        gameOverText.gameObject.SetActive(false);
        bulletCountText.text = remainingShotsText + bulletCount.ToString();
        pointText.text = scoreText + totalPoints.ToString();
        tutorial = GameObject.FindGameObjectWithTag("Tutorial");
        if (tutorial != null)
        {
            tutorial.SetActive(false);
        }
    }

    public void BulletCountDown()
    {
        bulletCount--;
        string text = remainingShotsText;
        if (bulletCount < 0) {
            text = remainingShotsText + "0";
        }
        else
        {
            text += bulletCount.ToString();
        }
        bulletCountText.text = text;
        if (bulletCount < 0) {
            LoseCase();
        }
    }
    
    public void AddPoints(int points, int pocketNumber)
    {
        totalPoints += points;
        pointText.text = scoreText + totalPoints.ToString();
        _analyticsManager.ld.ballsPerPocket[pocketNumber - 1]++;
        _analyticsManager.LogAnalytics();
    }
    
    public int GetPocketNumber(string pocket)
    {
        int index = int.Parse(pocket.Substring(6)) - 1;
        if (index < 0 || index >= maxPocketCount)
        {
            return 0;
        }
        return index + 1;
    }
    
    public int GetPocketPoints(string pocket)
    {
        int index = int.Parse(pocket.Substring(6)) - 1;
        if (index < 0 || index >= maxPocketCount)
        {
            return 0;
        }
        return pocketPoints[index];
    }
    
    public void ShowGameOverText(string text)
    {
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = text;
    }

    public void LoseCase()
    {
        _analyticsManager.ld.levelState = LevelState.Failed;
        _analyticsManager.LogAnalytics();
        ShowGameOverText(loseText);
        Invoke("LoadMainMenuScene", winTextDisplayTime);
    }
    
    public void WinCase()
    {
        _analyticsManager.ld.levelState = LevelState.Completed;
        _analyticsManager.LogAnalytics();
        ShowGameOverText(winText);
        Invoke("LoadNextLevel", winTextDisplayTime);
    }

    public void LoadNextLevel()
    {
        gameOverText.gameObject.SetActive(false);
        gameManager.LoadNextScene();
    }
    
    public void LoadMainMenuScene()
    {
        gameOverText.gameObject.SetActive(false);
        gameManager.LoadMainMenuScene();
    }

    public void RestartCurrentLevel()
    {
        _analyticsManager.ld.levelState = LevelState.InProgress;
        _analyticsManager.LogAnalytics();
        gameManager.RestartCurrentScene();
    }

    public void ShowTutorial()
    {
        if (tutorial != null)
        {
            tutorial.SetActive(true);
        }
    }
    
    public void HideTutorial()
    {
        if (tutorial != null)
        {
            tutorial.SetActive(false);
        }
    }
}
