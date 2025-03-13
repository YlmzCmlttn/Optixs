using UnityEngine;

public class RayReflection : MonoBehaviour
{
    [Header("Ray Settings")]
    public Vector2 rayOrigin = new Vector2(0, 0);
    public Vector2 rayDirection = Vector2.right;
    public float rayDistance = 100f;

    void Update()
    {
        // Cast the ray from the origin in the specified direction.
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);

        if (hit.collider != null)
        {
            // Draw the incident ray in red.
            Debug.DrawLine(rayOrigin, hit.point, Color.red);

            // Compute the reflection vector.
            Vector2 incomingDirection = rayDirection;
            Vector2 reflectDirection = Vector2.Reflect(incomingDirection, hit.normal);

            // Optionally, draw the reflected ray in green.
            Debug.DrawRay(hit.point, reflectDirection * (rayDistance - hit.distance), Color.green);

            // (Optional) Log hit info.
            Debug.Log($"Hit {hit.collider.name} at {hit.point} with normal {hit.normal}. Reflected direction: {reflectDirection}");
        }
        else
        {
            // No hit: draw the ray in blue.
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.blue);
        }
    }
}
