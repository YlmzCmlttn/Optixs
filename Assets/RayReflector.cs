using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayReflector : MonoBehaviour
{
    // The origin from which all rays start
    public Transform rayOrigin;

    // Total allowed ray length (original ray + reflections)
    public float maxRayLength = 50f;

    // Field of view (in degrees) to spread the rays
    public float fov = 30f;

    // Number of rays to cast within the given FOV
    public int numberOfRays = 5;

    // Update is called once per frame
    void Update() {
        if (rayOrigin == null) {
            Debug.LogWarning("Ray Origin is not assigned.");
            return;
        }

        // Loop to create rays spread evenly across the given FOV.
        // The central ray is at rayOrigin.forward.
        for (int i = 0; i < numberOfRays; i++) {
            // Calculate angle offset. This evenly spaces rays in [-fov/2, fov/2].
            // If numberOfRays == 1, angleOffset will be 0.
            float angleOffset = (numberOfRays > 1) ? -fov / 2 + (fov * i / (numberOfRays - 1)) : 0f;

            // Rotate the forward vector by the angle offset around the rayOrigin's up axis.
            Vector3 rayDirection = Quaternion.AngleAxis(angleOffset, rayOrigin.up) * rayOrigin.forward;

            // Start the recursive raycasting for this ray.
            CastRayRecursive(rayOrigin.position, rayDirection, maxRayLength);
        }
    }

    /// <summary>
    /// Recursively casts a ray from the given start position and direction for the remaining length.
    /// It draws the ray segments and reflects when a mirror is hit.
    /// </summary>
    /// <param name="startPosition">Start position of the current ray segment</param>
    /// <param name="direction">Direction of the current ray segment</param>
    /// <param name="remainingLength">Remaining length allowed for this ray segment</param>
    void CastRayRecursive(Vector3 startPosition, Vector3 direction, float remainingLength) {
        if (remainingLength <= 0)
            return;

        Ray ray = new Ray(startPosition, direction);
        RaycastHit hit;

        // Cast the ray for the current remaining length.
        if (Physics.Raycast(ray, out hit, remainingLength)) {
            // Draw the ray segment from start to the hit point.
            Debug.DrawLine(startPosition, hit.point, Color.red);

            // If the hit object is tagged "Mirror", reflect the ray.
            if (hit.collider.gameObject.CompareTag("Reflective")) {
                // Compute the reflection vector using the hit normal.
                Vector3 reflectionDirection = Vector3.Reflect(direction, hit.normal);

                // Subtract the distance traveled from the remaining length.
                float newRemainingLength = remainingLength - hit.distance;

                // Recursively cast the reflected ray.
                CastRayRecursive(hit.point, reflectionDirection, newRemainingLength);
            } else  {
                // For non-mirror objects, you may draw an indicator and stop the ray.
                Debug.DrawRay(hit.point, hit.normal, Color.blue);
            }
        } else {
            // If nothing was hit, draw the entire remaining ray segment.
            Debug.DrawLine(startPosition, startPosition + direction * remainingLength, Color.red);
        }
    }
}
