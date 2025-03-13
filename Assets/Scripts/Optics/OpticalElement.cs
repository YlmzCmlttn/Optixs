using UnityEngine;

public abstract class OpticalElement : MonoBehaviour
{
    public abstract void OnLaserHit(LaserRay laser, RaycastHit2D hit);
}
