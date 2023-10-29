using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void LoadLevel0()
    {
        gameManager.LoadSceneByName("Level0");
    }

    public void LoadLevel1()
    {
        gameManager.LoadSceneByName("Level1");
    }

    public void LoadLevel2()
    {
        gameManager.LoadSceneByName("Level2");
    }

    public void LoadLevel3()
    {
        gameManager.LoadSceneByName("Level3");
    }

    public void LoadLevel4()
    {
        gameManager.LoadSceneByName("Level4");
    }

    public void LoadLevel5()
    {
        gameManager.LoadSceneByName("Level5");
    }
    
    public void LoadLevel6_GlassShelf()
    {
        gameManager.LoadSceneByName("Level6_GlassShelf");
    }

    public void LoadLevel7_GlassShelf()
    {
        gameManager.LoadSceneByName("Level7_GlassShelf");
    }
}
