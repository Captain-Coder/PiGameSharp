using System;
using System.Runtime.InteropServices;
using PiGameSharp.Dispmanx;

namespace PiGameSharp.EGL
{
	public static class EGL
	{
#if !WINDOWS
		[DllImport("libbrcmEGL.so")] private static extern IntPtr eglGetDisplay(IntPtr native_display);
		[DllImport("libbrcmEGL.so")] private static extern bool eglInitialize(IntPtr display, IntPtr major, IntPtr minor);	// int *major, *minor; can't define them as ref since null's can't be passed with ref keyword
		[DllImport("libbrcmEGL.so")] private static extern bool eglBindAPI(EglApi api);
		[DllImport("libbrcmEGL.so")] private static extern bool eglChooseConfig(IntPtr display, uint[] attrib_list, ref IntPtr config, uint size, ref uint num_config);
		[DllImport("libbrcmEGL.so")] private static extern IntPtr eglCreateContext(IntPtr display, IntPtr config, IntPtr share_context, IntPtr attrib_list);
		[DllImport("libbrcmEGL.so")] private static extern IntPtr eglCreateWindowSurface(IntPtr display, IntPtr config, IntPtr native_window, IntPtr attrib_list);
		[DllImport("libbrcmEGL.so")] private static extern bool eglMakeCurrent(IntPtr display, IntPtr draw_surface, IntPtr read_surface, IntPtr context);
		[DllImport("libbrcmEGL.so")] private static extern bool eglSwapBuffers(IntPtr display, IntPtr surface);
		[DllImport("libbrcmEGL.so")] private static extern bool eglDestroySurface(IntPtr display, IntPtr surface);
		[DllImport("libbrcmEGL.so")] private static extern bool eglDestroyContext(IntPtr display, IntPtr context);
		[DllImport("libbrcmEGL.so")] private static extern bool eglTerminate(IntPtr display);
		[DllImport("libbrcmEGL.so")] private static extern EglError eglGetError();
#else
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern IntPtr eglGetDisplay(IntPtr native_display);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern bool eglInitialize(IntPtr display, IntPtr major, IntPtr minor);	// int *major, *minor; can't define them as ref since null's can't be passed with ref keyword
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern bool eglBindAPI(EglApi api);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern bool eglChooseConfig(IntPtr display, uint[] attrib_list, ref IntPtr config, uint size, ref uint num_config);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern IntPtr eglCreateContext(IntPtr display, IntPtr config, IntPtr share_context, IntPtr attrib_list);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern IntPtr eglCreateWindowSurface(IntPtr display, IntPtr config, IntPtr native_window, IntPtr attrib_list);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern bool eglMakeCurrent(IntPtr display, IntPtr draw_surface, IntPtr read_surface, IntPtr context);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern bool eglSwapBuffers(IntPtr display, IntPtr surface);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern bool eglDestroySurface(IntPtr display, IntPtr surface);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern bool eglDestroyContext(IntPtr display, IntPtr context);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern bool eglTerminate(IntPtr display);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern EglError eglGetError();

		[DllImport("user32.dll")] static extern IntPtr GetDC(IntPtr hwnd);
		[DllImport("user32.dll")] static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
#endif

		private static Handle display;
		private static IntPtr config;
		private static Handle context;
		private static Handle windowsurface;

#if WINDOWS
		public static void Init(IntPtr hwnd, EglApi bind)
		{
			if (display != IntPtr.Zero)
				return;

			IntPtr d;
			Handle hdc = new Handle("Windows Device Context", GetDC(hwnd), delegate(IntPtr h)
				{
					ReleaseDC(hwnd, h);
				});

#else			
		public static void Init(DisplayDevice device, EglApi bind)
		{
			if (display != IntPtr.Zero)
				return;

			IntPtr d;
			IntPtr hdc = IntPtr.Zero, hwnd = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(EglDispmanxWindow)));
			Marshal.StructureToPtr(new EglDispmanxWindow() { dispmanx_element = device.Element, size = device.Mode.Size }, hwnd, false);
#endif
			display = new Handle(
				"EGL Display Connection",
				d = eglGetDisplay(hdc),
	 			delegate(IntPtr h)
				{
					if (!eglTerminate(h))
						throw new Exception("Unable to terminate display connection " + eglGetError());
				});
			if (display.IsInvalid)
				throw new Exception("Unable to get egl display " + eglGetError());
			uint config_count = 1;
			if (!eglInitialize(display, IntPtr.Zero, IntPtr.Zero))
				throw new Exception("Unable to initialize display " + eglGetError());
			if (!eglBindAPI(bind))
				throw new Exception("Unable to bind to API " + bind + ": " + eglGetError());
			
			//    EGL_LUMINANCE_SIZE,     EGL_DONT_CARE,
			
			if (!eglChooseConfig(display, new uint[] { 0x3024, 8, 0x3023, 8, 0x3022, 8, 0x3021, 8,  0x3033, 4, 0x3031, 1, 0x3040, 2, 0x3038 }, ref config, 1, ref config_count) || config_count != 1)
				throw new Exception("Unable to choose config " + eglGetError());
			context = new Handle(
				"EGL Context",
				eglCreateContext(display, config, IntPtr.Zero, IntPtr.Zero),
				delegate (IntPtr h)
				{
					if (!eglDestroyContext(d, h))
						throw new Exception("Unable to destroy context " + eglGetError());
				});
			if (context.IsInvalid)
				throw new Exception("Unable to create context " + eglGetError());
			// Important stuff! https://www.raspberrypi.org/forums/viewtopic.php?f=68&t=30261 (halleluya for this guy catching this! Would have taken me a while...)
			// Apparently the expectation of EGL is that window objects stay in place. Well that might be true for C/C++/X11/Win32 but it certainly isn't in .net world.
			// So we have to ensure we never move the struct we are gonna give to eglCreateWindowSurface, otherwise you get mayhem and madness! (belive me I checked).
			// Two options are, using a GCHandle with pinned type, or Marshal heap allocation. Where the latter is designed for this situation, and GCHandle involves
			// dealing with valuetype boxing effects.
			windowsurface = new Handle(
				"EGL Window Surface",
				eglCreateWindowSurface(display, config, hwnd, IntPtr.Zero),
				delegate(IntPtr h)
				{
					if (!eglDestroySurface(d, h))
						throw new Exception("Unable to destroy window surface " + eglGetError());
#if !WINDOWS
					Marshal.FreeHGlobal(hdc);
#endif
				});
			if (windowsurface.IsInvalid)
				throw new Exception("Unable to create window surface " + eglGetError());

			if (!eglMakeCurrent(display, windowsurface, windowsurface, context))
				throw new Exception("Unable to make surface and context current " + eglGetError());

			if (bind == EglApi.OpenVG)
			{
#if !WINDOWS
				PiGameSharp.VG.VG.RenderSize = device.Mode.Size;
#endif
				//this apparently forces egl to reevaluate the sizes of the color buffers
				Swap();
			}
		}

		public static void Deinit()
		{
			if (display == null || display == IntPtr.Zero)
				return;

			if (!eglMakeCurrent(display, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
				throw new Exception("Unable to unset surface and context from current state " + eglGetError());
			if (windowsurface != null)
				windowsurface.Close();
			if (context != null)
				context.Close();
			display.Close();
			windowsurface = context = display = null;
		}

		internal static void Swap()
		{
			if (!eglSwapBuffers(display, windowsurface))
				throw new Exception("Unable to swap buffers " + eglGetError());
		}
	}
}
