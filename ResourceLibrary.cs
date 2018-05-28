using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PiGameSharp.Sound;
using PiGameSharp.VG;

namespace PiGameSharp
{
	public static class ResourceLibrary
	{
		private static List<Resource> resources = new List<Resource>();
		private static Cache cache = new Cache();
		private static Dictionary<string, int> key_map = new Dictionary<string, int>();

		public static Resource Get(int key)
		{
			Resource ret = cache.Lookup(key);
			if (ret == null)
			{
				ret = resources.Find(x => x.Key == key);
				if (ret != null)
					cache.Add(ret);
			}
			return ret;
		}
		public static Resource Get(string name) => key_map.ContainsKey(name) ? Get(key_map[name]) : null;

		public static void Register(Resource item)
		{
			if (resources.Contains(item))
				return;
			resources.Add(item);
		}

		public static void Register(Assembly target)
		{
			foreach (string name in target.GetManifestResourceNames())
			{
				Resource r = null;
				if (name.EndsWith(".raw"))
				{
					Func<Stream> datasource = delegate
					{
						return target.GetManifestResourceStream(name);
					};
					r = new PCM(datasource);
				}
				else if (name.EndsWith(".bin"))
				{
					string size = System.IO.Path.GetFileNameWithoutExtension(name).Substring(name.LastIndexOf("_") + 1);
					Func<byte[]> datasource = delegate
					{
						byte[] data;
						using(Stream s = target.GetManifestResourceStream(name))
						{
							data = new byte[s.Length];
							s.Read(data, 0, data.Length);
						}
						return data;
					};
					r = new Image(ImageFormat.Rgba8888, new Vector2(uint.Parse(size.Substring(0, size.IndexOf("x"))), uint.Parse(size.Substring(size.IndexOf("x") + 1))), datasource);
				}

				if (r != null)
				{
					key_map[System.IO.Path.GetFileNameWithoutExtension(name)] = r.Key;
					Register(r);
				}
			}
		}

		public static void Dispose()
		{
			cache.Flush();
			resources.Clear();
			key_map.Clear();
		}
	}
}
