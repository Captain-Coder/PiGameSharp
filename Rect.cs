using System.Runtime.InteropServices;

namespace PiGameSharp
{
	[StructLayout(LayoutKind.Explicit, Size=16, Pack=0)]
	public struct Rect
	{
		[FieldOffset(0)]
		public readonly Vector2 pos;
		[FieldOffset(8)]
		public readonly Vector2 size;

		public Rect(Vector2 origin, Vector2 size)
		{
			this.pos = origin;
			this.size = size;
		}

		// In some cases rectangles are denoted using fixed point math, for example the source rectables in dispmanx elements. 
		public static Rect operator << (Rect item, int bits)
		{
			return new Rect(item.pos << bits, item.size << bits);
		}

		public static Rect operator >> (Rect item, int bits)
		{
			return new Rect(item.pos >> bits, item.size >> bits);
		}
	}
}
