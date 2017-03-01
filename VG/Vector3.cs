using System;
using System.Runtime.InteropServices;

namespace PiGameSharp.VG
{
	/// <summary>
	/// A three dimensional vector
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Pack=0, Size=12)]
	public struct Vector3
	{
		/// <summary>
		/// The x element
		/// </summary>
		[FieldOffset(0)]
		public readonly float x;
		/// <summary>
		/// The y element
		/// </summary>
		[FieldOffset(4)]
		public readonly float y;
		/// <summary>
		/// The z element
		/// </summary>
		[FieldOffset(8)]
		public readonly float z;

		/// <summary>
		/// The zero vector
		/// </summary>
		public static Vector3 Zero = new Vector3();

		/// <summary>
		/// The unit vector for the X axis
		/// </summary>
		public static Vector3 UnitX = new Vector3(1.0f, 0, 0);
		/// <summary>
		/// The unit vector for the Y axis
		/// </summary>
		public static Vector3 UnitY = new Vector3(0, 1.0f, 0);
		/// <summary>
		/// The unit vector for the Z axis
		/// </summary>
		public static Vector3 UnitZ = new Vector3(0, 0, 1.0f);

		/// <summary>
		/// Initializes a new instance of the <see cref="PiGameSharp.VG.Vector3"/> struct with x and y coordinates and z = 1.0
		/// </summary>
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		public Vector3(float x, float y) : this (x, y, 1.0f)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PiGameSharp.VG.Vector3"/> struct with x, y and z coordinates
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		/// <summary>
		/// Negates all elements of the vector. (or scales with -1.0, or flipping the vector)
		/// </summary>
		public static Vector3 operator -(Vector3 single)
		{
			return new Vector3(-single.x, -single.y, -single.z);
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
			return new Vector3(left.x + right.x, left.y + right.y, left.z + right.z);
		}

		/// <summary>
		/// Adds a scalar to each element of the vector
		/// </summary>
		public static Vector3 operator +(Vector3 left, float right)
		{
			return new Vector3(left.x + right, left.y + right, left.z + right);
		}

		/// <summary>
		/// Adds a scalar to each element of the vector
		/// </summary>
		public static Vector3 operator +(float left, Vector3 right)
		{
			return new Vector3(left + right.x, left + right.y, left + right.z);
		}

		/// <summary>
		/// Subtracts two vectors
		/// </summary>
		public static Vector3 operator -(Vector3 left, Vector3 right)
		{
			return new Vector3(left.x - right.x, left.y - right.y, left.z - right.z);
		}

		/// <summary>
		/// Subtracts a scalar from each element of the vector
		/// </summary>
		public static Vector3 operator -(Vector3 left, float right)
		{
			return new Vector3(left.x - right, left.y - right, left.z - right);
		}

		/// <summary>
		/// Subtracts each element of the vector from a scalar
		/// </summary>
		public static Vector3 operator -(float left, Vector3 right)
		{
			return new Vector3(left - right.x, left - right.y, left - right.z);
		}

		/// <summary>
		/// Dot product of two vectors
		/// </summary>
		/// <remarks>The dot product equals |left|*|right|*cos(left, right)</remarks>
		public static float operator *(Vector3 left, Vector3 right)
		{
			return left.x*right.x + left.y*right.y + left.z*right.z;
		}

		/// <summary>
		/// Multiplies each element of the vector with a scalar
		/// </summary>
		public static Vector3 operator *(Vector3 left, float right)
		{
			return new Vector3(left.x * right, left.y * right, left.z * right);
		}

		/// <summary>
		/// Multiplies each element of the vector with a scalar
		/// </summary>
		public static Vector3 operator *(float left, Vector3 right)
		{
			return new Vector3(left * right.x, left * right.y, left * right.z);
		}

		/// <summary>
		/// Projects this vector onto another
		/// </summary>
		/// <param name="to">The vector to project to</param>
		public Vector3 Project(Vector3 to)
		{
			return to * ((this * to) / to.LengthSq);
		}

		/// <summary>
		/// Scales this vector such that it's magnitude becomes 1.0
		/// </summary>
		public Vector3 Normalize()
		{
			return this * (1.0f/Length);
		}

		/// <summary>
		/// Gets the square of the length (or magnitude) of this vector
		/// </summary>
		/// <value>The length squared</value>
		public float LengthSq
		{
			get
			{
				return this * this;
			}
		}

		/// <summary>
		/// Gets the length (or magnitude) of this vector
		/// </summary>
		/// <value>The length</value>
		public float Length
		{
			get
			{
				return (float)Math.Sqrt(LengthSq);
			}
		}

		public override string ToString()
		{
			return "[" + x + ", " + y + ", " + z + "]";
		}
	}
}
