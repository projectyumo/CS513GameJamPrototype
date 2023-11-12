using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float _horizontalInput;
    public float speed = 10.0f;
    private readonly float _playerWallOffset = 1.5f;
    private bool _isMovementAllowed = true;

    public BoxCollider2D leftWall;
    public BoxCollider2D rightWall;
    private float _leftBound;
    private float _rightBound;

    private void Start()
    {
        _leftBound = leftWall.bounds.max.x + _playerWallOffset;
        _rightBound = rightWall.bounds.min.x - _playerWallOffset;
    }

    void Update()
    {
        if (_isMovementAllowed)
        {
            PlayerMovement();
        }
    }

    void PlayerMovement()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        Vector3 currentPosition = transform.position;
        currentPosition.x += _horizontalInput * speed * Time.deltaTime;
        currentPosition.x = Mathf.Clamp(currentPosition.x, _leftBound, _rightBound);
        transform.position = currentPosition;
    }

    public void SetPlayerMovement(bool isAllowed)
    {
        _isMovementAllowed = isAllowed;
    }

    public bool GetPlayerMovement(){
      return _isMovementAllowed;
    }
}
