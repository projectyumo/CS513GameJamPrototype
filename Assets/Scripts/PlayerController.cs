using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private float _horizontalInput;
    public float speed = 10.0f;
    private float _playerWallOffset = 1.5f;
    private bool _isMovementAllowed = true;
    
    public BoxCollider2D leftWall;
    public BoxCollider2D rightWall;

    // Update is called once per frame
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
        
        // Calculate the new position
        float newPositionX = transform.position.x + (_horizontalInput * speed * Time.deltaTime);

        // Check if the new position exceeds the bounds of the walls
        if (newPositionX < leftWall.bounds.max.x + _playerWallOffset)
        {
            newPositionX = leftWall.bounds.max.x + _playerWallOffset;
        }
        else if (newPositionX > rightWall.bounds.min.x - _playerWallOffset)
        {
            newPositionX = rightWall.bounds.min.x - _playerWallOffset;
        }

        // Update the player's position
        transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);
    }
    
    public void SetPlayerMovement(bool isAllowed)
    {
        _isMovementAllowed = isAllowed;
    }
}
