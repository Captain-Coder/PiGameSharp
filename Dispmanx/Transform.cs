using System;

namespace PiGameSharp.Dispmanx
{
	[Flags]
	public enum Transform : uint
	{
		NoTransform = 0,

		/* Bottom 2 bits sets the orientation */
		RotateNone = 0,
		Rotate90 = 1,
		Rotate180 = 2,
		Rotate270 = 3,
		RotateMask = 0x3,

		FlipHorizontal = 1 << 16,
		FlipVertical = 1 << 17,

		/* invert left/right images */
		StarioscopicInvert =  1 << 19,
		/* extra flags for controlling 3d duplication behaviour */
		StarioscopicNone   =  0 << 20,
		StarioscopicMono   =  1 << 20,
		StarioscopicSBS    =  2 << 20,
		StarioscopicTB     =  3 << 20,
		StarioscopicMask   = 15 << 20,

		/* extra flags for controlling snapshot behaviour */
		SnapshotNoYUV = 1 << 24,
		SnapshotNoRGB = 1 << 25,
		SnapshotFill = 1 << 26,
		SnapshotSwapRedBlue = 1 << 27,
		SnapshotPack = 1 << 28
	}
}
