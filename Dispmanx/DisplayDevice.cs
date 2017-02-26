using System;
using System.Threading;

namespace PiGameSharp.Dispmanx
{
	public class DisplayDevice
	{
		public readonly uint Id;
		public readonly ModeInfo Mode;
		private Handle handle;
		private Handle element;

		public DisplayDevice(uint id, ModeInfo mode)
		{
			Id = id;
			Mode = mode;
		}

		public override string ToString()
		{
			return "Display Device 0x" + Id.ToString("X");
		}

		internal Handle DisplayHandle
		{
			get
			{
				if (handle != IntPtr.Zero)
					return handle;

				handle = new Handle(
					"Dispmanx Display",
					BcmHost.vc_dispmanx_display_open(Id),
					delegate(IntPtr h)
					{
						if (BcmHost.vc_dispmanx_display_close(h) < 0)
							throw new Exception("Unable to close display");
					});
				if (handle == IntPtr.Zero)
					throw new Exception("Unable to open display");
				return handle;
			}
		}

		public void CloseHandle()
		{
			Handle h = Interlocked.Exchange<Handle>(ref handle, null);
			if (h != null)
				h.Close();
		}

		public IntPtr Element
		{
			get
			{
				if (element != IntPtr.Zero)
					return element;

				Rect src = new Rect(Vector2.Zero, Mode.Size) << 16, dest = new Rect (Vector2.Zero, Mode.Size);
				element = new Handle(
					"Dispmanx Element",
					BcmHost.vc_dispmanx_element_add(BcmHost.Update, DisplayHandle, 1, ref dest, IntPtr.Zero, ref src, 0, IntPtr.Zero, IntPtr.Zero, Transform.NoTransform),
					delegate(IntPtr h)
					{
						if (BcmHost.vc_dispmanx_element_remove(BcmHost.Update, h) < 0)
							throw new Exception("Unable to remove element");
						BcmHost.DoUpdate();
					});
				BcmHost.vc_dispmanx_display_set_background(BcmHost.Update, DisplayHandle, 0, 0, 0);
				BcmHost.DoUpdate();

				return element;
			}
		}

		public void CloseElement()
		{
			Handle h = Interlocked.Exchange<Handle>(ref element, null);
			if (h != null)
				h.Close();
		}

		public void SetBackground(byte r, byte g, byte b)
		{
			BcmHost.vc_dispmanx_display_set_background(BcmHost.Update, DisplayHandle, r, g, b);
			BcmHost.DoUpdate();
		}
	}
}
