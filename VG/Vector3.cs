using System;
using System.Runtime.InteropServices;
namespace PiGameSharp.VG
{
	[StructLayout(LayoutKind.Explicit, Pack=0, Size=12)]
	public struct Vector3
	{
		[FieldOffset(0)]
		public readonly float x;
		[FieldOffset(4)]
		public readonly float y;
		[FieldOffset(8)]
		public readonly float w;

		public static Vector3 Zero = new Vector3();
		public static Vector3 UnitX = new Vector3(1.0f, 0, 0);
		public static Vector3 UnitY = new Vector3(0, 1.0f, 0);
		public static Vector3 UnitW = new Vector3(0, 0, 1.0f);

		public Vector3(float x, float y) : this (x, y, 1.0f)
		{
		}

		public Vector3(float x, float y, float w)
		{
			this.x = x;
			this.y = y;
			this.w = w;
		}

		public static Vector3 operator -(Vector3 single)
		{
			return new Vector3(-single.x, -single.y, -single.w);
		}

		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
			return new Vector3(left.x + right.x, left.y + right.y, left.w + right.w);
		}

		public static Vector3 operator +(Vector3 left, float right)
		{
			return new Vector3(left.x + right, left.y + right, left.w + right);
		}

		public static Vector3 operator +(float left, Vector3 right)
		{
			return new Vector3(left + right.x, left + right.y, left + right.w);
		}

		public static Vector3 operator -(Vector3 left, Vector3 right)
		{
			return new Vector3(left.x - right.x, left.y - right.y, left.w - right.w);
		}

		public static Vector3 operator -(Vector3 left, float right)
		{
			return new Vector3(left.x - right, left.y - right, left.w - right);
		}

		public static Vector3 operator -(float left, Vector3 right)
		{
			return new Vector3(left - right.x, left - right.y, left - right.w);
		}

		public static float operator *(Vector3 left, Vector3 right)
		{
			return left.x*right.x + left.y*right.y + left.w*right.w;
		}

		public static Vector3 operator *(Vector3 left, float right)
		{
			return new Vector3(left.x * right, left.y * right, left.w * right);
		}

		public static Vector3 operator *(float left, Vector3 right)
		{
			return new Vector3(left * right.x, left * right.y, left * right.w);
		}

		public Vector3 Project(Vector3 to)
		{
			return to * ((this * to) / to.LengthSq);
		}

		public Vector3 Normalize()
		{
			return this * (1.0f/Length);
		}

		public float LengthSq
		{
			get
			{
				return this * this;
			}
		}

		public float Length
		{
			get
			{
				return (float)Math.Sqrt(LengthSq);
			}
		}

		public override string ToString()
		{
			return "[" + x + ", " + y + ", " + w + "]";
		}
	}
}
