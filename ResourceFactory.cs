using System;
using System.Collections.Generic;
using System.Reflection;

namespace PiGameSharp
{
	public static class ResourceFactory
	{
		internal static Dictionary<string, ConstructorInfo> registeredtypes = new Dictionary<string, ConstructorInfo>();

		internal static Resource Construct(Dictionary<string, string> arguments)
		{
			if (!arguments.ContainsKey("type") || !registeredtypes.ContainsKey(arguments["type"]))
				return null;
			return (Resource)registeredtypes[arguments["type"]].Invoke(new object[] { arguments });
		}

		public static void Register(Assembly target)
		{
			foreach (Type t in target.GetTypes())
				ScanType(t);
		}

		private static void ScanType(Type t)
		{
			if (registeredtypes.ContainsKey(t.Name))
				return;
			if (t.IsSubclassOf(typeof(Resource)))
			{
				ConstructorInfo ci = t.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Dictionary<string, string>) }, null);
				if (ci != null)
					registeredtypes[t.Name] = ci;
			}
			foreach (Type tn in t.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public))
				ScanType(tn);
		}
	}
}