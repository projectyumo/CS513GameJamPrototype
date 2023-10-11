using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update

    public int bulletCount = 5;
    public int totalPoints = 0;
    public int winTextDisplayTime = 3;
    static public int maxPocketCount = 10;
    public int[] pocketPoints = new int[maxPocketCount];

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
    
    public void AddPoints(string pocket)
    {
        totalPoints += GetPocketPoints(pocket);
        pointText.text = "Score: " + totalPoints.ToString();
    }
    
    private int GetPocketPoints(string pocket)
    {
        int index = int.Parse(pocket.Substring(6)) - 1;
        int points = 0;
        switch (index)
        {
            case 0:
                points += pocketPoints[index];
                break;
            case 1:
                points += pocketPoints[index];
                break;
            case 2:
                points += pocketPoints[index];
                break;
            case 3:
                points += pocketPoints[index];
                break;
            case 4:
                points += pocketPoints[index];
                break;
            case 5:
                points += pocketPoints[index];
                break;
            case 6:
                points += pocketPoints[index];
                break;
            case 7:
                points += pocketPoints[index];
                break;
            case 8:
                points += pocketPoints[index];
                break;
            case 9:
                points += pocketPoints[index];
                break;
            default:
                points += 1;
                break;
        }
        Debug.Log(points);
        return points;
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
