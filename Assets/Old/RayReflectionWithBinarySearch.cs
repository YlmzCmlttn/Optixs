using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RayReflectorWithBinarySearch : MonoBehaviour {

    // The origin from which all rays start
    public Transform rayOrigin;

    // Total allowed ray length (original ray + reflections)
    public float maxRayLength = 50f;

    // Field of view (in degrees) to spread the rays
    public float fov = 30f;

    // Number of rays to cast within the given FOV
    public int m_NumberOfRays = 5;


    // Update is called once per frame
    void Update() {
        if (rayOrigin == null) {
            Debug.LogWarning("Ray Origin is not assigned.");
            return;
        }
        float leftmostAngle = -fov / 2;
        float rightmostAngle = fov / 2;

        Vector3 leftMostRayDirection = Quaternion.AngleAxis(leftmostAngle, rayOrigin.up) * rayOrigin.forward;
        Vector3 centerRayDirection = Quaternion.AngleAxis(0, rayOrigin.up) * rayOrigin.forward;
        Vector3 rightMostRayDirection = Quaternion.AngleAxis(rightmostAngle, rayOrigin.up) * rayOrigin.forward;

        DoesItHit(rayOrigin.position, centerRayDirection, maxRayLength);

        //Debug.DrawLine(rayOrigin.position, rayOrigin.position + leftMostRayDirection * maxRayLength, Color.red);
        //Debug.DrawLine(rayOrigin.position, rayOrigin.position + centerRayDirection * maxRayLength, Color.red);
        //Debug.DrawLine(rayOrigin.position, rayOrigin.position + rightMostRayDirection * maxRayLength, Color.red);

    }

    void RayTracing() {

    }

    void DoesItHit(Vector3 position, Vector3 direction, float remainingLength) {
        Ray ray = new Ray(position, direction);
        RaycastHit hit;

        // Cast the ray for the given remaining length.
        if (Physics.Raycast(ray, out hit, remainingLength)) {
            // If the hit object is tagged "Mirror", then process it.
            if (hit.collider.gameObject.CompareTag("Mirror")) {
                // Draw the initial hit line.
                Debug.DrawLine(position, hit.point, Color.yellow);

                // Optionally, create an "imaginary ray image" (reflection ray)
                // Here we create a ray starting at the mirror's position with the direction opposite to the hit normal.
                Ray rayFromObject = new Ray(hit.collider.gameObject.transform.position, -hit.normal);
                // (You can use rayFromObject.direction later if needed for further calculations.)

                // Try to compute the mirror's leftmost and rightmost edge points.
                // This example assumes the mirror has a BoxCollider and that its local right vector (transform.right)
                // represents the horizontal (left/right) axis.


                // Get the mirror's transform.
                Transform mirrorTransform = hit.collider.transform;
                // For a quad (default size 1x1), the half width is 0.5 times its scale along x.
                float halfWidth = 0.5f * mirrorTransform.lossyScale.x;

                // Compute the left and right edge positions in world space.
                // Assuming the quad's local right (transform.right) points to its right edge.
                Vector3 leftEdge = mirrorTransform.position - mirrorTransform.right * halfWidth;
                Vector3 rightEdge = mirrorTransform.position + mirrorTransform.right * halfWidth;



                // Draw lines from the mirror center to the left and right edges.
                Debug.DrawLine(mirrorTransform.position, leftEdge, Color.cyan);
                Debug.DrawLine(mirrorTransform.position, rightEdge, Color.cyan);

                // Now, to draw a line from your source (rayOrigin) to, say, the left edge,
                // you can do:
                Debug.DrawLine(rayOrigin.position, leftEdge, Color.magenta);

            } else {
                // Handle non-mirror hits if needed.
            }
        } else {
            // Optionally, handle the case when nothing is hit.
        }
    }

    void BinarySearch(Vector3 position,Vector3 leftDirection, Vector3 rightDirection,float remainingLength,float threshold) {
        if ((leftDirection - rightDirection).magnitude < threshold) {
            return;
        }
        Vector3 middle = (leftDirection + rightDirection) /2.0f;
        
        Ray ray = new Ray(position, middle);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, remainingLength)) {
            if (hit.collider.gameObject.CompareTag("Mirror")) {
                Debug.DrawLine(position, hit.point, Color.red);
                return;
            }
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
            if (hit.collider.gameObject.CompareTag("Mirror")) {
                // Compute the reflection vector using the hit normal.
                Vector3 reflectionDirection = Vector3.Reflect(direction, hit.normal);

                // Subtract the distance traveled from the remaining length.
                float newRemainingLength = remainingLength - hit.distance;

                // Recursively cast the reflected ray.
                CastRayRecursive(hit.point, reflectionDirection, newRemainingLength);
            } else {
                // For non-mirror objects, you may draw an indicator and stop the ray.
                Debug.DrawRay(hit.point, hit.normal, Color.blue);
            }
        } else {
            // If nothing was hit, draw the entire remaining ray segment.
            Debug.DrawLine(startPosition, startPosition + direction * remainingLength, Color.red);
        }
    }
}
