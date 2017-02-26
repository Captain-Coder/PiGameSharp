using System.Runtime.InteropServices;

namespace PiGameSharp.Dispmanx
{
	[StructLayout(LayoutKind.Explicit)]
	public struct ModeInfo
	{
		[FieldOffset(0)]
		public Vector2 Size;

		[FieldOffset(8)]
		public Transform Transformation;

		[FieldOffset(12)]
		public InputFormat Format;

		[FieldOffset(16)]
		public uint DisplayNumber;

		public override string ToString()
		{
			return "Display Device 0x" + DisplayNumber.ToString("X") + " [" + Size + "] transform " + Transformation + " format " + Format;
		}
	}
}
