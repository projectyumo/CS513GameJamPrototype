using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float _horizontalInput;
    private float _speed = 10.0f;

    // Update is called once per frame
    void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        // Move the player side to side based on arrow key input
        transform.Translate(Vector3.right * (_horizontalInput * _speed * Time.deltaTime));
    }
}
