using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PiGameSharp
{
	public static class ResourceLibrary
	{
		static ResourceLibrary()
		{
			ResourceFactory.Register(typeof(ResourceLibrary).Assembly);

			AppDomain.CurrentDomain.AssemblyLoad += delegate (object sender, AssemblyLoadEventArgs e) { ResourceFactory.Register(e.LoadedAssembly);  Register(e.LoadedAssembly); };
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				ResourceFactory.Register(a);
				Register(a);
			}
		}

		private static List<Resource> resources = new List<Resource>();
		private static Cache cache = new Cache();
		private static Dictionary<string, int> keymap = new Dictionary<string, int>();

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
		public static Resource Get(string name) => keymap.ContainsKey(name) ? Get(keymap[name]) : null;

		public static void Register(Resource item)
		{
			if (resources.Contains(item))
				return;
			resources.Add(item);
		}

		public static void Register(Assembly target)
		{
			ResourceFactory.Register(target);
			foreach (string name in target.GetManifestResourceNames())
			{
				Resource r = null;
				Dictionary<string, string> arguments = new Dictionary<string, string>();
				string[] args = name.Split('_');
				string key = args[0];
				for (int i = 1; i < args.Length; i++)
					if (args[i].Contains("="))
						arguments[args[i].Substring(0, args[i].IndexOf("="))] = args[i].Substring(args[i].IndexOf("=") + 1);
					else
						arguments[args[i]] = "on";

				r = ResourceFactory.Construct(arguments);
				if (r != null)
				{
					key = key + "." + arguments["type"];
					if (keymap.ContainsKey(key))
						continue;
					r.DataSource = delegate
					{
						byte[] data;
						using (Stream s = target.GetManifestResourceStream(name))
						{
							data = new byte[s.Length];
							s.Read(data, 0, data.Length);
						}
						return data;
					};
					keymap[key] = r.Key;
					Register(r);
				}
			}
		}

		public static void Dispose()
		{
			cache.Flush();
			resources.Clear();
			keymap.Clear();
		}
	}
}
