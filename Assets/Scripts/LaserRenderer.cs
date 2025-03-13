using UnityEngine;

public class LaserRenderer : MonoBehaviour
{
    [Header("Ray Settings")]
    public Vector2 rayOrigin = new Vector2(0, 0);
    public Vector2 rayDirection = Vector2.right;
    public float rayDistance = 100f;
    public int maxReflections = 5;

    [Header("Debug Options")]
    [Tooltip("When enabled, the LineRenderer is disabled and the ray reflections are drawn using Gizmos.")]
    public bool useGizmos = false;

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    void Update()
    {
        // If using gizmos for debug drawing, disable the line renderer.
        if (useGizmos)
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;
            return; // Skip further processing if we're drawing with gizmos.
        }
        else
        {
            if (lineRenderer != null)
                lineRenderer.enabled = true;
        }

        // Initialize the line renderer with the starting point.
        int pointsCount = 1;
        lineRenderer.positionCount = pointsCount;
        lineRenderer.SetPosition(0, rayOrigin);

        // Set the initial origin and direction.
        Vector2 origin = rayOrigin;
        Vector2 direction = rayDirection;

        // Simulate the ray reflections.
        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance);

            if (hit.collider != null)
            {
                Debug.Log("Hit Colider " + pointsCount);
                // Increase points count and set the hit point.
                pointsCount++;
                lineRenderer.positionCount = pointsCount;
                lineRenderer.SetPosition(pointsCount - 1, hit.point);

                // Compute the reflected direction.
                Vector2 reflectDir = Vector2.Reflect(direction, hit.normal);

                // Update the origin and direction for the next segment.
                float epsilon = 0.01f; // A small value
                origin = hit.point + reflectDir * epsilon;
                //origin = hit.point;
                direction = reflectDir;
            }
            else
            {
                // No hit: extend the ray to its max distance.
                pointsCount++;
                lineRenderer.positionCount = pointsCount;
                lineRenderer.SetPosition(pointsCount - 1, origin + direction * rayDistance);
                break;
            }
        }
    }

    // This method draws the reflection using Gizmos in the Scene view when useGizmos is true.
    void OnDrawGizmos()
    {
        if (!useGizmos)
            return;

        Vector2 origin = rayOrigin;
        Vector2 direction = rayDirection;

        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance);

            if (hit.collider != null)
            {
                Debug.Log("If: i:"+i);
                // Draw the incident ray in red.
                Gizmos.color = Color.red;
                Gizmos.DrawLine(origin, hit.point);

                // Compute the reflected direction.
                Vector2 reflectDir = Vector2.Reflect(direction, hit.normal);

                // Draw the reflected ray in green.
                Gizmos.color = Color.green;
                Gizmos.DrawLine(hit.point, hit.point + reflectDir * (rayDistance - hit.distance));

                // Update origin and direction for the next reflection.
                float epsilon = 0.01f; // A small value
                origin = hit.point + reflectDir * epsilon;
                direction = reflectDir;
            }
            else
            {
                Debug.Log("Else: i:"+i);
                Debug.Log(origin + " " + origin + direction * rayDistance);
                // Draw the remaining ray in blue if nothing is hit.
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(origin, origin + direction * rayDistance);
                break;
            }
        }
    }
}
