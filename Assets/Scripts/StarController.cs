using System.Collections;
using UnityEngine;

public class StarController: MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 20f;
    public float hoverHeight = 1f;
    private bool shouldMove = false;

    void Start()
    {
        StartCoroutine(MoveSequence());
    }

    IEnumerator MoveSequence()
    {
        // Step 1: Hover up
        Vector3 hoverPosition = new Vector3(transform.position.x, transform.position.y + hoverHeight, transform.position.z);
        while (transform.position != hoverPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, hoverPosition, 10 * Time.deltaTime);
            yield return null;
        }

        // Step 2: Wait for 0.2 seconds
        yield return new WaitForSeconds(0.4f);

        // Step 3: Move to target position
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(16.65f, 17.35f, 0f), speed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }
}
