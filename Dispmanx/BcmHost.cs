using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace PiGameSharp.Dispmanx
{
    public static class BcmHost
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
		[DllImport("libbrcmGLESv2.so")] private static extern int glGetError(); 

		internal static Handle UpdateHandle;
				
		public static List<DisplayDevice> Devices = null;

		public static void Init()
		{
			if (Devices != null)
				return;

			bcm_host_init();

			Devices = new List<DisplayDevice>();
			for (uint i = 0; i < 32; i++)
			{
				IntPtr handle = vc_dispmanx_display_open(i);
				ModeInfo mode = new ModeInfo();
				if (handle == IntPtr.Zero)
					continue;
				if (vc_dispmanx_display_get_info(handle, ref mode) < 0 || mode.DisplayNumber != i)
					continue;
				Devices.Add(new DisplayDevice(i, mode));
				vc_dispmanx_display_close(handle);
			}
			DoUpdate();
			
			glGetError();
			// If you are thinking WTF? then yes. At the time I made this, the libbrcmEGL.so depends on a few exports from libbrcmGLESv2.so (even though I'm going to use OpenVG.so in the end).
			// But it does not declare this as a requirement since this would be a circular import (gles depends on egl). To fix this we have to make sure to touch libbrcmGLESv2.so before
			// touching libEGL.so. So this is one of the few places where this is could be possible (there are no CLI guarantees here!). So choose an innocent function from gles and call it. 
		}

		public static void Deinit()
		{
			if (Devices == null)
				return;

			foreach (DisplayDevice dev in Devices)
			{
				dev.CloseElement();
				dev.CloseHandle();
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

		public static void DoUpdate()
		{
			Handle prev = Interlocked.Exchange<Handle>(ref UpdateHandle, null);
			if (prev != null)
				prev.Close();
		}
	}
}
