using System;
using System.Numerics;
using RocketUI.Utilities.Helpers;

namespace RocketUI.Utilities.Extensions;

public static class NumericsExtensions
{
    public static void ExtractRotation(this Matrix4x4 matrix, ref Quaternion rotation)
    {
        if (float.IsNaN(matrix.M11))
            return;

        var sx = (Math.Sign(matrix.M11 * matrix.M12 * matrix.M13 * matrix.M14) < 0) ? -1.0f : 1.0f;
        var sy = (Math.Sign(matrix.M21 * matrix.M22 * matrix.M23 * matrix.M24) < 0) ? -1.0f : 1.0f;
        var sz = (Math.Sign(matrix.M31 * matrix.M32 * matrix.M33 * matrix.M34) < 0) ? -1.0f : 1.0f;

        sx *= (float)Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12 + matrix.M13 * matrix.M13);
        sy *= (float)Math.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22 + matrix.M23 * matrix.M23);
        sz *= (float)Math.Sqrt(matrix.M31 * matrix.M31 + matrix.M32 * matrix.M32 + matrix.M33 * matrix.M33);

        if (sx == 0.0 || sy == 0.0 || sz == 0.0)
        {
            rotation = Quaternion.Identity;
            return;
        }

        var m = new Matrix4x4(matrix.M11 / sx, matrix.M12 / sx, matrix.M13 / sx, 0.0f,
            matrix.M21 / sy, matrix.M22 / sy, matrix.M23 / sy, 0.0f,
            matrix.M31 / sz, matrix.M32 / sz, matrix.M33 / sz, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        rotation = Quaternion.CreateFromRotationMatrix(m);
    }
    
    public static void ExtractRotation(this Matrix4x4 matrix, ref Vector3 rotation)
    {
        Quaternion q = Quaternion.Identity;
        ExtractRotation(matrix, ref q);
        q.ToEuler(ref rotation);
    }
    
    
        public static float ArcTanAngle(float X, float Y)
        {
            if (X == 0)
            {
                if (Y == 1)
                    return (float)MathHelpers.PiOver2;
                else
                    return (float)-MathHelpers.PiOver2;
            }
            else if (X > 0)
                return (float)Math.Atan(Y / X);
            else if (X < 0)
            {
                if (Y > 0)
                    return (float)Math.Atan(Y / X) + MathHelpers.Pif;
                else
                    return (float)Math.Atan(Y / X) - MathHelpers.Pif;
            }
            else
                return 0;
        }
        
        //returns Euler angles that point from one point to another
        public static Vector3 AngleTo(Vector3 from, Vector3 location)
        {
            Vector3 angle = new Vector3();
            Vector3 v3    = Vector3.Normalize(location - from);
            angle.X = (float)Math.Asin(v3.Y);
            angle.Y = ArcTanAngle(-v3.Z, -v3.X);
            return angle;
        }

        public static void ToEuler(float x, float y, float z, float w, ref Vector3 result)
        {
            var rotation = new Quaternion(x, y, z, w);
            var forward  = Vector3.Transform(DirectionVector.Forward, rotation);
            var up       = Vector3.Transform(DirectionVector.Up, rotation);
            result = AngleTo(new Vector3(), forward);
            if (result.X == MathHelpers.PiOver2)
            {
                result.Y = ArcTanAngle(up.Z, up.X);
                result.Z = 0;
            }
            else if (result.X == -MathHelpers.PiOver2)
            {
                result.Y = ArcTanAngle(-up.Z, -up.X);
                result.Z = 0;
            }
            else
            {
                up = Vector3.Transform(up, Matrix4x4.CreateRotationY(-result.Y));
                up = Vector3.Transform(up, Matrix4x4.CreateRotationX(-result.X));
                result.Z = ArcTanAngle(up.Y, -up.X);
            }
        }
        
        public static void ToEuler(this Quaternion quaternion, ref Vector3 result)
        {
            ToEuler(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W, ref result);
        }

        public static Vector3 ToEuler(this Quaternion quaternion)
        {
            Vector3 result = Vector3.Zero;
            ToEuler(quaternion, ref result);
            return result;
        }

        public static Quaternion Euler(float x, float y, float z) => Quaternion.CreateFromYawPitchRoll(y, x, z);

        public static Quaternion Euler(Vector3 rotation) => Euler(rotation.X, rotation.Y, rotation.Z);
}