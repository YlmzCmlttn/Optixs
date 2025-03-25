using System.Collections.Generic;
using UnityEngine;

public class EarClippingTriangulation : MonoBehaviour
{
    // List of vertices that define your polygon (should be in consistent order)
    public List<Vector2> polygon;

    // List to store triangle indices (each set of 3 indices represents a triangle)
    public List<int> triangleIndices;

    void Start()
    {
        if (polygon != null && polygon.Count >= 3)
        {
            triangleIndices = Triangulate(polygon);
            // You can now use triangleIndices to create a Mesh, for example.
        }
        else
        {
            Debug.LogWarning("Polygon must have at least 3 vertices.");
        }
    }

    
    void Update()
    {
        // Draw the polygon outline
        // Draw the triangulation (triangles)
        //DrawPolygon();
        DrawTriangles();
    }

    // Draws the polygon outline using Debug.DrawLine
    void DrawPolygon()
    {
        if (polygon == null || polygon.Count < 2)
            return;

        for (int i = 0; i < polygon.Count; i++)
        {
            // Get current vertex and the next vertex (wrapping around)
            Vector2 current = polygon[i];
            Vector2 next = polygon[(i + 1) % polygon.Count];

            // Convert to Vector3 (assuming z = 0)
            Vector3 start = new Vector3(current.x, current.y, 0f);
            Vector3 end = new Vector3(next.x, next.y, 0f);

            Debug.DrawLine(start, end, Color.green);
        }
    }

    // Draws each triangle (obtained from triangulation) using Debug.DrawLine
    void DrawTriangles()
    {
        if (triangleIndices == null || triangleIndices.Count < 3)
            return;

        // Iterate through each triangle (3 indices per triangle)
        for (int i = 0; i < triangleIndices.Count; i += 3)
        {
            Vector2 v1 = polygon[triangleIndices[i]];
            Vector2 v2 = polygon[triangleIndices[i + 1]];
            Vector2 v3 = polygon[triangleIndices[i + 2]];

            // Convert the vertices to Vector3 (assuming z = 0)
            Vector3 p1 = new Vector3(v1.x, v1.y, 0f);
            Vector3 p2 = new Vector3(v2.x, v2.y, 0f);
            Vector3 p3 = new Vector3(v3.x, v3.y, 0f);

            // Draw the edges of the triangle in red
            Debug.DrawLine(p1, p2, Color.red);
            Debug.DrawLine(p2, p3, Color.red);
            Debug.DrawLine(p3, p1, Color.red);
        }
    }
    List<int> Triangulate(List<Vector2> poly)
    {
        List<int> triangles = new List<int>();
        // Create a working list of vertex indices.
        List<int> vertices = new List<int>();
        for (int i = 0; i < poly.Count; i++)
            vertices.Add(i);

        // Continue clipping ears until only one triangle remains.
        while (vertices.Count > 3)
        {
            bool earFound = false;

            // Iterate through all vertices to find an ear.
            for (int i = 0; i < vertices.Count; i++)
            {
                int prevIndex = vertices[(i - 1 + vertices.Count) % vertices.Count];
                int currIndex = vertices[i];
                int nextIndex = vertices[(i + 1) % vertices.Count];

                Vector2 prev = poly[prevIndex];
                Vector2 curr = poly[currIndex];
                Vector2 next = poly[nextIndex];

                // Check if the angle at 'curr' is convex.
                if (IsConvex(prev, curr, next))
                {
                    bool earFoundThisIteration = true;
                    // Check that no other vertex is inside the triangle.
                    for (int j = 0; j < vertices.Count; j++)
                    {
                        int testIndex = vertices[j];
                        if (testIndex == prevIndex || testIndex == currIndex || testIndex == nextIndex)
                            continue;

                        if (PointInTriangle(poly[testIndex], prev, curr, next))
                        {
                            earFoundThisIteration = false;
                            break;
                        }
                    }

                    if (earFoundThisIteration)
                    {
                        // If an ear is found, add the triangle indices.
                        triangles.Add(prevIndex);
                        triangles.Add(currIndex);
                        triangles.Add(nextIndex);

                        // Remove the ear tip and break to restart the loop.
                        vertices.RemoveAt(i);
                        earFound = true;
                        break;
                    }
                }
            }

            if (!earFound)
            {
                Debug.LogWarning("No ear found. The polygon might be self-intersecting or degenerate.");
                break;
            }
        }

        // Add the final remaining triangle.
        if (vertices.Count == 3)
        {
            triangles.Add(vertices[0]);
            triangles.Add(vertices[1]);
            triangles.Add(vertices[2]);
        }

        return triangles;
    }

    // Determines if the angle formed by points (a, b, c) is convex.
    bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
    {
        // For a polygon in clockwise order, the cross product should be negative.
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) < 0;
    }

    // Checks if point pt lies inside the triangle defined by v1, v2, and v3 using barycentric coordinates.
    bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d1 = Sign(pt, v1, v2);
        float d2 = Sign(pt, v2, v3);
        float d3 = Sign(pt, v3, v1);

        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(hasNeg && hasPos);
    }

    float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
}
