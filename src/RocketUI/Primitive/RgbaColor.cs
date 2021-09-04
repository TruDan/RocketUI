using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace RocketUI
{
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct RgbaColor
    {
        private uint _value;

        static RgbaColor()
        {
        }

        /// <summary>
        /// Constructs an RGBA color from a packed value.
        /// The value is a 32-bit unsigned integer, with R in the least significant octet.
        /// </summary>
        /// <param name="value">The packed value (ABGR).</param>
        [CLSCompliant(false)]
        public RgbaColor(uint value) => _value = value;

        /// <summary>
        /// Constructs an RGBA color from the XYZW unit length components of a vector.
        /// </summary>
        /// <param name="color">A <see cref="Vector4"/> representing color.</param>
        public RgbaColor(Vector4 color) : this(
            (int)((double)color.X * (double)byte.MaxValue),
            (int)((double)color.Y * (double)byte.MaxValue),
            (int)((double)color.Z * (double)byte.MaxValue),
            (int)((double)color.W * (double)byte.MaxValue)
        )
        {
        }

        /// <summary>
        /// Constructs an RGBA color from the XYZ unit length components of a vector. Alpha value will be opaque.
        /// </summary>
        /// <param name="color">A <see cref="Vector3"/> representing color.</param>
        public RgbaColor(Vector3 color)
            : this((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from a <see cref="RgbaColor"/> and an alpha value.
        /// </summary>
        /// <param name="color">A <see cref="RgbaColor"/> for RGB values of new <see cref="RgbaColor"/> instance.</param>
        /// <param name="alpha">The alpha component value from 0 to 255.</param>
        public RgbaColor(RgbaColor color, int alpha)
        {
            if ((alpha & 0xFFFFFF00) != 0)
            {
                var clampedA = (uint)Math.Clamp(alpha, Byte.MinValue, Byte.MaxValue);

                _value = (color._value & 0x00FFFFFF) | (clampedA << 24);
            }
            else
            {
                _value = (color._value & 0x00FFFFFF) | ((uint)alpha << 24);
            }
        }

        /// <summary>
        /// Constructs an RGBA color from color and alpha value.
        /// </summary>
        /// <param name="color">A <see cref="RgbaColor"/> for RGB values of new <see cref="RgbaColor"/> instance.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        public RgbaColor(RgbaColor color, float alpha):
            this(color, (int)(alpha * 255))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        public RgbaColor(float r, float g, float b)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        public RgbaColor(float r, float g, float b, float alpha)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255), (int)(alpha * 255))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public RgbaColor(int r, int g, int b) : this()
        {
            if (((r | g | b) & 0xFFFFFF00) != 0)
            {
                var clampedR = (uint)Math.Clamp(r, Byte.MinValue, Byte.MaxValue);
                var clampedG = (uint)Math.Clamp(g, Byte.MinValue, Byte.MaxValue);
                var clampedB = (uint)Math.Clamp(b, Byte.MinValue, Byte.MaxValue);
                
                _value = 0xFF000000 | (clampedB << 16) | (clampedG << 8) | (clampedR);
            }
            else
            {
                _value = 0xFF000000 | ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
            }
        }
        
        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        /// <param name="alpha">Alpha component value from 0 to 255.</param>
        public RgbaColor(int r, int g, int b, int alpha) : this()
        {
            if (((r | g | b | alpha) & 0xFFFFFF00) != 0)
            {
                var clampedR = (uint)Math.Clamp(r, Byte.MinValue, Byte.MaxValue);
                var clampedG = (uint)Math.Clamp(g, Byte.MinValue, Byte.MaxValue);
                var clampedB = (uint)Math.Clamp(b, Byte.MinValue, Byte.MaxValue);
                var clampedA = (uint)Math.Clamp(alpha, Byte.MinValue, Byte.MaxValue);

                _value = (clampedA << 24) | (clampedB << 16) | (clampedG << 8) | (clampedR);
            }
            else
            {
                _value = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
            }
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <remarks>
        /// This overload sets the values directly without clamping, and may therefore be faster than the other overloads.
        /// </remarks>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="alpha"></param>
        public RgbaColor(byte r, byte g, byte b, byte alpha) : this()
        {
            _value = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | (r);
        }
        
        
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        [DataMember]
        public byte B
        {
            get
            {
                unchecked
                {
                    return (byte) (this._value >> 16);
                }
            }
            set
            {
                this._value = (this._value & 0xff00ffff) | ((uint)value << 16);
            }
        }

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        [DataMember]
        public byte G
        {
            get
            {
                unchecked
                {
                    return (byte)(this._value >> 8);
                }
            }
            set
            {
                this._value = (this._value & 0xffff00ff) | ((uint)value << 8);
            }
        }

        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        [DataMember]
        public byte R
        {
            get
            {
                unchecked
                {
                    return (byte) this._value;
                }
            }
            set
            {
                this._value = (this._value & 0xffffff00) | value;
            }
        }

        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        [DataMember]
        public byte A
        {
            get
            {
                unchecked
                {
                    return (byte)(this._value >> 24);
                }
            }
            set
            {
                this._value = (this._value & 0x00ffffff) | ((uint)value << 24);
            }
        }
		
	/// <summary>
        /// Compares whether two <see cref="RgbaColor"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="RgbaColor"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="RgbaColor"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(RgbaColor a, RgbaColor b)
        {
            return (a._value == b._value);
        }
	
	/// <summary>
        /// Compares whether two <see cref="RgbaColor"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="RgbaColor"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="RgbaColor"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>	
        public static bool operator !=(RgbaColor a, RgbaColor b)
        {
            return (a._value != b._value);
        }

        /// <summary>
        /// Gets the hash code of this <see cref="RgbaColor"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="RgbaColor"/>.</returns>
        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }
	
        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        /// <param name="obj">The <see cref="RgbaColor"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return ((obj is RgbaColor) && this.Equals((RgbaColor)obj));
        }
        
	    /// <summary>
        /// Multiply <see cref="RgbaColor"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="RgbaColor"/>.</param>
        /// <param name="scale">Multiplicator.</param>
        /// <returns>Multiplication result.</returns>
	    public static RgbaColor Multiply(RgbaColor value, float scale)
	    {
	        return new RgbaColor((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
	    }
	
	    /// <summary>
        /// Multiply <see cref="RgbaColor"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="RgbaColor"/>.</param>
        /// <param name="scale">Multiplicator.</param>
        /// <returns>Multiplication result.</returns>
	    public static RgbaColor operator *(RgbaColor value, float scale)
        {
            return new RgbaColor((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
        }

        public static RgbaColor operator *(float scale, RgbaColor value)
        {
            return new RgbaColor((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
        }

        /// <summary>
        /// Gets a <see cref="Vector3"/> representation for this object.
        /// </summary>
        /// <returns>A <see cref="Vector3"/> representation for this object.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(R / 255.0f, G / 255.0f, B / 255.0f);
        }

        /// <summary>
        /// Gets a <see cref="Vector4"/> representation for this object.
        /// </summary>
        /// <returns>A <see cref="Vector4"/> representation for this object.</returns>
        public Vector4 ToVector4()
        {
            return new Vector4(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
        }
	
        /// <summary>
        /// Gets or sets packed value of this <see cref="RgbaColor"/>.
        /// </summary>
        [CLSCompliant(false)]
        public UInt32 PackedValue
        {
            get { return _value; }
            set { _value = value; }
        }


        internal string DebugDisplayString
        {
            get
            {
                return string.Concat(
                    this.R.ToString(), "  ",
                    this.G.ToString(), "  ",
                    this.B.ToString(), "  ",
                    this.A.ToString()
                );
            }
        }


        /// <summary>
        /// Returns a <see cref="String"/> representation of this <see cref="RgbaColor"/> in the format:
        /// {R:[red] G:[green] B:[blue] A:[alpha]}
        /// </summary>
        /// <returns><see cref="String"/> representation of this <see cref="RgbaColor"/>.</returns>
	public override string ToString ()
	{
        StringBuilder sb = new StringBuilder(25);
        sb.Append("{R:");
        sb.Append(R);
        sb.Append(" G:");
        sb.Append(G);
        sb.Append(" B:");
        sb.Append(B);
        sb.Append(" A:");
        sb.Append(A);
        sb.Append("}");
        return sb.ToString();
	}
	
	/// <summary>
        /// Translate a non-premultipled alpha <see cref="RgbaColor"/> to a <see cref="RgbaColor"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="vector">A <see cref="Vector4"/> representing color.</param>
        /// <returns>A <see cref="RgbaColor"/> which contains premultiplied alpha data.</returns>
        public static RgbaColor FromNonPremultiplied(Vector4 vector)
        {
            return new RgbaColor(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W, vector.W);
        }
	
	/// <summary>
        /// Translate a non-premultipled alpha <see cref="RgbaColor"/> to a <see cref="RgbaColor"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="r">Red component value.</param>
        /// <param name="g">Green component value.</param>
        /// <param name="b">Blue component value.</param>
        /// <param name="a">Alpha component value.</param>
        /// <returns>A <see cref="RgbaColor"/> which contains premultiplied alpha data.</returns>
        public static RgbaColor FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new RgbaColor(r * a / 255, g * a / 255, b * a / 255, a);
        }

        #region IEquatable<Color> Members
	
	/// <summary>
        /// Compares whether current instance is equal to specified <see cref="RgbaColor"/>.
        /// </summary>
        /// <param name="other">The <see cref="RgbaColor"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(RgbaColor other)
        {
	    return this.PackedValue == other.PackedValue;
        }

        #endregion

        /// <summary>
        /// Deconstruction method for <see cref="RgbaColor"/>.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public void Deconstruct(out byte r, out byte g, out byte b)
        {
            r = R;
            g = G;
            b = B;
        }

        /// <summary>
        /// Deconstruction method for <see cref="RgbaColor"/>.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        public void Deconstruct(out float r, out float g, out float b)
        {
            r = R / 255f;
            g = G / 255f;
            b = B / 255f;
        }

        /// <summary>
        /// Deconstruction method for <see cref="RgbaColor"/> with Alpha.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        /// <param name="a">Alpha component value from 0 to 255.</param>
        public void Deconstruct(out byte r, out byte g, out byte b, out byte a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }

        /// <summary>
        /// Deconstruction method for <see cref="RgbaColor"/> with Alpha.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        /// <param name="a">Alpha component value from 0.0f to 1.0f.</param>
        public void Deconstruct(out float r, out float g, out float b, out float a)
        {
            r = R / 255f;
            g = G / 255f;
            b = B / 255f;
            a = A / 255f;
        }
    }
}