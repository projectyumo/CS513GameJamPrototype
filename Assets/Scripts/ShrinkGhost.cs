using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkGhost : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;

    // Start is called before the first frame update
    void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "GhostBullet")
        {
            if (!collision.gameObject.GetComponent<BulletControl>().isShrunk)
            {
   
                collision.gameObject.transform.localScale *= 0.5f;
                collision.gameObject.GetComponent<BulletControl>().isShrunk = true;
                _analyticsManager.ld.powerup++;
            }
        }
    }
}
