using System;
using System.Diagnostics;

namespace PiGameSharp.VG
{
	/// <summary>
	/// An OpenVG image
	/// </summary>
	public class Image : Resource
	{
		/// <summary>
		/// Performance counter to measure image draw rate
		/// </summary>
		public static PerformanceCounter draws = new PerformanceCounter("Image.Draw()")
		{
			SampleInterval = TimeSpan.FromSeconds(10),
			Unit = "calls"
		};

		private Handle handle;
		private Func<byte[]> source;

		/// <summary>
		/// Initializes a new <see cref="PiGameSharp.VG.Image"/> with format, size and optinally data
		/// </summary>
		/// <param name="format">The pixel format to use for this image</param>
		/// <param name="size">The size of the image</param>
		/// <param name="datasource">A delegate that produces image data and a scanlinestride, or null to keep the image unfilled</param>
		public Image(ImageFormat format, Vector2 size, Func<byte[]> datasource)
		{
			Size = size;
			Format = format;
			source = datasource;
		}

		/// <summary>
		/// Gets the size of the image
		/// </summary>
		/// <value>The size vector</value>
		public Vector2 Size { get; }

		/// <summary>
		/// Gets the format of the image
		/// </summary>
		/// <value>The format</value>
		public ImageFormat Format { get; }

		protected override void Load()
		{
			if (handle != null)
				return;

			IntPtr hnd = VG.vgCreateImage(Format, (int)Size.x, (int)Size.y, ImageRenderQuality.Better | ImageRenderQuality.Faster);
			if (hnd == IntPtr.Zero)
				throw new Exception("Error creating Image " + VG.vgGetError());
			handle = new Handle("OpenVG Image", hnd, delegate (IntPtr h)
			{
				VG.vgDestroyImage(h);
			});

			if (source != null)
			{
				byte[] data = source();
				SetImageData(data, data.Length /  (int)Size.y, new Rect(Vector2.Zero, Size));
			}
		}

		protected override void Unload()
		{
			if (handle != null)
			{
				handle.Close();
				handle = null;
			}
		}

		/// <summary>
		/// Sets the image contents
		/// </summary>
		/// <param name="data">The pixel data</param>
		/// <param name="scanlinestride">Offset for consecutive scan lines of the image in data.</param>
		/// <param name="destination">The rectangle of the image to fill</param>
		/// <remarks>
		/// The pixels should be packed into data with the pixel format used to construct this image.
		/// There is no offset between pixels on a line. Consecutive lines are placed at scanlinestride intervals, measured
		/// from line start to line start.
		/// </remarks>
		public void SetImageData(byte[] data, int scanlinestride, Rect destination)
		{
			if (handle == null)
				throw new ObjectDisposedException("Some Image");

			int lastbyte = (scanlinestride * ((int)destination.size.y - 1)) + (int)destination.size.x;
			if (data == null || lastbyte < 0 || data.Length < lastbyte)
				throw new ArgumentException("Expected " + lastbyte + " bytes, got " + (data == null ? "null" : data.Length.ToString()));
			VG.vgImageSubData(handle, data, scanlinestride, Format, destination.pos.x, destination.pos.y, destination.size.x, destination.size.y);
			VG.DetectError("Set image data failed");
		}

		/// <summary>
		/// Draw this image to the current OpenVG drawing surface.
		/// </summary>
		public void Draw()
		{
			if (handle == null)
				throw new ObjectDisposedException("Some Image");
			VG.vgDrawImage(handle);
			VG.DetectError("Draw Image failed");
			draws.Add(1.0);
		}

		/// <summary>
		/// Dumps the OpenVG parameters of this Image
		/// </summary>
		[Conditional("DEBUG")]
		public void DumpParameters()
		{
			Console.WriteLine("Parameters for Image " + ((IntPtr)handle).ToString("X"));
			VG.DumpContext(handle);
		}
	}
}