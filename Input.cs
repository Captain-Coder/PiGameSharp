using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace PiGameSharp
{
	public class Input
	{
		[DllImport("libc", EntryPoint = "ioctl")] internal static extern int Ioctl(IntPtr handle, uint request, IntPtr item);
	
		private static readonly List<FileStream> streams = new List<FileStream>();
		private static readonly BlockingCollection<InputEvent> eventqueue = new BlockingCollection<InputEvent>();
		
		public static event Action<InputEvent> Handler;

		internal static void Init()
		{
			if (Directory.Exists("/dev/input"))
				//TODO: observe /dev/input for future devices (i.e.: reconnected bluetooth device and such)
				foreach (string f in Directory.GetFiles("/dev/input", "event*", SearchOption.TopDirectoryOnly))
					OpenStream(f);

			if (streams.Count == 0)
			{
				Console.Error.WriteLine("No input devices available, using console input");
				ThreadPool.QueueUserWorkItem(ReadConsoleKeys);
			}
			ThreadPool.QueueUserWorkItem(HandleInputEvents, null);
		}

		public static void Inject(InputEvent ev)
		{
			if (!eventqueue.IsAddingCompleted)
				eventqueue.Add(ev);
		}

		internal static void Deinit()
		{
			for (int i = streams.Count -1 ; i>= 0; i--)
				CloseStream(streams[i]);
			Thread.Sleep(1);
			eventqueue.CompleteAdding();
		}

		private static void OpenStream(string f)
		{
			byte[] str = new byte[Marshal.SizeOf(typeof(InputEvent)) * 4];
			FileStream fs = new FileStream(f, FileMode.Open, FileAccess.Read);
			int result = Ioctl(fs.SafeFileHandle.DangerousGetHandle(), (1u << 30) | (4u << 16) | ((uint)'E' << 8) | (0x90u << 0), (IntPtr)1);
			if (result < 0)
			{
				Console.Error.WriteLine("Unable to grab input " + f + " ioctl return: " + result);
				fs.Close();
			}
			else
			{
				streams.Add(fs);
				StartRead(fs, str);
				//Console.WriteLine("Direct input from: " + f + " ioctl return: " + result);
			}
		}

		private static void StartRead(FileStream fs, byte[] str)
		{
			fs.BeginRead(str, 0, str.Length, delegate(IAsyncResult ar)
			{
				int cnt = 0;
				try
				{
					cnt = fs.EndRead(ar);
				}
				catch (IOException)
				{
					return;
				}
				if (cnt == 0 || eventqueue.IsAddingCompleted || cnt % 16 != 0)
				{
					if (cnt % 16 != 0)
						Console.Error.WriteLine("Error reading from input stream");
					CloseStream(fs);
				}
				else
				{
					GCHandle h = GCHandle.Alloc(str, GCHandleType.Pinned);
					for (int i = 0; i << 4 < cnt; i++)
						Inject((InputEvent)Marshal.PtrToStructure(h.AddrOfPinnedObject() + (i<<4), typeof(InputEvent)));
					StartRead(fs, str);
					h.Free();
				}
			}, null);
		}

		private static void CloseStream(FileStream fs)
		{
			int result = Ioctl(fs.SafeFileHandle.DangerousGetHandle(), (1u << 30) | (4u << 16) | ((uint)'E' << 8) | (0x90u << 0), (IntPtr)0);
			Console.WriteLine("Direct input from: " + fs.Name + " ended, ioctl return: " + result);
			fs.Close();
			streams.Remove(fs);
		}

		private static void ReadConsoleKeys(object state)
		{
			while (!eventqueue.IsAddingCompleted)
			{
				ConsoleKeyInfo e = Console.ReadKey(true);
				Input.InputEvent ev = new Input.InputEvent();
				ev.time.Seconds = (uint)((DateTime.Now.Hour*60 + DateTime.Now.Minute)*60 + DateTime.Now.Second);
				ev.time.MicroSeconds = (uint)(DateTime.Now.Millisecond * 1000);
				ev.type = Input.InputType.Key;
				if (e.Key == ConsoleKey.LeftArrow)
					ev.code = (ushort)Input.KeyCodes.LEFT;
				else if (e.Key == ConsoleKey.RightArrow)
					ev.code = (ushort)Input.KeyCodes.RIGHT;
				else if (e.Key == ConsoleKey.UpArrow)
					ev.code = (ushort)Input.KeyCodes.UP;
				else if (e.Key == ConsoleKey.DownArrow)
					ev.code = (ushort)Input.KeyCodes.DOWN;
				else if (e.Key == ConsoleKey.Enter)
					ev.code = (ushort)Input.KeyCodes.ENTER;
				else
					ev.code = (ushort)e.Key;
				ev.value = 1;
				Input.Inject(ev);
				Thread.Sleep(20);
				ev.value = 0;
				Input.Inject(ev);
			}
		}

		private static void HandleInputEvents(object state)
		{
			while (!eventqueue.IsCompleted)
			{
				try
				{
					InputEvent ev = eventqueue.Take();
					if (Handler != null)
						Handler(ev);
				}
				catch (InvalidOperationException)
				{ }
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct InputEvent
		{
			[FieldOffset(0)]
			public Timestamp time;
			[FieldOffset(8)]
			public InputType type;
			[FieldOffset(10)]
			public ushort code;
			[FieldOffset(12)]
			public uint value;

			public override string ToString() => "Input event @ " + time.Seconds + "." + time.MicroSeconds.ToString("D6") + " type " + type + " code " + code + " value " + value;

			public KeyCodes KeyCode
			{
				get
				{
					return (KeyCodes)code;
				}
			}

			public AbsoluteCodes AxisCode
			{
				get
				{
					return (AbsoluteCodes)code;
				}
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Timestamp
		{
			[FieldOffset(0)]
			public uint Seconds;
			[FieldOffset(4)]
			public uint MicroSeconds;
		}

		public enum InputType : ushort
		{
			Sync                = 0x00,
			Key                 = 0x01,
			Relative            = 0x02,
			Absolute            = 0x03,
			Misc                = 0x04,
			SW                  = 0x05,
			LED                 = 0x11,
			Sound               = 0x12,
			REP                 = 0x14,
			ForceFeedback       = 0x15,
			Power               = 0x16,
			ForceFeedbackStatus = 0x17
		}

		public enum KeyCodes : ushort
		{
			ESC                     = 001,
			K1                      = 002,
			K2                      = 003,
			K3                      = 004,
			K4                      = 005,
			K5                      = 006,
			K6                      = 007,
			K7                      = 008,
			K8                      = 009,
			K9                      = 010,
			K0                      = 011,
			MINUS                   = 012,
			EQUAL                   = 013,
			BACKSPACE               = 014,
			TAB                     = 015,
			Q                       = 016,
			W                       = 017,
			E                       = 018,
			R                       = 019,
			T                       = 020,
			Y                       = 021,
			U                       = 022,
			I                       = 023,
			O                       = 024,
			P                       = 025,
			LEFTBRACE               = 026,
			RIGHTBRACE              = 027,
			ENTER                   = 028,
			LEFTCTRL                = 029,
			A                       = 030,
			S                       = 031,
			D                       = 032,
			F                       = 033,
			G                       = 034,
			H                       = 035,
			J                       = 036,
			K                       = 037,
			L                       = 038,
			SEMICOLON               = 039,
			APOSTROPHE              = 040,
			GRAVE                   = 041,
			LEFTSHIFT               = 042,
			BACKSLASH               = 043,
			Z                       = 044,
			X                       = 045,
			C                       = 046,
			V                       = 047,
			B                       = 048,
			N                       = 049,
			M                       = 050,
			COMMA                   = 051,
			DOT                     = 052,
			SLASH                   = 053,
			RIGHTSHIFT              = 054,
			KeyPadASTERISK          = 055,
			LEFTALT                 = 056,
			SPACE                   = 057,
			CAPSLOCK                = 058,
			F1                      = 059,
			F2                      = 060,
			F3                      = 061,
			F4                      = 062,
			F5                      = 063,
			F6                      = 064,
			F7                      = 065,
			F8                      = 066,
			F9                      = 067,
			F10                     = 068,
			NUMLOCK                 = 069,
			SCROLLLOCK              = 070,
			KeyPad7                 = 071,
			KeyPad8                 = 072,
			KeyPad9                 = 073,
			KeyPadMINUS             = 074,
			KeyPad4                 = 075,
			KeyPad5                 = 076,
			KeyPad6                 = 077,
			KeyPadPLUS              = 078,
			KeyPad1                 = 079,
			KeyPad2                 = 080,
			KeyPad3                 = 081,
			KeyPad0                 = 082,
			KeyPadDOT               = 083,
			F11                     = 087,
			F12                     = 088,
			KeyPadENTER             = 096,
			RIGHTCTRL               = 097,
			KeyPadSLASH             = 098,
			SYSRQ                   = 099,
			RIGHTALT                = 100,
			LINEFEED                = 101,
			HOME                    = 102,
			UP                      = 103,
			PAGEUP                  = 104,
			LEFT                    = 105,
			RIGHT                   = 106,
			END                     = 107,
			DOWN                    = 108,
			PAGEDOWN                = 109,
			INSERT                  = 110,
			DELETE                  = 111,
			MUTE                    = 113,
			VOLUMEDOWN              = 114,
			VOLUMEUP                = 115,
			POWER                   = 116,
			KeyPadEQUAL             = 117,
			KeyPadPLUSMINUS         = 118,
			PAUSE                   = 119,
			MouseLEFT               = 0x110,
			MouseRIGHT              = 0x111,
			MouseMIDDLE             = 0x112,
			MouseSIDE               = 0x113,
			MouseEXTRA              = 0x114,
			MouseFORWARD            = 0x115,
			MouseBACK               = 0x116,
			MouseTASK               = 0x117,
			JoystickTRIGGER         = 0x120,
			JoystickTHUMB           = 0x121,
			JoystickTHUMB2          = 0x122,
			JoystickTOP             = 0x123,
			JoystickTOP2            = 0x124,
			JoystickPINKIE          = 0x125,
			JoystickBASE            = 0x126,
			JoystickBASE2           = 0x127,
			JoystickBASE3           = 0x128,
			JoystickBASE4           = 0x129,
			JoystickBASE5           = 0x12a,
			JoystickBASE6           = 0x12b,
			JoystickDEAD            = 0x12f,
			GamePadSOUTH            = 0x130,
			GamePadEAST             = 0x131,
			GamePadC                = 0x132,
			GamePadNORTH            = 0x133,
			GamePadWEST             = 0x134,
			GamePadZ                = 0x135,
			GamePadTL               = 0x136,
			GamePadTR               = 0x137,
			GamePadTL2              = 0x138,
			GamePadTR2              = 0x139,
			GamePadSELECT           = 0x13a,
			GamePadSTART            = 0x13b,
			GamePadMODE             = 0x13c,
			GamePadTHUMBL           = 0x13d,
			GamePadTHUMBR           = 0x13e
		}

		public enum RelativeCodes : ushort
		{
			X      = 0x00,
			Y      = 0x01,
			Z      = 0x02,
			RX     = 0x03,
			RY     = 0x04,
			RZ     = 0x05,
			HWHEEL = 0x06,
			DIAL   = 0x07,
			WHEEL  = 0x08,
			MISC   = 0x09
		}

		public enum AbsoluteCodes : ushort
		{
			X        = 0x00,
			Y        = 0x01,
			Z        = 0x02,
			RX       = 0x03,
			RY       = 0x04,
			RZ       = 0x05,
			THROTTLE = 0x06,
			RUDDER   = 0x07,
			WHEEL    = 0x08,
			GAS      = 0x09,
			BRAKE    = 0x0a,
			HAT0X    = 0x10,
			HAT0Y    = 0x11,
			HAT1X    = 0x12,
			HAT1Y    = 0x13,
			HAT2X    = 0x14,
			HAT2Y    = 0x15,
			HAT3X    = 0x16,
			HAT3Y    = 0x17
		}
	}
}