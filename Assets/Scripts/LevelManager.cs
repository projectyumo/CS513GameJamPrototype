using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update

    public int bulletCount = 5;
    public int totalPoints = 0;
    public int winTextDisplayTime = 3;

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI bulletCountText;
    public TextMeshProUGUI pointText;
    public GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameOverText.gameObject.SetActive(false);
        bulletCountText.text = "Remaining shots: " + bulletCount.ToString();
        pointText.text = "Score: " + totalPoints.ToString();
    }

    public void BulletCountDown()
    {
        bulletCount--;
        string text = "Remaining shots: ";
        if (bulletCount < 0) {
            text = "Remaining shots: 0";
        }
        else
        {
            text += bulletCount.ToString();
        }
        bulletCountText.text = text;
        if (bulletCount == -1) {
            ShowGameOverText("You Lose!");
        }
    }
    
    public void AddPoints(int points)
    {
        totalPoints += points;
        pointText.text = "Score: " + totalPoints.ToString();
    }
    
    public void ShowGameOverText(string text)
    {
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = text;
    }
    
    public void WinCase()
    {
        ShowGameOverText("You Win!");
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
}
