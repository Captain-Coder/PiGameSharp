using System;
using System.Runtime.InteropServices;

namespace PiGameSharp.EGL
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct EglDispmanxWindow
	{
		[FieldOffset(0)]
		public IntPtr dispmanx_element;
		[FieldOffset(4)]
		public Vector2 size;
	}
}
