using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace PiGameSharp.Dispmanx
{
    /// <summary>
    /// Handles the specifics of the RaspberryPi/Dispmanx
    /// </summary>
	public static sealed class BcmHost
    {
		[DllImport("libbcm_host.so")] private static extern void bcm_host_init();
		[DllImport("libbcm_host.so")] private static extern void bcm_host_deinit();

		[DllImport("libbcm_host.so")] private static extern IntPtr vc_dispmanx_update_start(int priority);
		[DllImport("libbcm_host.so")] private static extern int vc_dispmanx_update_submit_sync(IntPtr update);
		[DllImport("libbcm_host.so")] internal static extern IntPtr vc_dispmanx_display_open(uint display);
		[DllImport("libbcm_host.so")] internal static extern int vc_dispmanx_display_close(IntPtr handle);
		[DllImport("libbcm_host.so")] internal static extern int vc_dispmanx_display_get_info(IntPtr handle, ref ModeInfo currentmode);
		[DllImport("libbcm_host.so")] internal static extern IntPtr vc_dispmanx_element_add(IntPtr update, IntPtr display, int layer, ref Rect dest_rect, IntPtr src_resource, ref Rect src_rect, uint protection, IntPtr alpha, IntPtr clamp, Transform transform );
		[DllImport("libbcm_host.so")] internal static extern int vc_dispmanx_element_remove(IntPtr update, IntPtr element);
		[DllImport("libbcm_host.so")] internal static extern int vc_dispmanx_display_set_background(IntPtr update, IntPtr display, byte red, byte green, byte blue);

		// See comments in Init
		[DllImport("libGLESv2.so")] private static extern int glGetError(); 

		// The currently active dispmanx update
		internal static Handle UpdateHandle;
				
		/// <summary>
		/// The list of devices that the dispmanx implementation can use. Filled during <see cref="Init()"/>.
		/// </summary>
		public static List<DisplayDevice> Devices = null;

		/// <summary>
		/// Initializes the dispmanx features of the host.
		/// </summary>
		public static void Init()
		{
			// Has the BcmHost been initialized already?
			if (Devices != null)
				return;

			bcm_host_init();
			Devices = new List<DisplayDevice>();

			// The raspberry pi dispmanx code is customized to have only 1 display, but it can have more when the offical lcd is added. So we really  ought to check for more displays.
			// Unfortunately there is no dispmanx function to enumerate the displays, or get a count of them. So resort to testing a few and seeing what sticks.
			for (uint i = 0; i < 32; i++)
			{
				IntPtr handle = vc_dispmanx_display_open(i);
				ModeInfo mode = new ModeInfo();
				if (handle == IntPtr.Zero)
					continue;
				if (vc_dispmanx_display_get_info(handle, ref mode) < 0 || mode.DisplayNumber != i) // this checks if the dispmanx software returned the actual display number.
					continue;
				Devices.Add(new DisplayDevice(mode));
				vc_dispmanx_display_close(handle);
			}

			glGetError();
			// If you are thinking WTF? then yes. At the time I made this, the libbrcmEGL.so depends on a few exports from libbrcmGLESv2.so (even though I'm going to use OpenVG.so in the end).
			// But it does not declare this as a requirement since this would be a circular import (gles depends on egl). To fix this we have to make sure to touch libbrcmGLESv2.so before
			// touching libEGL.so. This causes them both to be loaded in the right order. But this has to happen before we touch libEGL. So this is one of the few places where this is
			// possible. And even though this works now, this depends on CLI specifics (mono in this case), so there is no guarantee this will keep working. So to force libbrcmGLESv2.so to load 
			// choose an innocent function from gles and call it. 
		}

		/// <summary>
		/// Releases all dispmanx resources (devices/updates) held by this process.
		/// </summary>
		public static void Deinit()
		{
			// Has the BcmHost never been initialized or already deinitialized?
			if (Devices == null)
				return;

			foreach (DisplayDevice dev in Devices)
			{
				dev.CloseElement();
				dev.CloseDisplayHandle();
			}
			if (UpdateHandle != null)
				DoUpdate();
			Devices = null;
			bcm_host_deinit();
		}

		internal static Handle Update
		{
			get
			{
				if (UpdateHandle == null)
					UpdateHandle = new Handle(
						"Dispmanx Update",
						vc_dispmanx_update_start(0),
						delegate(IntPtr h)
						{
							if (vc_dispmanx_update_submit_sync(h) < 0)
								throw new Exception("Unable to perform dispmanx update transaction");
						});
				return UpdateHandle;
			}
		}

		internal static void DoUpdate()
		{
			// This one can potentially be called from (critical) finalizers, thus we have to watch for
			// multiple concurrent calls on different threads and make sure we don't throw anything... ever.
			Handle prev = Interlocked.Exchange(ref UpdateHandle, null);
			if (prev != null)
				prev.Close();
		}
	}
}
