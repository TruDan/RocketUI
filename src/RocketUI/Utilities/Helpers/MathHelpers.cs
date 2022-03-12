using System;
using System.Runtime.CompilerServices;

namespace RocketUI.Utilities.Helpers
{
    public static class MathHelpers
    {
        public const double PiOver180 = Math.PI / 180.0;
        public const double PiOver2 = Math.PI / 2.0;
        public const double Pi = Math.PI;

        public const float PiOver180f = (float)(Math.PI / 180.0f);
        public const float PiOver2f = (float)(Math.PI / 2.0f);
        public const float Pif = (float)Math.PI;

        public static int IntCeil(double value)
        {
            var i = (int)value;
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
        public static float ToRadians(this float degrees) => degrees * ((float)Math.PI / 180f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(this float radians) => radians * (180f / (float)Math.PI);
    }
}