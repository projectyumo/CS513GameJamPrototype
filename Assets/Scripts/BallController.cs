using UnityEngine;

public class BallController : MonoBehaviour
{
    public LevelManager levelManager;
    public BallManager ballManager;
    private Rigidbody2D _rb;
    private Vector3 _scale;
    private bool _isMoving;
    private readonly float _stationaryThreshold = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        ballManager = FindObjectOfType<BallManager>();
        levelManager = FindObjectOfType<LevelManager>();
        _rb = GetComponent<Rigidbody2D>();
        _scale = transform.localScale;
        _rb.mass /= _scale.x/2;
    }

    void Update()
    {
        CheckMovement();
    }

    // Tracks count of stationary balls
    void CheckMovement()
    {
        if (!_isMoving && _rb.velocity.magnitude > _stationaryThreshold)
        {
            _isMoving = true;
            ballManager.BallStartedMoving();
        }
        else if (_isMoving && _rb.velocity.magnitude <= _stationaryThreshold)
        {
            _isMoving = false;
            ballManager.BallStoppedMoving();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pocket"))
        {
            PocketController pocket = other.gameObject.GetComponent<PocketController>();
            levelManager.AddPoints(pocket.points, pocket.pocketNumber);
            ballManager.HandleBallCollision(gameObject);
            if (_isMoving)
            {
                ballManager.BallStoppedMoving();
            }
            Destroy(gameObject);
        }
    }
}
