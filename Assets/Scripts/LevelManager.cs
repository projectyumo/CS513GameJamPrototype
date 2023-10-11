using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update

    public int bulletCount = 10;
    public int totalPoints = 0;

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI bulletCountText;
    public TextMeshProUGUI pointText;

    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        bulletCountText.text = "Remaining shot: " + bulletCount.ToString();
        pointText.text = "Score: " + totalPoints.ToString();
    }

    public void BulletCountDown()
    {
        bulletCount--;
        bulletCountText.text = "Remaining shot: " + bulletCount.ToString();
        if (bulletCount == 0) {
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
}
