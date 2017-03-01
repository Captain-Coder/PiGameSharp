using System;
using System.Runtime.InteropServices;

namespace PiGameSharp.EGL
{
	/// <summary>
	/// Native window used when passing a Dispmanx Element to EGL
	/// </summary>
	/// <remarks>
	/// When using dispmanx a referrence to this struct is passed to EGL as a pointer to the native window.
	/// It contains the size of the dispmanx element, since dispmanx offers no query method to obtain this size.
	/// Be sure to pin the referrence passed to EGL, since it expects this pointer to persist.
	/// </remarks>
	[StructLayout(LayoutKind.Explicit)]
	internal struct EglDispmanxWindow
	{
		[FieldOffset(0)]
		public IntPtr dispmanx_element;
		[FieldOffset(4)]
		public Vector2 size;
	}
}
