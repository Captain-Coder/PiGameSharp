using System;
using System.Runtime.InteropServices;
using PiGameSharp.Dispmanx;

namespace PiGameSharp.EGL
{
	/// <summary>
	/// Handles the specifics of managing the Embedded Graphics Library
	/// </summary>
	public static class EGL
	{
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern IntPtr eglGetDisplay(IntPtr native_display);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] private static extern bool eglInitialize(IntPtr display, IntPtr major, IntPtr minor);
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

		[DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hwnd);
		[DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

		private static Handle display;
		private static IntPtr config;
		private static Handle context;
		private static Handle windowsurface;
		private static Handle extrahandle;

		/// <summary>
		/// Performance counter to measure framerate
		/// </summary>
		public readonly static PerformanceCounter frames = new PerformanceCounter("EGL.Swap()")
			{
				Unit = "Frames",
				SampleInterval = TimeSpan.FromSeconds(10)
			};

		/// <summary>
		/// Initialize EGL on the Miscrosoft Windows platform. Requires a window handle.
		/// </summary>
		/// <param name="hwnd">The Window handle to initialize EGL on</param>
		/// <param name="bind">The EGL API to bind the current thread to</param>
		internal static void InitWin32(IntPtr hwnd, EglApi bind)
		{
			extrahandle = new Handle("Windows Device Context", GetDC(hwnd), delegate(IntPtr h)
			{
				ReleaseDC(hwnd, h);
			});
			Init(hwnd, extrahandle, bind);
		}

		/// <summary>
		/// Initialize EGL on the Dispmanx platform. Requires a Dispmanx DisplayDevice
		/// </summary>
		/// <param name="device">The Dispmanx DisplayDevice to initialize EGL on</param>
		/// <param name="bind">The EGL API to bind the current thread to</param>
		internal static void InitDispmanx(DisplayDevice device, EglApi bind)
		{
			// Important stuff! https://www.raspberrypi.org/forums/viewtopic.php?f=68&t=30261 (halleluya for this guy catching this! Would have taken me a while...)
			// Apparently the expectation of EGL is that window objects stay in place. Well that might be true for C/C++/X11/Win32 but it certainly isn't in .net world.
			// So we have to ensure we never move the struct we are gonna give to eglCreateWindowSurface, otherwise you get mayhem and madness! (belive me I checked).
			// Two options are, using a GCHandle with pinned type, or Marshal heap allocation. Where the latter is designed for this situation, and GCHandle involves
			// dealing with valuetype boxing nastyness.
			extrahandle = new Handle("EGL Native Window", Marshal.AllocHGlobal(Marshal.SizeOf(typeof(EglDispmanxWindow))), Marshal.FreeHGlobal);
			Marshal.StructureToPtr(new EglDispmanxWindow() { dispmanx_element = device.Element, size = device.Mode.Size }, extrahandle, false);
			Init(extrahandle, IntPtr.Zero, bind);

			PiGameSharp.VG.VG.RenderSize = device.Mode.Size;
		}

		/// <summary>
		/// Initialize EGL on an X11 window
		/// </summary>
		/// <param name="bind">The EGL API to bind the current thread to</param>
		internal static void InitX11(EglApi bind)
		{
			//TODO: x11 init
		}

		private static void Init(IntPtr hwnd, IntPtr hdc, EglApi bind)
		{
			if (display != IntPtr.Zero)
				return;

			IntPtr d;
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
			
			//TODO: make enum for these consts...
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

			windowsurface = new Handle(
				"EGL Window Surface",
				eglCreateWindowSurface(display, config, hwnd, IntPtr.Zero),
				delegate(IntPtr h)
				{
					if (!eglDestroySurface(d, h))
						throw new Exception("Unable to destroy window surface " + eglGetError());
				});
			if (windowsurface.IsInvalid)
				throw new Exception("Unable to create window surface " + eglGetError());

			if (!eglMakeCurrent(display, windowsurface, windowsurface, context))
				throw new Exception("Unable to make surface and context current " + eglGetError());

			//this apparently forces egl to reevaluate the sizes of the color buffers, so we are ready to draw stuff.
			Swap();
		}

		/// <summary>
		/// Deinitialize EGL and release all resources
		/// </summary>
		internal static void Deinit()
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
			if (extrahandle != null)
				extrahandle.Close();

			windowsurface = context = display = extrahandle = null;
		}

		internal static void Swap()
		{
			frames.Add(1.0);
			if (!eglSwapBuffers(display, windowsurface))
				throw new Exception("Unable to swap buffers " + eglGetError());
		}
	}
}
