namespace PiGameSharp.VG
{
	public enum ErrorCode : uint
	{
		NoError = 0,
		BadHandle = 0x1000,
		IllegalArgument = 0x1001,
		OutOfMemory = 0x1002,
		PathCapability = 0x1003,
		UnsupportedImageFormat = 0x1004,
		UnsupportedPathFormat = 0x1005,
		ImageInUse = 0x1006,
		NoContext = 0x1007
	}
}
