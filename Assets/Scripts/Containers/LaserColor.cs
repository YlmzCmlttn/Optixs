using System;

[Serializable]
[Flags]
public enum LaserColor
{
    None = 0,
    Red = 1 << 0,    // 1
    Green = 1 << 1,  // 2
    Blue = 1 << 2,   // 4
    Yellow = Red | Green,  // 3
    Cyan = Green | Blue,   // 6
    Purple = Red | Blue,   // 5
    White = Red | Green | Blue // 7
}
// LaserColor myColor = LaserColor.Red | LaserColor.Blue; // This represents Purple

// // Check if a color contains Red
// bool hasRed = (myColor & LaserColor.Red) == LaserColor.Red;

// // Check if a color is exactly Purple
// bool isPurple = myColor == LaserColor.Purple; 