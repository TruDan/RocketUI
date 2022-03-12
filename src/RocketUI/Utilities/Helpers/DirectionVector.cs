using System.Numerics;

namespace RocketUI.Utilities.Helpers;

public static class DirectionVector
{
    public static readonly Vector3 Zero = new Vector3(0.0f, 0.0f, 0.0f);
    public static readonly Vector3 One = new Vector3(1f, 1f, 1f);
    public static readonly Vector3 UnitX = new Vector3(1f, 0.0f, 0.0f);
    public static readonly Vector3 UnitY = new Vector3(0.0f, 1f, 0.0f);
    public static readonly Vector3 UnitZ = new Vector3(0.0f, 0.0f, 1f);
    public static readonly Vector3 Up = new Vector3(0.0f, 1f, 0.0f);
    public static readonly Vector3 Down = new Vector3(0.0f, -1f, 0.0f);
    public static readonly Vector3 Right = new Vector3(1f, 0.0f, 0.0f);
    public static readonly Vector3 Left = new Vector3(-1f, 0.0f, 0.0f);
    public static readonly Vector3 Forward = new Vector3(0.0f, 0.0f, -1f);
    public static readonly Vector3 Backward = new Vector3(0.0f, 0.0f, 1f);
}