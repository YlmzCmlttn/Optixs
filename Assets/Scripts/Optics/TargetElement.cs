using UnityEngine;

public class TargetElement : OpticalElement
{
    public LaserColor targetColor;
    private LaserColor hittedColor = LaserColor.None;

    public override void OnLaserHit(LaserRay laser, RaycastHit2D hit)
    {
        hittedColor |= laser.laserColor; // Accumulate all colors that hit the target
    }

    void LateUpdate()
    {
        LaserColor missingColors = targetColor & ~hittedColor; // Find colors that are required but not present
        LaserColor extraColors = hittedColor & ~targetColor; // Find colors that are present but not required

        if (hittedColor == targetColor) 
        {
            Debug.Log("Target COMPLETED! Correct color: " + hittedColor);
        }
        else if (missingColors != LaserColor.None && extraColors == LaserColor.None) 
        {
            Debug.Log("PARTIALLY Correct. Missing colors: " + missingColors);
        }
        else if (missingColors == LaserColor.None && extraColors != LaserColor.None) 
        {
            Debug.Log("WRONG! Extra colors detected: " + extraColors);
        }
        else if (missingColors != LaserColor.None && extraColors != LaserColor.None) 
        {
            Debug.Log("WRONG! Missing colors: " + missingColors + ", Extra colors: " + extraColors);
        }
        else 
        {
            Debug.Log("WRONG color! No required colors detected.");
        }

        hittedColor = LaserColor.None; // Reset for the next frame
    }
}
