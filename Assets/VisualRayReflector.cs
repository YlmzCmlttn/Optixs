using UnityEngine;
using System.Collections.Generic;

public class VisualRayReflector : MonoBehaviour {
    // The origin from which all rays start.
    public Transform rayOrigin;

    // Total allowed length for the ray (original + reflections).
    public float maxRayLength = 50f;

    // Field of view (in degrees) to spread the rays.
    public float fov = 30f;

    // Number of rays to cast within the given FOV.
    public int numberOfRays = 5;

    // Prefab that contains a MeshFilter and MeshRenderer for visualizing the beam.
    public GameObject meshLinePrefab;

    // Beam width (half width used to offset vertices).
    public float beamWidth = 0.2f;

    // List to keep track of the current instantiated mesh objects.
    private List<GameObject> currentMeshes = new List<GameObject>();

    void Update() {
        // Clear previously instantiated beam meshes.
        foreach (GameObject meshObj in currentMeshes) {
            Destroy(meshObj);
        }
        currentMeshes.Clear();

        // Cast multiple rays across the specified FOV.
        for (int i = 0; i < numberOfRays; i++) {
            // Calculate an angle offset so rays are spread evenly over [-fov/2, fov/2].
            float angleOffset = (numberOfRays > 1)
                ? -fov / 2 + (fov * i / (numberOfRays - 1))
                : 0f;

            // Rotate the forward direction by the angle offset around the rayOrigin's up axis.
            Vector3 rayDirection = Quaternion.AngleAxis(angleOffset, rayOrigin.up) * rayOrigin.forward;

            // List to store the ray's reflection points.
            List<Vector3> rayPoints = new List<Vector3>();
            rayPoints.Add(rayOrigin.position);

            // Collect all reflection points recursively.
            CastRayRecursive(rayOrigin.position, rayDirection, maxRayLength, rayPoints);

            // Instantiate the mesh prefab.
            GameObject meshObj = Instantiate(meshLinePrefab, Vector3.zero, Quaternion.identity, transform);
            MeshFilter mf = meshObj.GetComponent<MeshFilter>();
            Mesh mesh = CreateBeamMesh(rayPoints, beamWidth);
            mf.mesh = mesh;

            // Optionally, tweak the MeshRenderer (e.g., assign an animated/glowing material).

            // Store the mesh object for cleanup.
            currentMeshes.Add(meshObj);
        }
    }

    /// <summary>
    /// Recursively casts a ray from the given position and direction, collecting reflection points.
    /// </summary>
    void CastRayRecursive(Vector3 startPosition, Vector3 direction, float remainingLength, List<Vector3> rayPoints) {
        if (remainingLength <= 0)
            return;

        Ray ray = new Ray(startPosition, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, remainingLength)) {
            rayPoints.Add(hit.point);
            if (hit.collider.gameObject.CompareTag("Mirror")) {
                Vector3 reflectionDirection = Vector3.Reflect(direction, hit.normal);
                float newRemainingLength = remainingLength - hit.distance;
                CastRayRecursive(hit.point, reflectionDirection, newRemainingLength, rayPoints);
            }
        } else {
            rayPoints.Add(startPosition + direction * remainingLength);
        }
    }

    /// <summary>
    /// Creates a quad-strip mesh along the given list of points.
    /// </summary>
    Mesh CreateBeamMesh(List<Vector3> points, float width) {
        Mesh mesh = new Mesh();
        if (points.Count < 2)
            return mesh;

        // Two vertices per point (for left and right offsets).
        int vertexCount = points.Count * 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(points.Count - 1) * 6];
        Vector2[] uvs = new Vector2[vertexCount];

        for (int i = 0; i < points.Count; i++) {
            // Determine the direction of the current segment.
            Vector3 forward;
            if (i == points.Count - 1)
                forward = (points[i] - points[i - 1]).normalized;
            else
                forward = (points[i + 1] - points[i]).normalized;

            // Compute a perpendicular vector.
            Vector3 perpendicular = Vector3.Cross(forward, Vector3.up).normalized * width;
            // If forward is parallel to up, choose another axis.
            if (perpendicular == Vector3.zero)
                perpendicular = Vector3.Cross(forward, Vector3.right).normalized * width;

            // Create two vertices per point: one on each side of the ray.
            vertices[i * 2] = points[i] + perpendicular;
            vertices[i * 2 + 1] = points[i] - perpendicular;

            // Set UV coordinates along the beam length (for animated texture effects).
            float v = (float)i / (points.Count - 1);
            uvs[i * 2] = new Vector2(0, v);
            uvs[i * 2 + 1] = new Vector2(1, v);
        }

        // Build the triangle indices.
        int triIndex = 0;
        for (int i = 0; i < points.Count - 1; i++) {
            int baseIndex = i * 2;
            // First triangle.
            triangles[triIndex++] = baseIndex;
            triangles[triIndex++] = baseIndex + 2;
            triangles[triIndex++] = baseIndex + 1;
            // Second triangle.
            triangles[triIndex++] = baseIndex + 1;
            triangles[triIndex++] = baseIndex + 2;
            triangles[triIndex++] = baseIndex + 3;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
