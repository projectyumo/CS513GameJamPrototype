using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void LoadLevel(int level)
    {
        gameManager.LoadLevel(level);
    }
}