﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using RocketUI.Utilities.Converters;

namespace RocketUI
{
    [Flags]
    [TypeConverter(typeof(EnumTypeConverter<Alignment>))]

    public enum Alignment : int
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
        private static Alignment[] _baseAlignments = new Alignment[]
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
                    break;
                case Alignment.None:
                    return Alignment.None;
                    break;
                case Alignment.NoneX:
                    return Alignment.NoneX;
                    break;
                case Alignment.NoneY:
                    return Alignment.NoneY;
                    break;
                case Alignment.MinX:
                    return Alignment.MinY;
                    break;
                case Alignment.MinY:
                    return Alignment.MinX;
                    break;
                case Alignment.MaxX:
                    return Alignment.MaxY;
                    break;
                case Alignment.MaxY:
                    return Alignment.MaxX;
                    break;
                case Alignment.CenterX:
                    return Alignment.CenterY;
                    break;
                case Alignment.CenterY:
                    return Alignment.CenterX;
                    break;
                case Alignment.FillX:
                    return Alignment.FillY;
                    break;
                case Alignment.FillY:
                    return Alignment.FillX;
                    break;
                case Alignment.Default:
                    return Alignment.Default;
                    break;
                case Alignment.TopLeft:
                    return Alignment.TopLeft;
                    break;
                case Alignment.TopCenter:
                    return Alignment.MiddleLeft;
                    break;
                case Alignment.TopRight:
                    return Alignment.BottomLeft;
                    break;
                case Alignment.TopFill:
                    return Alignment.MiddleFill;
                    break;
                case Alignment.MiddleLeft:
                    return Alignment.TopCenter;
                    break;
                case Alignment.MiddleCenter:
                    return Alignment.MiddleCenter;
                    break;
                case Alignment.MiddleRight:
                    return Alignment.BottomCenter;
                    break;
                case Alignment.MiddleFill:
                    return Alignment.FillCenter;
                    break;
                case Alignment.BottomLeft:
                    return Alignment.TopRight;
                    break;
                case Alignment.BottomCenter:
                    return Alignment.MiddleRight;
                    break;
                case Alignment.BottomRight:
                    return Alignment.BottomRight;
                    break;
                case Alignment.BottomFill:
                    return Alignment.FillRight;
                    break;
                case Alignment.FillLeft:
                    return Alignment.TopFill;
                    break;
                case Alignment.FillCenter:
                    return Alignment.MiddleFill;
                    break;
                case Alignment.FillRight:
                    return Alignment.BottomFill;
                    break;
                case Alignment.Fill:
                    return Alignment.Fill;
                    break;
                case Alignment.OrientationX:
                    return Alignment.OrientationY;
                    break;
                case Alignment.OrientationY:
                    return Alignment.OrientationX;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
            }

            return alignment;
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
            checkAlignments = checkAlignments ?? _baseAlignments;

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