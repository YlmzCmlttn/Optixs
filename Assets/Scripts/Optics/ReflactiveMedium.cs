using UnityEngine;

public class ReflactiveMedium : OpticalElement
{
    public RefractiveIndexData refractiveIndexData;
    private const float AIR_REFRACTIVE_INDEX = 1.0f;
    public bool isInside = false; // Track if the ray is inside or outside the medium

    public override void OnLaserHit(LaserRay laser, RaycastHit2D hit)
    {
        float incidentAngle = Mathf.Acos(-Vector3.Dot(hit.normal.normalized, laser.Direction.normalized)) * Mathf.Rad2Deg;
        Debug.Log($"Correct Incident Angle: {incidentAngle}");
        Vector3 incidentDir = laser.Direction.normalized;
        Vector3 normal = hit.normal.normalized;
        float n1, n2;
        float dot = Vector2.Dot(laser.Direction.normalized, normal);
        
        isInside = laser.isInside;
        // Determine if the ray is entering or exiting the medium
        if (isInside)
        {
            Debug.Log("Hit from inside");
            n1 = GetRefractiveIndex(laser.laserColor);  // Inside refractive medium
            n2 = AIR_REFRACTIVE_INDEX;                 // Exiting to air
            //normal = -normal; // Flip normal to handle exiting correctly
        }
        else
        {
            Debug.Log("Hit from outside");
            n1 = AIR_REFRACTIVE_INDEX;                 // Air to refractive medium
            n2 = GetRefractiveIndex(laser.laserColor);
        }

        Vector3 refracted = Refract(incidentDir, normal, n1, n2, ref laser.isInside);
        Debug.DrawRay(hit.point, refracted, Color.green);
        Debug.DrawRay(hit.point, incidentDir, Color.blue);
        Debug.DrawRay(hit.point, normal, Color.yellow);
        laser.CastLaser(hit.point, refracted);

        // Vector3 I = new Vector3(0.707107f,-0.707107f,0);
        // Vector3 n = new Vector3(0,1,0);
        // float nr1 = 1.3333f;
        // float nr2 = 1.0f;
        // Vector3 R = Refract(I, n, nr1, nr2);
        // Debug.Log("R: "+R);
        //Debug.DrawRay(hit.point, R, Color.red);
    }

    private float GetRefractiveIndex(LaserColor color)
    {
        switch (color)
        {
            case LaserColor.Red:
                return refractiveIndexData.RedRefractiveIndex;
            case LaserColor.Green:
                return refractiveIndexData.GreenRefractiveIndex;
            case LaserColor.Blue:
                return refractiveIndexData.BlueRefractiveIndex;
            default:
                return 1.5f;
        }
    }

    private Vector3 Refract(Vector3 incident, Vector3 normal, float n1, float n2, ref bool isInside)
    {
        incident = incident.normalized;
        normal = normal.normalized;
        
        float eta = n1 / n2;
        float cosI = -Vector3.Dot(normal, incident);
        float sinT2 = eta * eta * (1.0f - cosI * cosI);

        // Check for total internal reflection
        Debug.Log("CosI "+cosI+ "sinT2: " + sinT2);
        if (sinT2 > 1.0f)
        {
            Debug.Log("Total Internal Reflection occurred");
            return Vector3.Reflect(incident, normal);
        }

        float cosT = Mathf.Sqrt(1.0f - sinT2);
        isInside = !isInside;
        return (eta * incident + (eta * cosI - cosT) * normal).normalized;
    }
    
} 