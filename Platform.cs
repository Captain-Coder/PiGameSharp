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
				string hw = File.ReadAllText("/proc/cpuinfo").ToLower();
				if (hw.Contains("bcm2708") || hw.Contains("bcm2709") || hw.Contains("bcm2837") || hw.Contains("bcm2836") ||  hw.Contains("bcm2835"))
					PiInit();
				else
					LinuxInit();
			}

			Input.Init();
		}

		/// <summary>
		/// Deinitialize and release all resources.
		/// </summary>
		public static void Deinit()
		{
			PiGameSharp.EGL.EGL.Deinit();
			if (deinit != null)
				deinit();

			Input.Deinit();
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
			if (BcmHost.Devices == null || BcmHost.Devices.Count < 0)
				throw new Exception("No dispmanx display available");
			PiGameSharp.EGL.EGL.InitDispmanx(BcmHost.Devices[0], EglApi.OpenVG);

			PiGameSharp.Sound.ALSA.Init();
		}

		private static void PiDeinit()
		{
			PiGameSharp.Sound.ALSA.Deinit();
			BcmHost.Deinit();
		}

		private static void LinuxInit()
		{
			PiGameSharp.Sound.ALSA.Init();
			deinit = LinuxDeinit;
		}

		private static void LinuxDeinit()
		{
			PiGameSharp.Sound.ALSA.Deinit();
		}
	}
}

