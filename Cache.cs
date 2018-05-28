using System.Collections.Generic;

namespace PiGameSharp
{
	/// <summary>
	/// A resource cache using the Adaptive Replacement Cache (ARC) design.
	/// </summary>
	public class Cache
	{
		// TODO: Come up with better locking, it's rather heavy handed right now.
		private object sync_key = new object();

		private LinkedList<Resource> MRU_ghost = new LinkedList<Resource>();
		private LinkedList<Resource> MRU = new LinkedList<Resource>();
		private LinkedList<Resource> MFU = new LinkedList<Resource>();
		private LinkedList<Resource> MFU_ghost = new LinkedList<Resource>();

		private int cache = 100;
		private int split = 30;

		/// <summary>
		/// Gets or sets the size of the cache.
		/// </summary>
		/// <remarks>
		/// This value sets the number of items kept loaded in the cache. Items that become unloaded are still tracked by the cache in ghost lists. The total number of items tracked by the cache is 2 * CacheSize.
		/// 
		/// If the cache size is reduced below the number of items currently kept in the cache, items are first eviced from the Most Recently Used side, after that the Most Frequently Used side.
		/// </remarks>
		public int CacheSize
		{
			get => cache;
			set
			{
				if (value < 0)
					value = 0;
				lock (sync_key)
				{
					while (value < cache)
					{
						if (split > 1)
						{
							Evict(MRU, MRU_ghost);
							split--;
						}
						else
							Evict(MFU, MFU_ghost);
						cache--;
					}
					if (value > cache)
						split += (value - cache) / 2;
					cache = value;
					while (MRU_ghost.Count > (cache >> 1))
						Evict(MRU_ghost, null);
					while (MFU_ghost.Count > (cache >> 1))
						Evict(MFU_ghost, null);
				}
			}
		}

		/// <summary>
		/// Adds an item to the cache, evicting older or less frequently used resources if needed.
		/// </summary>
		/// <param name="r">The resource to add</param>
		public void Add(Resource r)
		{
			lock (sync_key)
			{
				foreach (LinkedListNode<Resource> item in GetItems())
					if (item.Value.Key == r.Key)
						return;
				AddMRU(r);
			}
			r.LoadSync();
		}

		/// <summary>
		/// Finds a resource in the cache
		/// </summary>
		/// <param name="key">The key of the resource to lookup</param>
		/// <returns>The requested resource if found, null otherwise</returns>
		public Resource Lookup(int key)
		{
			lock (sync_key)
			{
				foreach (LinkedListNode<Resource> item in GetItems())
					if (item.Value.Key == key)
					{
						if (item.List == MFU_ghost)
						{
							if (split > 1)
								split--;
						}
						else if (item.List == MRU_ghost)
						{
							if (split < cache - 1)
								split++;
						}
						item.List.Remove(item);
						AddMFU(item.Value);
						return item.Value;
					}
			}
			return null;
		}

		/// <summary>
		/// Flushes a specific entry from the cache
		/// </summary>
		/// <param name="key">The resource to flush</param>
		/// <remarks>This operation causes the flushed resource to become unloaded</remarks>
		public void Flush(int key)
		{
			lock (sync_key)
			{
				foreach (LinkedListNode<Resource> item in GetItems())
					if (item.Value.Key == key)
					{
						item.Value.UnloadSync();
						item.List.Remove(item);
						break;
					}
			}
		}

		/// <summary>
		/// Flushes the entire cache
		/// </summary>
		/// <remarks>This operation causes the flushed resources to become unloaded</remarks>
		public void Flush()
		{
			lock (sync_key)
			{
				foreach (LinkedListNode<Resource> item in GetItems())
					item.Value.UnloadSync();
				MFU.Clear();
				MRU.Clear();
				MFU_ghost.Clear();
				MRU_ghost.Clear();
			}
		}

		private void AddMRU(Resource r)
		{
			MRU.AddFirst(r);
			while (MRU.Count >= split)
				Evict(MRU, MRU_ghost);
		}

		private void AddMFU(Resource r)
		{
			MFU.AddFirst(r);
			while (MFU.Count >= (cache - split))
				Evict(MFU, MFU_ghost);
		}

		private void Evict(LinkedList<Resource> list, LinkedList<Resource> list_ghost)
		{
			Resource item = list.Last.Value;
			list.RemoveLast();
			if (list_ghost != null)
			{
				item.UnloadSync();
				list_ghost.AddFirst(item);
				while (list_ghost.Count > (cache >> 1))
					Evict(list_ghost, null);
			}
		}

		private IEnumerable<LinkedListNode<Resource>> GetItems()
		{
			LinkedListNode<Resource> finger;
			foreach (LinkedList<Resource> list in new LinkedList<Resource>[] { MFU, MRU, MFU_ghost, MRU_ghost })
			{
				finger = list.First;
				while (finger != null)
				{
					LinkedListNode<Resource> next = finger.Next;  // allow list modifications (only item finger) outside of the enumerator by getting the next element first.
					yield return finger;
					finger = next;
				}
			}
		}
	}
}
