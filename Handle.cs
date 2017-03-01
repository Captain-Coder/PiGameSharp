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
			System.Diagnostics.Debug.WriteLine("Allocated handle 0x" + handle.ToString("X") + " tag: " + Tag);
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
				System.Diagnostics.Debug.WriteLine("Releasing handle 0x" + handle.ToString("X") + " tag: " + Tag);
				rel(handle);
			}
			else
				System.Diagnostics.Debug.WriteLine("Double release handle 0x" + handle.ToString("X") + " tag: " + Tag);
			return rel != null;
		}

		public static implicit operator IntPtr(Handle inst)
		{
			if (inst == null)
				return IntPtr.Zero;
			return inst.handle;
		}
	}
}
