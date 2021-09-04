using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RocketUI.Utilities.Helpers
{
    public static class MathHelpers
    {
        public const double PiOver180 = Math.PI / 180.0;
        private const double _180OverPi = 180.0 / Math.PI;

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        /// <remarks>
        /// This method uses double precission internally,
        /// though it returns single float
        /// Factor = 180 / pi
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(this float radians)
        { 
            return (float)(radians * _180OverPi);
        }
        
        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        /// <remarks>
        /// This method uses double precission internally,
        /// though it returns single float
        /// Factor = pi / 180
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(this float degrees)
        { 
            return (float)(degrees * PiOver180);
        }
        
        public static int IntCeil(double value)
        {
            var i = (int) value;
            return value > i ? i + 1 : i;
        }
        public static double SinInterpolation(double a, double b, double t)
        {
            return a + Math.Sin(t * PiOver180) * (b - a);
        }
        
        public static int RoundToNearestInterval(int value, int interval)
        {	
            var scale = (1f / interval);
            return (int)(Math.Round(value * scale) / scale);
        }

        public static double RoundToNearestInterval(double value, double interval)
        {	
            var scale = (1d / interval);
            return Math.Round(value * scale) / scale;
        }

        public static float RoundToNearestInterval(float value, float interval)
        {	
            var scale = (1f / interval);
            return (float)(Math.Round(value * scale) / scale);
        }

        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(max, Math.Max(min, value));
        }
        
        public static double Clamp(double value, double min, double max)
        {
            return Math.Min(max, Math.Max(min, value));
        }
        
        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(max, Math.Max(min, value));
        }
        
        public static byte Clamp(byte value, byte min, byte max)
        {
            return Math.Min(max, Math.Max(min, value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point ToPoint(this Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }
    }
}