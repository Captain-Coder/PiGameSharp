namespace PiGameSharp.EGL
{
	public enum EglError : uint
	{
		Success            = 0x3000,
		NotInitialized     = 0x3001,
		BadAccess          = 0x3002,
		BadAlloc           = 0x3003,
		BadAttribute       = 0x3004,
		BadConfig          = 0x3005,
		BadContext         = 0x3006,
		BadCurrentSurface  = 0x3007,
		BadDisplay         = 0x3008,
		BadMatch           = 0x3009,
		BadNativePixMap    = 0x300A,
		BadNativeWindow    = 0x300B,
		BadParameter       = 0x300C,
		BadSurface         = 0x300D,
		ContextLost        = 0x300E
	}
}
