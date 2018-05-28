using System.Threading;

namespace PiGameSharp
{
	/// <summary>
	/// A (un)loadable resource identified by an int32 resource key.
	/// </summary>
	public abstract class Resource
	{
		private static int ResourceId = 0;

		protected readonly object sync_key = new object();

		protected Resource() => Key = Interlocked.Increment(ref ResourceId);

		/// <summary>
		/// The unique resource identifier for this resource
		/// </summary>
		public int Key { get; }

		/// <summary>
		/// Load the resource and prepare it for use
		/// </summary>
		public void LoadSync()
		{
			lock (sync_key)
				Load();
		}

		/// <summary>
		/// Releases all resource used by the <see cref="PiGameSharp.Resource"/> object.
		/// </summary>
		public void UnloadSync()
		{
			lock (sync_key)
				Unload();
		}

		protected abstract void Load();
		protected abstract void Unload();
	}
}
