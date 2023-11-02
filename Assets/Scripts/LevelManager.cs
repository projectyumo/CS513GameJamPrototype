using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using static GameConstants;

public class LevelManager : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;
    private PlayerController _playerController;
    private GunController _gunController;
    private BallManager _ballManager;

    // Number of allowed bullets
    public int bulletCount = 5;
    public int totalPoints;
    public int[] pocketPoints = new int[maxPocketCount];

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI bulletCountText;
    public TextMeshProUGUI pointText;
    public TextMeshProUGUI levelText;
    public GameManager gameManager;
    public GameObject tutorial;
    public TextMeshProUGUI ghostPlayerTutorialText;
    public TextMeshProUGUI barrierTutorialText;

    public string levelName;

    // currentLevel variable indicates which level you are currently playing.
    // We will be using buildIndex - 1 of SceneManager to set this variable.
    // Since, Level0 will have buildIndex of 1, we will subtract 1 from it to get currentLevel = 0.
    public int currentLevel;
    private bool _isGameOver;

    // Feature flags to indicate which mechanics are active for the current level
    public FeatureFlags featureFlags = new FeatureFlags();

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        _playerController = FindObjectOfType<PlayerController>();
        _gunController = FindObjectOfType<GunController>();
        _ballManager = FindObjectOfType<BallManager>();

        gameOverText = GameObject.FindGameObjectWithTag("GameOverText").GetComponent<TextMeshProUGUI>();
        bulletCountText = GameObject.FindGameObjectWithTag("BulletCountText").GetComponent<TextMeshProUGUI>();
        pointText = GameObject.FindGameObjectWithTag("PointText").GetComponent<TextMeshProUGUI>();
        levelText = GameObject.FindGameObjectWithTag("LevelText").GetComponent<TextMeshProUGUI>();
        if (GameObject.FindGameObjectWithTag("GhostPlayerTutorialText"))
        {
            ghostPlayerTutorialText = GameObject.FindGameObjectWithTag("GhostPlayerTutorialText").GetComponent<TextMeshProUGUI>();
        }
        if (GameObject.FindGameObjectWithTag("BarrierTutorialText"))
        {
            barrierTutorialText = GameObject.FindGameObjectWithTag("BarrierTutorialText").GetComponent<TextMeshProUGUI>();
        }
        
        levelName = SceneManager.GetActiveScene().name;
        currentLevel = SceneManager.GetActiveScene().buildIndex - 1;
        _analyticsManager.ld.currentLevel = currentLevel;
        _analyticsManager.ld.levelName = levelName;
        gameOverText.gameObject.SetActive(false);
        bulletCountText.text = remainingShotsText + bulletCount.ToString();
        pointText.text = scoreText + totalPoints.ToString();
        if (currentLevel == 0)
        {
            levelText.text = currentLevelText + "Tutorial";
        }
        else
        {
            levelText.text = currentLevelText + currentLevel.ToString();
        }

        tutorial = GameObject.FindGameObjectWithTag("Tutorial");
        if (tutorial != null && currentLevel != 0)
        {
            tutorial.SetActive(false);
        }

        if (ghostPlayerTutorialText != null)
        {
            ghostPlayerTutorialText.gameObject.SetActive(false);
        }
        if (barrierTutorialText != null)
        {
            ShowBarrierTutorialText();
        }

        SetFeatureFlags();
    }

    void Update()
    {
        // Lose case
        if (!_isGameOver && bulletCount == 0 && BulletControl.activeBulletCount == 0)
        {
            Invoke("LoseCase", 1);
        }
    }

    // Enable mechanics here pertaining to the current level
    void SetFeatureFlags()
    {
        if (currentLevel >= 1)
        {
            // Core mechanic
            featureFlags.coreMechanic = true;
        }
        if (currentLevel > 9)
        {
            featureFlags.projectile = true;
            featureFlags.coreMechanic = true;
        }
    }

    public void BulletCountDown()
    {
        bulletCount--;
        string text = remainingShotsText;
        if (bulletCount < 0)
        {
            text = remainingShotsText + "0";
        }
        else
        {
            text += bulletCount.ToString();
        }

        bulletCountText.text = text;
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

    private void DestroyPlayers()
    {
        // Destroy all active ghost players
        while (_gunController.ghostPlayers.Count > 0)
        {
            GameObject ghostPlayer;
            if (_gunController.ghostPlayers.TryDequeue(out ghostPlayer))
            {
                Destroy(ghostPlayer);
            }
        }

        Destroy(_playerController.gameObject);
    }

    public void LoseCase()
    {
        // If there are still balls moving, wait for them to stop
        if (_ballManager.movingBallsCount > 0 || _isGameOver)
        {
            return;
        }

        _isGameOver = true;
        DestroyPlayers();
        _analyticsManager.ld.levelState = LevelState.Failed;
        _analyticsManager.LogAnalytics();
        ShowGameOverText(loseText);
        Invoke("LoadMainMenuScene", winTextDisplayTime);
    }

    public void WinCase()
    {
        _isGameOver = true;
        DestroyPlayers();
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

    public void ShowGhostPlayerTutorialText()
    {
        if (ghostPlayerTutorialText != null)
        {
            ghostPlayerTutorialText.gameObject.SetActive(true);
            Invoke("HideGhostPlayerTutorialText", 5);
        }
    }

    private void HideGhostPlayerTutorialText()
    {
        ghostPlayerTutorialText.gameObject.SetActive(false);
    }
    
    private void ShowBarrierTutorialText()
    {
        if (barrierTutorialText != null)
        {
            barrierTutorialText.gameObject.SetActive(true);
            Invoke("HideBarrierTutorialText", 5);
        }
    }

    private void HideBarrierTutorialText()
    {
        barrierTutorialText.gameObject.SetActive(false);
    }
}
