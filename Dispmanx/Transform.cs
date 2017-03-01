using System;

namespace PiGameSharp.Dispmanx
{
	/// <summary>
	/// Types of transformations that can be applied to a dispmanx display or element
	/// </summary>
	[Flags]
	public enum Transform : uint
	{
		NoTransform = 0,

		RotateNone = 0,
		Rotate90 = 1,
		Rotate180 = 2,
		Rotate270 = 3,
		RotateMask = 0x3,

		FlipHorizontal = 1 << 16,
		FlipVertical = 1 << 17,

		SterioscopicSwapLeftRight =  1 << 19,
		SterioscopicNone   =  0 << 20,
		SterioscopicMono   =  1 << 20,
		SterioscopicSBS    =  2 << 20,
		SterioscopicTB     =  3 << 20,
		SterioscopicMask   = 15 << 20,

		SnapshotNoYUV = 1 << 24,
		SnapshotNoRGB = 1 << 25,
		SnapshotFill = 1 << 26,
		SnapshotSwapRedBlue = 1 << 27,
		SnapshotPack = 1 << 28
	}
}
