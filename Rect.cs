using System.Runtime.InteropServices;

namespace PiGameSharp
{
	/// <summary>
	/// A rectangle with integer parameters
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Size=16, Pack=0)]
	public struct Rect
	{
		/// <summary>
		/// The upper left corner position of the rectangle
		/// </summary>
		[FieldOffset(0)]
		public readonly Vector2 pos;
		/// <summary>
		/// The size of the rectangle
		/// </summary>
		[FieldOffset(8)]
		public readonly Vector2 size;

		/// <summary>
		/// Initializes a new instance of the <see cref="PiGameSharp.Rect"/> struct.
		/// </summary>
		/// <param name="origin">Upper left position of the rectangle</param>
		/// <param name="size">Size of the rectangle</param>
		public Rect(Vector2 origin, Vector2 size)
		{
			this.pos = origin;
			this.size = size;
		}

		/// <summary>
		/// Shift left all integer parameters of a rectangle.
		/// </summary>
		/// <remarks>
		/// In some cases rectangles are denoted using fixed point math, for example the source rectangles in dispmanx elements.
		/// Bitshifting makes dealing with this much easier
		/// </remarks>
		/// <param name="item">The rectangle to shift left</param>
		/// <param name="bits">How many bits to shift left</param>
		public static Rect operator << (Rect item, int bits)
		{
			return new Rect(item.pos << bits, item.size << bits);
		}

		/// <summary>
		/// Shift right all integer parameters of a rectangle.
		/// </summary>
		/// <remarks>
		/// In some cases rectangles are denoted using fixed point math, for example the source rectangles in dispmanx elements.
		/// Bitshifting makes dealing with this much easier
		/// </remarks>
		/// <param name="item">The rectangle to shift right</param>
		/// <param name="bits">How many bits to shift right</param>
		public static Rect operator >> (Rect item, int bits)
		{
			return new Rect(item.pos >> bits, item.size >> bits);
		}
	}
}
