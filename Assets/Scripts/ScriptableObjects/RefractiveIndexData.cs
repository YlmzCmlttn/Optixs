using UnityEngine;

[CreateAssetMenu(fileName = "New Refractive Index Data", menuName = "Optixs/Refractive Index Data", order = 1)]
public class RefractiveIndexData : ScriptableObject
{
    public float RedRefractiveIndex = 1.5f;
    public float GreenRefractiveIndex = 1.6f;
    public float BlueRefractiveIndex = 1.7f;
} 