using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WallGenerator : MonoBehaviour
{
    // Define your wall points in the inspector (in clockwise or counter-clockwise order)
    public List<Vector2> wallPoints = new List<Vector2>()
    {
        new Vector2(0, 0),
        new Vector2(2, 1),
        new Vector2(3, 0),
        new Vector2(4, 1),
        new Vector2(3, 3),
        new Vector2(1, 2)
    };
    void Start()
    {
        GenerateWallMesh();
    }

    void GenerateWallMesh()
    {
        // Create an array of vertices (convert Vector2 to Vector3)
        Vector3[] vertices = new Vector3[wallPoints.Count];
        for (int i = 0; i < wallPoints.Count; i++)
        {
            vertices[i] = new Vector3(wallPoints[i].x, wallPoints[i].y, 0f);
        }

        // Triangulate the polygon
        // For a convex polygon, you can create triangles fan-style.
        List<int> triangles = new List<int>();
        for (int i = 1; i < wallPoints.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // Create and assign the mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles.ToArray()
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}
