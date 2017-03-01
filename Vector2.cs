using System.Runtime.InteropServices;

namespace PiGameSharp
{
	/// <summary>
	/// A two dimensional vector
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Size=8, Pack=0)]
	public struct Vector2
	{
		/// <summary>
		/// The zero vector
		/// </summary>
		public readonly static Vector2 Zero = new Vector2();
		/// <summary>
		/// The unit vector for the X axis
		/// </summary>
		public readonly static Vector2 UnitX = new Vector2(1, 0);
		/// <summary>
		/// The unit vector for the Y axis
		/// </summary>
		public readonly static Vector2 UnitY = new Vector2(0, 1);

		/// <summary>
		/// The x element
		/// </summary>
		[FieldOffset(0)]
		public readonly uint x;
		/// <summary>
		/// The y element
		/// </summary>
		[FieldOffset(4)]
		public readonly uint y;

		/// <summary>
		/// Initializes a new instance of the <see cref="PiGameSharp.Vector2"/> struct.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Vector2(uint x, uint y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return "[" + x + ", " + y + "]";
		}

		/// <summary>
		/// Shift left all elements of a vector
		/// </summary>
		/// <remarks>
		/// In some cases we need fixed point math vectors. Having a bit shift operator makes dealing with this much easier
		/// </remarks>
		/// <param name="item">The vector to shift left</param>
		/// <param name="bits">How many bits to shift left</param>
		public static Vector2 operator << (Vector2 item, int bits)
		{
			return new Vector2(item.x << bits, item.y << bits);
		}

		/// <summary>
		/// Shift right all elements of a vector
		/// </summary>
		/// <remarks>
		/// In some cases we need fixed point math vectors. Having a bit shift operator makes dealing with this much easier
		/// </remarks>
		/// <param name="item">The vector to shift right</param>
		/// <param name="bits">How many bits to shift right</param>
		public static Vector2 operator >> (Vector2 item, int bits)
		{
			return new Vector2(item.x >> bits, item.y >> bits);
		}
	}
}
