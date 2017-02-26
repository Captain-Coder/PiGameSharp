using System;
namespace PiGameSharp.VG
{
	public class Image : RenderGraph, IDisposable
	{
		private Handle handle;
		private Vector2 size;
		private ImageFormat format;

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

		public Vector2 Size
		{
			get
			{
				return size;
			}
		}

		public void SetImageData(byte[] data, int scanlinestride, Rect destination)
		{
			if (handle == null)
				throw new ObjectDisposedException("Some Image");

			int lastbyte = (scanlinestride * ((int)destination.size.y - 1)) + (int)destination.size.x;
			if (data == null || lastbyte < 0 || data.Length < lastbyte)
				throw new ArgumentException("Expected " + lastbyte + " bytes, got " + (data==null?"null":data.Length.ToString()));
			VG.vgImageSubData(handle, data, scanlinestride, this.format, destination.pos.x, destination.pos.y, destination.size.x, destination.size.y);
			ErrorCode err;
			if ((err = VG.vgGetError()) != ErrorCode.NoError)
				throw new Exception("Set image data failed because " + err);
		}

		public override void Draw()
		{
			base.Draw();
			ErrorCode err;
			if (handle == null)
				throw new ObjectDisposedException("Some Image");
			VG.vgSet(Parameter.MATRIX_MODE, (int)MatrixMode.ImageUserToSurface);
			if ((err = VG.vgGetError()) != ErrorCode.NoError)
				throw new Exception("Set Matrix Mode failed because " + err);
			VG.vgLoadMatrix(ref this.CompoundedTransform);
			if ((err = VG.vgGetError()) != ErrorCode.NoError)
				throw new Exception("Load Matrix failed because " + err);
			VG.vgDrawImage(handle);
			if ((err = VG.vgGetError()) != ErrorCode.NoError)
				throw new Exception("Draw image failed because " + err);
		}

		public void DumpParameters()
		{
			Console.WriteLine("Parameters for Image " + ((IntPtr)handle).ToString("X"));
			VG.DumpContext(handle);
		}

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
