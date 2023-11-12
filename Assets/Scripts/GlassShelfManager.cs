using UnityEngine;

public class GlassShelfManager : MonoBehaviour
{
    private AnalyticsManager _analyticsManager;

    private void Start()
    {
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("GhostBullet")) return;
        _analyticsManager.ld.glassShelfPassthroughs++;
        _analyticsManager.LogAnalytics();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Bullet")) return;
        _analyticsManager.ld.glassShelfCollisions++;
        _analyticsManager.LogAnalytics();
    }
}