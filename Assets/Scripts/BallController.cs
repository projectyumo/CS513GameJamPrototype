using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public LevelManager levelManager;
    public BallManager ballManager;
    private Rigidbody2D _rb;
    private Vector3 _scale;
    private bool _isMoving;
    private readonly float _stationaryThreshold = 0.1f;

    public GameObject starPrefab;


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

            // newStar.GetComponent<StarController>().MoveToTarget(starCounterPosition);

            PocketController pocket = other.gameObject.GetComponent<PocketController>();
            for (int i = 0; i < pocket.points; i++)
            {
                GameObject newStar = Instantiate(starPrefab, transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f),UnityEngine.Random.Range(-0.75f, 0.75f) ,0), Quaternion.identity);
            }

            levelManager.AddPoints(pocket.points, pocket.pocketNumber);
            ballManager.HandleBallCollision(gameObject);
            StartCoroutine(ShrinkAndDestroy(other, Mathf.Abs(_rb.velocity.x)));
            // if (_isMoving)
            // {
            //     ballManager.BallStoppedMoving();
            // }

        }
    }
    private IEnumerator ShrinkAndDestroy(Collider2D other, float horizontalMoveSpeed)
    {
        const float moveSpeed = 8f; // Adjust this value for the moving speed
        const float shrinkSpeed = 3f; // Adjust this value for the shrinking speed
        const float minHorizontalSpeed = 2f; // Adjust this value for the minimum horizontal speed
        Vector3 pocketPosition = other.gameObject.transform.position;
        Vector3 targetScale = new Vector3(0.5f, 0.5f, 1f);

        // Remove trigger from ball and freeze physics
        Destroy(GetComponent<Collider2D>());
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Use the maximum of the horizontalMoveSpeed and minHorizontalSpeed
        float horizontalSpeed = Mathf.Max(horizontalMoveSpeed, minHorizontalSpeed);

        // Complete the movement in stages: first horizontally, then to halfway, and finally shrinking while moving to the pocket
        const double TOLERANCE = 0.1f;
        while (Math.Abs(transform.position.x - pocketPosition.x) > TOLERANCE || transform.localScale.x > targetScale.x)
        {
            // Move to the horizontal position of the pocket
            if (Math.Abs(transform.position.x - pocketPosition.x) > TOLERANCE)
            {
                var position = transform.position;
                position = new Vector3(
                    Mathf.MoveTowards(position.x, pocketPosition.x, horizontalSpeed * Time.deltaTime),
                    position.y,
                    -1f);
                transform.position = position;
            }
            // Once horizontal position is reached, move to halfway and shrink
            else
            {
                if (Vector3.Distance(transform.position, pocketPosition) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, pocketPosition, moveSpeed * Time.deltaTime);
                }

                if (transform.localScale.x > targetScale.x)
                {
                    transform.localScale -= new Vector3(1, 1, 1) * (shrinkSpeed * Time.deltaTime);
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }




}
