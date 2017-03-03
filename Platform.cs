using System;
using System.IO;
using PiGameSharp.Dispmanx;
using PiGameSharp.EGL;

namespace PiGameSharp
{
	/// <summary>
	/// Initialization for the PiGameSharp platform
	/// </summary>
	public static class Platform
	{
		private static Action deinit;

		/// <summary>
		/// Detect the current platform and perform relevant initialisation
		/// </summary>
		public static void Init(object target)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				WindowsInit(target);
			else if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				string hw = File.ReadAllText("/proc/cpuinfo");
				if (hw.Contains("BCM2708") || hw.Contains("BCM2709"))
					PiInit();
				else
					LinuxInit();
			}
		}

		/// <summary>
		/// Deinitialize and release all resources.
		/// </summary>
		public static void Deinit()
		{
			PiGameSharp.EGL.EGL.Deinit();
			if (deinit != null)
				deinit();
		}

		private static void WindowsInit(object target)
		{
			if (!(target is IntPtr))
				throw new ArgumentException("Windows init expects a window handle IntPtr");

			PiGameSharp.EGL.EGL.InitWin32((IntPtr)target, EglApi.OpenVG);
		}

		private static void PiInit()
		{
			deinit = PiDeinit;

			BcmHost.Init();
			Console.WriteLine("After init");
			if (BcmHost.Devices == null || BcmHost.Devices.Count < 0)
				throw new Exception("No dispmanx display available");
			PiGameSharp.EGL.EGL.InitDispmanx(BcmHost.Devices[0], EglApi.OpenVG);
		}

		private static void PiDeinit()
		{
			BcmHost.Deinit();
		}

		private static void LinuxInit()
		{
			deinit = LinuxDeinit;
		}

		private static void LinuxDeinit()
		{

		}
	}
}

