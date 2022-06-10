using System;
using System.Collections.Generic;
using System.ComponentModel;
using RocketUI.Utilities.Converters;

namespace RocketUI
{
    [Flags]
    [TypeConverter(typeof(EnumTypeConverter<Alignment>))]

    public enum Alignment
    {
        Fixed   = -0b11111111,
        None    =  0b00000000,

        NoneX   =  0b00000001, 
        NoneY   =  0b00010000,

        MinX    =  0b00000010,
        MinY    =  0b00100000,

        MaxX    =  0b00000100,
        MaxY    =  0b01000000,

        CenterX =  0b00001000,
        CenterY =  0b10000000,
        
        //FillX = 0x0800,
        //FillY = 0x8000,
        FillX = MinX | MaxX,
        FillY = MinY | MaxY,
        
        //JustifyX = MinX | MaxX,
        //JustifyY = MinY | MaxY,
        
        Default = None | NoneX | NoneY,

        TopLeft   = MinY | MinX,
        TopCenter = MinY | CenterX,
        TopRight  = MinY | MaxX,
        TopFill   = MinY | FillX,
		
        MiddleLeft   = CenterY | MinX,
        MiddleCenter = CenterY | CenterX,
        MiddleRight  = CenterY | MaxX,
        MiddleFill   = CenterY | FillX,

        BottomLeft   = MaxY | MinX,
        BottomCenter = MaxY | CenterX,
        BottomRight  = MaxY | MaxX,
        BottomFill   = MaxY | FillX,
        
        FillLeft   = FillY | MinX,
        FillCenter = FillY | CenterX,
        FillRight  = FillY | MaxX,
        Fill       = FillY | FillX,

        //OrientationX = None | NoneX | MinX | MaxX | CenterX | FillX,
        //OrientationY = None | NoneY | MinY | MaxY | CenterY | FillY,
        OrientationX = 0b00001111,
        OrientationY = 0b11110000,
                       
    }

    public static class AlignmentExtensions
    {
        private static readonly Alignment[] BaseAlignments = new Alignment[]
        {
            Alignment.NoneX,
            Alignment.MinX,
            Alignment.MaxX,
            Alignment.CenterX,
            Alignment.FillX,

            Alignment.NoneY,
            Alignment.MinY,
            Alignment.MaxY,
            Alignment.CenterY,
            Alignment.FillY
        };

        public static Alignment SwapXy2(this Alignment alignment)
        {
            var vertical = (alignment & Alignment.OrientationY);
            var horizontal = (alignment & Alignment.OrientationX);

            var newVertical   = (Alignment)((int)horizontal << 4) & Alignment.OrientationY;
            var newHorizontal = (Alignment)((int)vertical   >> 4) & Alignment.OrientationX;

            return (newVertical | newHorizontal);
        }
        
        public static Alignment SwapXy(this Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.Fixed:
                    return Alignment.Fixed;
                case Alignment.None:
                    return Alignment.None;
                case Alignment.NoneX:
                    return Alignment.NoneX;
                case Alignment.NoneY:
                    return Alignment.NoneY;
                case Alignment.MinX:
                    return Alignment.MinY;
                case Alignment.MinY:
                    return Alignment.MinX;
                case Alignment.MaxX:
                    return Alignment.MaxY;
                case Alignment.MaxY:
                    return Alignment.MaxX;
                case Alignment.CenterX:
                    return Alignment.CenterY;
                case Alignment.CenterY:
                    return Alignment.CenterX;
                case Alignment.FillX:
                    return Alignment.FillY;
                case Alignment.FillY:
                    return Alignment.FillX;
                case Alignment.Default:
                    return Alignment.Default;
                case Alignment.TopLeft:
                    return Alignment.TopLeft;
                case Alignment.TopCenter:
                    return Alignment.MiddleLeft;
                case Alignment.TopRight:
                    return Alignment.BottomLeft;
                case Alignment.TopFill:
                    return Alignment.MiddleFill;
                case Alignment.MiddleLeft:
                    return Alignment.TopCenter;
                case Alignment.MiddleCenter:
                    return Alignment.MiddleCenter;
                    
                case Alignment.MiddleRight:
                    return Alignment.BottomCenter;
                    
                case Alignment.MiddleFill:
                    return Alignment.FillCenter;
                    
                case Alignment.BottomLeft:
                    return Alignment.TopRight;
                    
                case Alignment.BottomCenter:
                    return Alignment.MiddleRight;
                    
                case Alignment.BottomRight:
                    return Alignment.BottomRight;
                    
                case Alignment.BottomFill:
                    return Alignment.FillRight;
                    
                case Alignment.FillLeft:
                    return Alignment.TopFill;
                    
                case Alignment.FillCenter:
                    return Alignment.MiddleFill;
                    
                case Alignment.FillRight:
                    return Alignment.BottomFill;
                    
                case Alignment.Fill:
                    return Alignment.Fill;
                    
                case Alignment.OrientationX:
                    return Alignment.OrientationY;
                    
                case Alignment.OrientationY:
                    return Alignment.OrientationX;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
            }
        }

        public static string ToFullString(this Alignment alignment)
        {
            var vertical   = (alignment & Alignment.OrientationY);
            var horizontal = (alignment & Alignment.OrientationX);

            return $"({ToFullStringParts(horizontal)}) x ({ToFullStringParts(vertical)}) | {alignment.ToBinary()}";
        }
        public static string ToBinary(this Alignment alignment)
        {
            var binaryStr = Convert.ToString((int) alignment, 2).PadLeft(8, '0');

            var parts = new List<string>();
            var partCount = Math.Ceiling(binaryStr.Length / 4f);
            for (int i = 0; i < partCount; i++)
            {
                parts.Add(binaryStr.Substring(i * 4, 4));
            }

            return string.Join(" ", parts);
        }

        private static string ToFullStringParts(Alignment alignment, Alignment[] checkAlignments = null)
        {
            checkAlignments = checkAlignments ?? BaseAlignments;

            var parts = new List<string>();

            foreach (var check in checkAlignments)
            {
                if ((alignment & check) != 0b0)
                {
                    parts.Add(check.ToString());
                }
            }

            return string.Join(" | ", parts);
        }
    }
}