using System.Runtime.InteropServices;

namespace PiGameSharp.Dispmanx
{
	/// <summary>
	/// Mode information for a dispmanx display
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct ModeInfo
	{
		/// <summary>
		/// The width and height of the display
		/// </summary>
		[FieldOffset(0)]
		public Vector2 Size;

		/// <summary>
		/// The <see cref="PiGameSharp.Dispmanx.Transform"/> that is applied to a display such as flips and inversions
		/// </summary>
		[FieldOffset(8)]
		public Transform Transformation;

		/// <summary>
		/// The <see cref="PiGameSharp.Dispmanx.InputFormat"/> for this display
		/// </summary>
		[FieldOffset(12)]
		public InputFormat Format;

		/// <summary>
		/// The numeric id of this display
		/// </summary>
		[FieldOffset(16)]
		public uint DisplayNumber;

		// This makes debugging less cumbersome
		override string ToString()
		{
			return "Display Device 0x" + DisplayNumber.ToString("X") + " [" + Size + "] transform " + Transformation + " format " + Format;
		}
	}
}
