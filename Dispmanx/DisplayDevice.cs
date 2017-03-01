using System;
using System.Threading;

namespace PiGameSharp.Dispmanx
{
	/// <summary>
	/// A dispmanx display for displaying things
	/// </summary>
	public sealed class DisplayDevice
	{
		/// <summary>
		/// Information about the mode of this display.
		/// </summary>
		public readonly ModeInfo Mode;

		private Handle handle;
		private Handle element;

		internal DisplayDevice(ModeInfo mode)
		{
			Mode = mode;
		}

		internal Handle DisplayHandle
		{
			get
			{
				// Has the display already been opened?
				if (handle != IntPtr.Zero)
					return handle;

				handle = new Handle(
					"Dispmanx Display",
					BcmHost.vc_dispmanx_display_open(Mode.DisplayNumber),
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

		internal void CloseDisplayHandle()
		{
			// This one can potentially be called from (critical) finalizers, thus we have to watch for
			// multiple concurrent calls on different threads and make sure we don't throw anything... ever.
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
				BcmHost.DoUpdate();

				return element;
			}
		}

		internal void CloseElement()
		{
			// This one can potentially be called from (critical) finalizers, thus we have to watch for
			// multiple concurrent calls on different threads and make sure we don't throw anything... ever.
			Handle h = Interlocked.Exchange<Handle>(ref element, null);
			if (h != null)
				h.Close();
		}

		/// <summary>
		/// Sets the background color of the display
		/// </summary>
		/// <param name="r">The red component</param>
		/// <param name="g">The green component</param>
		/// <param name="b">The blue component</param>
		public void SetBackground(byte r, byte g, byte b)
		{
			BcmHost.vc_dispmanx_display_set_background(BcmHost.Update, DisplayHandle, r, g, b);
			BcmHost.DoUpdate();
		}
	}
}
