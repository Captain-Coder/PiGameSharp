using System.Runtime.InteropServices;

namespace PiGameSharp
{
	[StructLayout(LayoutKind.Explicit, Size=8, Pack=0)]
	public struct Vector2
	{
		public readonly static Vector2 Zero = new Vector2();
		public readonly static Vector2 UnitX = new Vector2(1, 0);
		public readonly static Vector2 UnitY = new Vector2(0, 1);

		[FieldOffset(0)]
		public readonly uint x;
		[FieldOffset(4)]
		public readonly uint y;

		public Vector2(uint x, uint y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return "[" + x + ", " + y + "]";
		}

		public static Vector2 operator << (Vector2 item, int bits)
		{
			return new Vector2(item.x << bits, item.y << bits);
		}

		public static Vector2 operator >> (Vector2 item, int bits)
		{
			return new Vector2(item.x >> bits, item.y >> bits);
		}
	}
}
