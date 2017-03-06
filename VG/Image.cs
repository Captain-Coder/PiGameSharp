using System;
using System.Diagnostics;

namespace PiGameSharp.VG
{
	/// <summary>
	/// An OpenVG image
	/// </summary>
	public class Image : RenderNode, IDisposable
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
		private Vector2 size;
		private ImageFormat format;

		/// <summary>
		/// Initializes a new <see cref="PiGameSharp.VG.Image"/> with format, size and optinally data
		/// </summary>
		/// <param name="format">The pixel format to use for this image</param>
		/// <param name="size">The size of the image</param>
		/// <param name="data">The data with which to initialize this image, or null to keep the image unfilled</param>
		/// <param name="scanlinestride">Offset for consecutive scan lines of the image in data.</param>
		public Image(ImageFormat format, Vector2 size, byte[] data, int scanlinestride)
		{
			IntPtr hnd = VG.vgCreateImage(format, (int)size.x, (int)size.y, ImageRenderQuality.Better | ImageRenderQuality.Faster | ImageRenderQuality.NonAntialiased);
			if (hnd == IntPtr.Zero)
				throw new Exception("Error creating Image " + VG.vgGetError());
			handle = new Handle("OpenVG Image", hnd, delegate(IntPtr h)
				{
					VG.vgDestroyImage(h);
				});
			this.size = size;
			this.format = format;

			if (data != null)
				SetImageData(data, scanlinestride, new Rect(Vector2.Zero, size));
		}

		/// <summary>
		/// Gets the size of the image
		/// </summary>
		/// <value>The size vector</value>
		public Vector2 Size
		{
			get
			{
				return size;
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
				throw new ArgumentException("Expected " + lastbyte + " bytes, got " + (data==null?"null":data.Length.ToString()));
			VG.vgImageSubData(handle, data, scanlinestride, this.format, destination.pos.x, destination.pos.y, destination.size.x, destination.size.y);
			VG.DetectError("Set image data failed");
		}

		/// <summary>
		/// Draw this image to the current OpenVG drawing surface.
		/// </summary>
		public override void Draw()
		{
			base.Draw();
			if (handle == null)
				throw new ObjectDisposedException("Some Image");
			Matrix wt = this.WorldTransform;
			VG.vgSet(Parameter.MATRIX_MODE, (int)MatrixMode.ImageUserToSurface);
			VG.DetectError("Set Matrix Mode failed");
			VG.vgLoadMatrix(ref wt);
			VG.DetectError("Load Matrix Mode failed");
			VG.vgDrawImage(handle);
			VG.DetectError("Draw Image failed");
			draws++;
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

		/// <summary>
		/// Releases all resource used by the <see cref="PiGameSharp.VG.Image"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="PiGameSharp.VG.Image"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="PiGameSharp.VG.Image"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="PiGameSharp.VG.Image"/> so the garbage
		/// collector can reclaim the memory that the <see cref="PiGameSharp.VG.Image"/> was occupying.</remarks>
		public void Dispose()
		{
			if (handle != null)
			{
				handle.Close();
				handle = null;
			}
		}
	}
}
