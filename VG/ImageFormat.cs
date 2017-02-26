using System;
namespace PiGameSharp.VG
{
	public enum ImageFormat : uint
	{
		Rgbx8888          =  0,
		Rgba8888          =  1,
		Rgba8888Pre       =  2,
		Rgb565            =  3,
		Rgba5551          =  4,
		Rgba4444          =  5,
		Grayscale1        = 12,
		Grayscale8        =  6,
		LinearRgbx8888    =  7,
		LinearRgba8888    =  8,
		LinearRgba8888Pre =  9,
		LinearGrayscale8  = 10,
		AlphaOnly1        = 13,
		AlphaOnly4        = 14,
		AlphaOnly8        = 11,
		
		FormatMask        = 0x0f,
		AlphaLast         = 0,
		AlphaFirst        = 1 << 6,
		ColorOrderBgr     = (1 << 7)
	};

	//contains extension methods for the ImageFormat enumeration
	public static class ImageFormatQuery
	{
		public static bool IsColored(this ImageFormat fmt)
		{
			return (uint)(fmt & ImageFormat.FormatMask) < 9 && (fmt & ImageFormat.FormatMask) != ImageFormat.Grayscale8;
		}

		public static bool HasAlpha(this ImageFormat fmt)
		{
			return (uint)(fmt & ImageFormat.FormatMask) < 10 && fmt != ImageFormat.Rgb565 && fmt != ImageFormat.Grayscale8 || fmt == ImageFormat.AlphaOnly1 || fmt == ImageFormat.AlphaOnly4 || fmt == ImageFormat.AlphaOnly8;
		}

		public static bool IsValid(this ImageFormat fmt)
		{
			return ((fmt & ImageFormat.ColorOrderBgr) != ImageFormat.ColorOrderBgr || !fmt.IsColored()) && ((fmt & ImageFormat.AlphaFirst) != ImageFormat.AlphaFirst || !fmt.IsColored() || !fmt.HasAlpha());
		}

		public static uint BitsPerSample(this ImageFormat fmt)
		{
			switch (fmt & ImageFormat.FormatMask)
			{
				case ImageFormat.Rgbx8888:
				case ImageFormat.Rgba8888:
				case ImageFormat.Rgba8888Pre:
				case ImageFormat.LinearRgbx8888:
				case ImageFormat.LinearRgba8888:
				case ImageFormat.LinearRgba8888Pre:
					return 32;

				case ImageFormat.Rgb565:
				case ImageFormat.Rgba5551:
				case ImageFormat.Rgba4444:
					return 16;
	
				case ImageFormat.AlphaOnly8:
				case ImageFormat.Grayscale8:
				case ImageFormat.LinearGrayscale8:
					return 8;

				case ImageFormat.AlphaOnly4:
					return 4;

				case ImageFormat.AlphaOnly1:
				case ImageFormat.Grayscale1:
					return 1;
	
				default:
					//(fmt & ImageFormat.FormatMask) == (uint)15
					throw new ArgumentException("Invalid format");
			}
		}
	}
}
