using System.Collections;
using UnityEngine;

public class StarController: MonoBehaviour
{
    private Vector3 targetPosition = new Vector3(16.65f, 17.35f, -1f);
    public float speed = 20f;
    public float hoverHeight = 1f;

    void Start()
    {
        StartCoroutine(MoveSequence());
    }

    IEnumerator MoveSequence()
    {
        // Step 1: Hover up
        Vector3 hoverPosition = transform.position + new Vector3(0f, hoverHeight, 0f);
        while (transform.position != hoverPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, hoverPosition, 10 * Time.deltaTime);
            yield return null;
        }

        // Step 2: Wait for 0.2 seconds
        yield return new WaitForSeconds(0.4f);

        // Step 3: Move to target position
        while (Vector3.Distance(transform.position, targetPosition) >= 0.3f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }
}
