namespace PiGameSharp.VG
{
	public enum ImageRenderQuality : uint
	{
		NonAntialiased  = (1 << 0),
		// Harder
		Better          = (1 << 2),
		Faster          = (1 << 1)
		// Stronger? :P
	};
}
