using System;
using System.Runtime.InteropServices;

namespace PiGameSharp.VG
{
	/// <summary>
	/// Represensts a 3x3 matrix, with common mathematical operators defined. Can be used as a homogeneous coordinate transformation for two dimensional space.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct Matrix
	{
		/// <summary>
		/// The vector that represents the transformed x-axis
		/// </summary>
		[FieldOffset(0)]
		public readonly Vector3 x;
		/// <summary>
		/// The vector that represents the transformed y-axis
		/// </summary>
		[FieldOffset(12)]
		public readonly Vector3 y;
		/// <summary>
		/// The vector that represents the transformed z-axis
		/// </summary>
		[FieldOffset(24)]
		public readonly Vector3 z;

		/// <summary>
		/// The zero matrix
		/// </summary>
		public static readonly Matrix Zero = new Matrix();

		/// <summary>
		/// The identity matrix
		/// </summary>
		public static readonly Matrix Identity = new Matrix(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);

		/// <summary>
		/// Initializes a new instance of the <see cref="PiGameSharp.VG.Matrix"/> struct with an x, y, and z vector.
		/// </summary>
		/// <param name="x">The x axis vector</param>
		/// <param name="y">The y axis vector</param>
		/// <param name="t">The z axis vector</param>
		public Matrix(Vector3 x, Vector3 y, Vector3 z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		/// <summary>
		/// Inverts the matrix
		/// </summary>
		/// <param name="single">The matrix to invert</param>
		public static Matrix operator ~(Matrix single)
		{
			float inv = 1.0f/single.Determinant;
			return new Matrix(
				new Vector3(inv * (single.y.y*single.z.z - single.y.z*single.z.y), inv * (single.y.z*single.z.x - single.y.x*single.z.z), inv * (single.y.x*single.z.y - single.y.y*single.z.x)),
				new Vector3(inv * (single.x.z*single.z.y - single.x.y*single.z.z), inv * (single.x.x*single.z.z - single.x.z*single.z.x), inv * (single.x.y*single.z.x - single.x.x*single.z.y)),
				new Vector3(inv * (single.x.y*single.y.z - single.x.z*single.y.y), inv * (single.x.z*single.y.x - single.x.x*single.y.z), inv * (single.x.x*single.y.y - single.x.y*single.y.x)));
		}

		/// <summary>
		/// Adds two matrices
		/// </summary>
		public static Matrix operator +(Matrix left, Matrix right)
		{
			return new Matrix(left.x + right.x,left.y + right.y, left.z + right.z);
		}

		/// <summary>
		/// Adds a constant to all elements of the matrix
		/// </summary>
		public static Matrix operator +(float left, Matrix right)
		{
			return new Matrix(left + right.x, left + right.y, left + right.z);
		}

		/// <summary>
		/// Adds a constant to all elements of the matrix
		/// </summary>
		public static Matrix operator +(Matrix left, float right)
		{
			return new Matrix(left.x + right, left.y + right, left.z + right);
		}

		/// <summary>
		/// Subtracts two matrices
		/// </summary>
		public static Matrix operator -(Matrix left, Matrix right)
		{
			return new Matrix(left.x - right.x, left.y - right.y, left.z - right.z);
		}

		/// <summary>
		/// Subtracts the matrix elements from a constant
		/// </summary>
		public static Matrix operator -(float left, Matrix right)
		{
			return new Matrix(left - right.x, left - right.y, left - right.z);
		}

		/// <summary>
		/// Subtracts a constant from all elements of the matrix
		/// </summary>
		public static Matrix operator -(Matrix left, float right)
		{
			return new Matrix(left.x - right, left.y - right, left.z - right);
		}

		/// <summary>
		/// Multiplies all elements of the matrix with a scalar value
		/// </summary>
		public static Matrix operator *(float left, Matrix right)
		{
			return new Matrix(left * right.x, left * right.y, left * right.z);
		}
		
		/// <summary>
		/// Multiplies all elements of the matrix with a scalar value
		/// </summary>
		public static Matrix operator *(Matrix left, float right)
		{
			return new Matrix(left.x * right, left.y * right, left.z * right);
		}

		/// <summary>
		/// Multiplies a vector with a matrix, leading to a transformed vector.
		/// </summary>
		public static Vector3 operator *(Matrix left, Vector3 right)
		{
			return new Vector3(
				left.x.x * right.x + left.y.x * right.y + left.z.x * right.z,
				left.x.y * right.x + left.y.y * right.y + left.z.y * right.z,
				left.x.z * right.x + left.y.z * right.y + left.z.z * right.z);
		}

		/// <summary>
		/// Multiplies two matrices
		/// </summary>
		public static Matrix operator *(Matrix left, Matrix right)
		{
			return new Matrix(
				new Vector3(
					left.x.x*right.x.x + left.y.x*right.x.y + left.z.x*right.x.z,
					left.x.y*right.x.x + left.y.y*right.x.y + left.z.y*right.x.z,
					left.x.z*right.x.x + left.y.z*right.x.y + left.z.z*right.x.z),
				new Vector3(
					left.x.x*right.y.x + left.y.x*right.y.y + left.z.x*right.y.z,
					left.x.y*right.y.x + left.y.y*right.y.y + left.z.y*right.y.z,
					left.x.z*right.y.x + left.y.z*right.y.y + left.z.z*right.y.z),
				new Vector3(
					left.x.x*right.z.x + left.y.x*right.z.y + left.z.x*right.z.z,
					left.x.y*right.z.x + left.y.y*right.z.y + left.z.y*right.z.z,
					left.x.z*right.z.x + left.y.z*right.z.y + left.z.z*right.z.z));
		}

		/// <summary>
		/// Creates an transformation matrix which translates the point of origin
		/// </summary>
		/// <param name="v">Vector specifying the translation to apply</param>
		public static Matrix Translation(Vector3 v) => new Matrix(Vector3.UnitX, Vector3.UnitY, new Vector3(v.x, v.y, 1.0f));

		/// <summary>
		/// Creates an transformation matrix which scales around the point of origin
		/// </summary>
		/// <param name="factor">Scalar value indicating the scaling to apply</param>
		public static Matrix Scale(float factor) => new Matrix(Vector3.UnitX * factor, Vector3.UnitY * factor, Vector3.UnitZ);

		/// <summary>
		/// Creates an transformation matrix which rotates around the point of origin
		/// </summary>
		/// <param name="radians">The rotation to perform expressed in radians</param>
		public static Matrix Rotation(float radians)
		{
			float sin = (float)Math.Sin(radians), cos = (float)Math.Cos(radians);
			return new Matrix(new Vector3(cos, sin, 0), new Vector3(-sin, cos, 0), Vector3.UnitZ);
		}

		/// <summary>
		/// Gets the determinant of the matrix.
		/// </summary>
		/// <value>The determinant of the matrix</value>
		public float Determinant
		{
			get
			{
				return x.x * y.y * z.z + x.y * y.z * z.x + x.z * y.x * z.y - x.x * y.z * z.y - x.y * y.x * z.z - x.z * y.y * z.x;
			}
		}

		/// <summary>
		/// Transposes the matrix
		/// </summary>
		public Matrix Transpose()
		{
			return new Matrix(
				new Vector3(x.x, y.x, z.x),
				new Vector3(x.y, y.y, z.y),
				new Vector3(x.z, y.z, z.z));
		}

		public override string ToString() => "[" + x + ", " + y + ", " + z + "]";
	}
}
