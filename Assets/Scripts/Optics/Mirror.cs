using UnityEngine;

public class Mirror : OpticalElement
{
    public override void OnLaserHit(LaserRay laser, RaycastHit2D hit)
    {
        // Reflect the laser
        Debug.Log("Mirror Hit");
        Vector3 reflectDirection = Vector3.Reflect(laser.Direction, hit.normal);
        laser.CastLaser(hit.point, reflectDirection);
    }
} 