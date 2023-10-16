using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update

    public int bulletCount = 5;
    public int totalPoints = 0;
    public int winTextDisplayTime = 3;
    static public int MaxPocketCount = 10;
    public int[] pocketPoints = new int[MaxPocketCount];

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI bulletCountText;
    public TextMeshProUGUI pointText;
    public GameManager gameManager;
    public GameObject tutorial;
    public string winText = "You Win!";
    public string remainingShotsText = "Remaining shots: ";
    public string scoreText = "Score: ";

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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
            ShowGameOverText("You Lose!");
        }
    }
    
    public void AddPoints(int points)
    {
        totalPoints += points;
        pointText.text = scoreText + totalPoints.ToString();
    }
    
    public int GetPocketPoints(string pocket)
    {
        int index = int.Parse(pocket.Substring(6)) - 1;
        if (index < 0 || index >= MaxPocketCount)
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
    
    public void WinCase()
    {
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
        gameManager.LoadMainMenuScene();
    }

    public void RestartCurrentLevel()
    {
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
