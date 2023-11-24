using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using static GameConstants;
using System.Collections;

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
    public TextMeshProUGUI reduceGhostBulletSizeText;
    public GameObject curvedShotTutorial;
    public GameObject loseMenu;

    // public float flashDuration = 0.5f; // Duration for each flash

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

        loseMenu = GameObject.FindGameObjectWithTag("LoseMenu");

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

        if (GameObject.FindGameObjectWithTag("CurvedShotTutorial"))
        {
            curvedShotTutorial = GameObject.FindGameObjectWithTag("CurvedShotTutorial");
        }

        levelName = SceneManager.GetActiveScene().name;
        currentLevel = SceneManager.GetActiveScene().buildIndex - 1;
        _analyticsManager.ld.currentLevel = currentLevel;
        _analyticsManager.ld.levelName = levelName;
        gameOverText.gameObject.SetActive(false);
        loseMenu.SetActive(false);
        bulletCountText.text = bulletCount.ToString();
        pointText.text = gameManager.totalScore.ToString();
        levelText.text = currentLevel.ToString();

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

        if (reduceGhostBulletSizeText != null)
        {
            ShowReduceGhostBulletSizeText();
        }

        if (curvedShotTutorial != null)
        {
            ShowCurvedShotTutorial();
        }

        SetFeatureFlags();

        // Loop through all the pockets and set the points for each pocket
        for (int i = 0; i < maxPocketCount; i++)
        {
            string pocketName = "Pocket" + (i + 1);
            GameObject pocket = GameObject.Find(pocketName);
            if (pocket != null)
            {
                int points = GetPocketPoints(pocketName);
                if (points > 5)
                {
                    points = 5;
                }
                // Get the sprite N-star-icon
                Sprite starSprite = Resources.Load<Sprite>("Sprites/" + points + "-stars-icon");
                // Create a new game object with the sprite
                GameObject star = new GameObject("Star");
                star.transform.parent = pocket.transform;
                star.transform.localPosition = new Vector3(0f, 0f, -1f);
                star.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                SpriteRenderer sr = star.AddComponent<SpriteRenderer>();
                sr.sprite = starSprite;
                sr.sortingLayerName = "Pocket";
                sr.sortingOrder = 1;
            }
        }
    }

    void Update()
    {
        // Lose case
        if (!_isGameOver && bulletCount == 0 && (BulletControl.activeBulletCount == 0 || currentLevel == 0))
        {
            Invoke(nameof(LoseCase), 1);
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
        if (currentLevel is >= 10 and <= 11)
        {
            featureFlags.shrinkPowerup = true;
        }
        if (currentLevel > 11)
        {
            featureFlags.projectile = true;
            featureFlags.coreMechanic = true;
        }
    }

    public void BulletCountDown()
    {
        bulletCount--;
        var text = bulletCount < 0 ? "0" : bulletCount.ToString();

        bulletCountText.text = text;

        if (bulletCount<=2){
          StartCoroutine(FlashColor(bulletCountText, Color.red, 0.5f, 3));
        }

        if (bulletCount==4){
          StartCoroutine(FlashColor(bulletCountText, Color.yellow, 0.5f, 3));
        }
    }

    private IEnumerator FlashColor(TextMeshProUGUI text, Color flashColor, float flashDuration, int flashes)
    {
        Color originalColor = text.color; // Store the original color
        float originalFontSize = text.fontSize; // Store the original font size

        // Flash three times
        for (int i = 0; i < flashes; i++)
        {
            text.color = flashColor; // Change to flash color
            if (flashColor == Color.red) { text.fontSize *= 1.5f;}
            else { text.fontSize *= 1.25f; }
            yield return new WaitForSeconds(flashDuration); // Wait for the duration

            // Original color and font size hardcoded because if player shoots twice
            // the script might not reach this point and
            // the text might not revert
            text.color = Color.white;
            text.fontSize = 40;
            yield return new WaitForSeconds(flashDuration); // Wait for the duration
        }
    }


    public void BulletCountUp()
    {
        bulletCount++;
        string text = bulletCount.ToString();
        bulletCountText.text = text;
    }

    public void AddPoints(int points, int pocketNumber)
    {
        gameManager.totalScore += points;
        totalPoints += points;
        pointText.text = gameManager.totalScore.ToString();

        StartCoroutine(FlashColor(pointText, Color.yellow, 1.0f, 1));

        _analyticsManager.ld.ballsPerPocket[pocketNumber - 1]++;
        _analyticsManager.ld.totalScore = gameManager.totalScore;
        _analyticsManager.ld.levelScore = totalPoints;
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

    private void ShowGameOverText(string text)
    {
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = text;
    }

    private void DestroyPlayers()
    {
        // Destroy all active ghost players
        while (_gunController.ghostPlayers.Count > 0)
        {
            if (_gunController.ghostPlayers.TryDequeue(out var ghostPlayer))
            {
                Destroy(ghostPlayer);
            }
        }
        if (_playerController != null && _playerController.gameObject != null)
        {
            Destroy(_playerController.gameObject);
        }
    }

    public void LoseCase()
    {
        // If there are still balls moving, wait for them to stop
        if (_ballManager.movingBallsCount > 0 || _isGameOver)
        {
            return;
        }

        _isGameOver = true;
        _gunController.ResetShrinks();
        DestroyPlayers();
        _analyticsManager.ld.levelState = LevelState.Failed;
        _analyticsManager.LogAnalytics();
        // ShowGameOverText(loseText);
        loseMenu.SetActive(true);
        // Invoke(nameof(LoadMainMenuScene), winTextDisplayTime);
    }

    public void WinCase()
    {
        _isGameOver = true;
        _gunController.ResetShrinks();
        DestroyPlayers();
        _analyticsManager.ld.levelState = LevelState.Completed;
        _analyticsManager.LogAnalytics();
        ShowGameOverText(winText);
        Invoke(nameof(LoadNextLevel), winTextDisplayTime);
    }

    public void LoadNextLevel()
    {
        _gunController.ResetShrinks();
        gameOverText.gameObject.SetActive(false);
        if (_gunController != null && _gunController.gameObject != null)
        {

            Destroy(_gunController.gameObject);
        }
        gameManager.LoadNextScene();
    }

    public void LoadMainMenuScene()
    {
        _gunController.ResetShrinks();
        gameOverText.gameObject.SetActive(false);
        if (_gunController != null && _gunController.gameObject != null)
        {
            Destroy(_gunController.gameObject);
        }
        gameManager.LoadMainMenuScene();
    }

    public void RestartCurrentLevel()
    {
        _gunController.ResetShrinks();
        _analyticsManager.ld.levelState = LevelState.InProgress;
        _analyticsManager.LogAnalytics();
        DestroyAllBullets();
        if (_gunController != null && _gunController.gameObject != null)
        {
            Destroy(_gunController.gameObject);
        }
        gameManager.RestartCurrentScene();
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

        GameObject[] ghostBullets = GameObject.FindGameObjectsWithTag("GhostBullet");
        // Destroy each bullet instance
        foreach (var ghostBullet in ghostBullets)
        {
            Destroy(ghostBullet);
        }
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
            // Invoke("HideGhostPlayerTutorialText", 5);
        }
    }

    private void ShowCurvedShotTutorial()
    {
        if (curvedShotTutorial != null)
        {
            curvedShotTutorial.gameObject.SetActive(true);
            // Invoke("HideCurvedShotTutorial", 5);
        }
    }


    private void HideGhostPlayerTutorialText()
    {
        ghostPlayerTutorialText.gameObject.SetActive(false);
    }

    private void HideCurvedShotTutorial()
    {
        curvedShotTutorial.gameObject.SetActive(false);
    }

    private void ShowBarrierTutorialText()
    {
        if (barrierTutorialText != null)
        {
            barrierTutorialText.gameObject.SetActive(true);
            Invoke(nameof(HideBarrierTutorialText), 5);
        }
    }

    private void HideBarrierTutorialText()
    {
        barrierTutorialText.gameObject.SetActive(false);
    }

    private void ShowReduceGhostBulletSizeText()
    {
        if (ghostPlayerTutorialText != null)
        {
            ghostPlayerTutorialText.gameObject.SetActive(true);
            Invoke(nameof(HideGhostPlayerTutorialText), 5);
        }
    }

    private void HideReduceGhostBulletSizeText()
    {
        ghostPlayerTutorialText.gameObject.SetActive(false);
    }

}
