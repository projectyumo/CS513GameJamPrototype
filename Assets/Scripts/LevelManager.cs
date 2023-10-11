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
        bulletCountText.text = "Remaining shot: " + bulletCount.ToString();
        pointText.text = "Score: " + totalPoints.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (bulletCount == -1)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "Game Over!";
        }
    }

    public void BulletCountDown()
    {
        bulletCount--;
        if (bulletCount!= -1) {
            bulletCountText.text = "Remaining shot: " + bulletCount.ToString();
        }

        // All balls are knocked off
    }
    
    public void AddPoints(int points)
    {
        totalPoints += points;
        pointText.text = "Score: " + totalPoints.ToString();
    }
}
