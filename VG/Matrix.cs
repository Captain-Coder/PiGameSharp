using System;
using System.Runtime.InteropServices;
namespace PiGameSharp.VG
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Matrix
	{
		[FieldOffset(0)]
		public readonly Vector3 x;
		[FieldOffset(12)]
		public readonly Vector3 y;
		[FieldOffset(24)]
		public readonly Vector3 t;

		public static readonly Matrix Identity = new Matrix(Vector3.UnitX, Vector3.UnitY, Vector3.UnitW);

		public Matrix(Vector3 x, Vector3 y, Vector3 t)
		{
			this.x = x;
			this.y = y;
			this.t = t;
		}

		public static Matrix operator ~(Matrix single)
		{
			float inv = 1.0f/single.Determinant();
			return new Matrix(
				new Vector3(inv * (single.y.y*single.t.w - single.y.w*single.t.y), inv * (single.y.w*single.t.x - single.y.x*single.t.w), inv * (single.y.x*single.t.y - single.y.y*single.t.x)),
				new Vector3(inv * (single.x.w*single.t.y - single.x.y*single.t.w), inv * (single.x.x*single.t.w - single.x.w*single.t.x), inv * (single.x.y*single.t.x - single.x.x*single.t.y)),
				new Vector3(inv * (single.x.y*single.y.w - single.x.w*single.y.y), inv * (single.x.w*single.y.x - single.x.x*single.y.w), inv * (single.x.x*single.y.y - single.x.y*single.y.x)));
		}

		public static Matrix operator + (Matrix left, Matrix right)
		{
			return new Matrix(left.x + right.x,left.y + right.y, left.t + right.t);
		}

		public static Matrix operator + (float left, Matrix right)
		{
			return new Matrix(left + right.x, left + right.y, left + right.t);
		}

		public static Matrix operator + (Matrix left, float right)
		{
			return new Matrix(left.x + right, left.y + right, left.t + right);
		}

		public static Matrix operator +(Matrix left, Vector3 right)
		{
			return new Matrix(left.x, left.y, left.t + right);
		}

		public static Matrix operator - (Matrix left, Matrix right)
		{
			return new Matrix(left.x - right.x, left.y - right.y, left.t - right.t);
		}

		public static Matrix operator - (float left, Matrix right)
		{
			return new Matrix(left - right.x, left - right.y, left - right.t);
		}

		public static Matrix operator - (Matrix left, float right)
		{
			return new Matrix(left.x - right, left.y - right, left.t - right);
		}

		public static Matrix operator *(float left, Matrix right)
		{
			return new Matrix(left * right.x, left * right.y, left * right.t);
		}
		
		public static Matrix operator *(Matrix left, float right)
		{
			return new Matrix(left.x * right, left.y * right, left.t * right);
		}

		public static Vector3 operator *(Matrix left, Vector3 right)
		{
			return new Vector3(
				left.x.x * right.x + left.y.x * right.y + left.t.x * right.w,
				left.x.y * right.x + left.y.y * right.y + left.t.y * right.w,
				left.x.w * right.x + left.y.w * right.y + left.t.w * right.w);
		}

		public static Matrix operator * (Matrix left, Matrix right)
		{
			return new Matrix(
				new Vector3(
					left.x.x*right.x.x + left.y.x*right.x.y + left.t.x*right.x.w,
					left.x.y*right.x.x + left.y.y*right.x.y + left.t.y*right.x.w,
					left.x.w*right.x.x + left.y.w*right.x.y + left.t.w*right.x.w),
				new Vector3(
					left.x.x*right.y.x + left.y.x*right.y.y + left.t.x*right.y.w,
					left.x.y*right.y.x + left.y.y*right.y.y + left.t.y*right.y.w,
					left.x.w*right.y.x + left.y.w*right.y.y + left.t.w*right.y.w),
				new Vector3(
					left.x.x*right.t.x + left.y.x*right.t.y + left.t.x*right.t.w,
					left.x.y*right.t.x + left.y.y*right.t.y + left.t.y*right.t.w,
					left.x.w*right.t.x + left.y.w*right.t.y + left.t.w*right.t.w));
		}

		public static Matrix Translation(Vector3 v)
		{
			return new Matrix(Vector3.UnitX, Vector3.UnitY, new Vector3(v.x, v.y, 1.0f));
		}

		public static Matrix Scale(float factor)
		{
			return new Matrix(Vector3.UnitX * factor, Vector3.UnitY * factor, Vector3.UnitW);
		}

		public static Matrix Rotation(float radians)
		{
			float sin = (float)Math.Sin(radians), cos = (float)Math.Cos(radians);
			return new Matrix(new Vector3(cos, sin, 0), new Vector3(-sin, cos, 0), Vector3.UnitW);
		}

		public float Determinant()
		{
			return x.x*y.y*t.w + x.y*y.w*t.x + x.w*y.x*t.y - x.x*y.w*t.y - x.y*y.x*t.w - x.w*y.y*t.x;
		}

		public Matrix Transpose()
		{
			return new Matrix(
				new Vector3(x.x, y.x, t.x),
				new Vector3(x.y, y.y, t.y),
				new Vector3(x.w, y.w, t.w));
		}

		public override string ToString()
		{
			return "[" + x + ", " + y + ", " + t + "]";
		}
	}
}
