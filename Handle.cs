using System;
using System.Runtime.InteropServices;
using System.Threading;
namespace PiGameSharp
{
	internal class Handle : CriticalHandle
	{
		private string Tag;
		private Action<IntPtr> Release;

		public Handle(string tag, IntPtr hnd, Action<IntPtr> release) : base(IntPtr.Zero)
		{
			Tag = tag;
			handle = hnd;
			Release = release;
			Out("Allocated handle 0x" + handle.ToString("X") + " tag: " + Tag);
		}

		public override bool IsInvalid
		{
			get
			{
				return handle == IntPtr.Zero || Release == null;
			}
		}

		protected override bool ReleaseHandle()
		{
			Action<IntPtr> rel = Interlocked.Exchange(ref Release, null);
			if (rel != null)
			{
				Out("Releasing handle 0x" + handle.ToString("X") + " tag: " + Tag);
				rel(handle);
			}
			else
				Out("Double release handle 0x" + handle.ToString("X") + " tag: " + Tag);
			return rel != null;
		}

		public static implicit operator IntPtr(Handle inst)
		{
			if (inst == null)
				return IntPtr.Zero;
			return inst.handle;
		}

		public static void Out(string msg)
		{
#if WINDOWS
			System.Diagnostics.Debug.WriteLine(msg);
#else
			Console.WriteLine(msg);
#endif
		}
	}
}
