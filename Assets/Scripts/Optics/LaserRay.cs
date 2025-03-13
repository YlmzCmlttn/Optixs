using UnityEngine;
using System.Collections.Generic;

public class LaserRay : MonoBehaviour
{
    public LaserColor laserColor;
    private List<Vector3> m_LaserPath = new List<Vector3>();
    public Vector3 Direction { get; private set; }
    
    // Add this field for toggle control
    public bool showDebugRays = true;
    
    // Add these fields for Gizmos customization
    public Color gizmoColor = Color.red;
    public float gizmoLineWidth = 0.1f;
    private int counter = 0;
    public Vector2 rayDirection = Vector2.right;
    public bool isInside = false;
    void Update()
    {
        counter = 0;
        isInside = false;
        m_LaserPath.Clear();
        CastLaser(transform.position, rayDirection);
    }

    public void CastLaser(Vector3 origin, Vector3 direction)
    {
        counter++;
        Debug.Log("CastLaser origin: " + origin + " direction: " + direction + "Counter: "+counter + "isInside: "+isInside);
        if(counter > 5){
            Debug.Log("Counter > 5");
            return;
        }
        m_LaserPath.Add(origin);
        Direction = direction;

        const float OFFSET = 0.01f;
        Vector3 offsetOrigin = origin + (direction * OFFSET);

        RaycastHit2D hit = Physics2D.Raycast(offsetOrigin, direction);
        if (hit.collider != null)
        {
            // Check if we hit a WALL (Stops the ray)
            if (hit.collider.CompareTag("Wall"))
            {
                m_LaserPath.Add(hit.point);
            }else if (hit.collider.CompareTag("Target"))
            {
                m_LaserPath.Add(hit.point);
            }
            

            // Handle Optical Element
            OpticalElement element = hit.collider.GetComponent<OpticalElement>();
            if (element != null)
            {
                element.OnLaserHit(this, hit);
                return;
            }else{
                Debug.Log("No optical element found");
            }
            // Reflect and continue
            // origin = hit.point;
            // direction = Vector3.Reflect(direction, hit.normal);
        }else{
            m_LaserPath.Add(origin + (direction * 10f));
        }
    }

    // Add this method for Gizmos visualization
    private void OnDrawGizmos()
    {
        if (!showDebugRays || m_LaserPath.Count == 1){
            Gizmos.DrawLine(transform.position, transform.position + Direction * 10f);
            return;
        }
        if (!showDebugRays || m_LaserPath.Count < 2) return;

        Gizmos.color = gizmoColor;
        
        // Draw lines between each point in the laser path
        for (int i = 0; i < m_LaserPath.Count - 1; i++)
        {
            Gizmos.DrawLine(m_LaserPath[i], m_LaserPath[i + 1]);
            // Optional: Draw spheres at each point for better visualization
            Gizmos.DrawSphere(m_LaserPath[i], gizmoLineWidth * 0.5f);
        }
        // Draw sphere at the last point
        Gizmos.DrawSphere(m_LaserPath[m_LaserPath.Count - 1], gizmoLineWidth * 0.5f);
    }
}
