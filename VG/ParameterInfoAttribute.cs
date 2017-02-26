using System;
namespace PiGameSharp.VG
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	sealed class ParameterInfoAttribute : Attribute
	{
		public Type Type { get; set; }
		public ParameterAccessor Access { get; set; }

		public ParameterInfoAttribute(ParameterAccessor a)
		{
			Access = a;
			switch (a)
			{
				case ParameterAccessor.Int:
					Type = typeof(int);
					break;
				case ParameterAccessor.Float:
					Type = typeof(float);
					break;
				case ParameterAccessor.IntArray:
					Type = typeof(int[]);
					break;
				case ParameterAccessor.FloatArray:
					Type = typeof(float[]);
					break;
				default:
					break;
			}
		}
		
		public ParameterInfoAttribute(ParameterAccessor a, Type t)
		{
			Access = a;
			if (a == ParameterAccessor.FloatArray)
				Type = typeof(float[]);
			else if (a == ParameterAccessor.IntArray)
				Type = typeof(int[]);
			else
				Type = t;
		}

		public ParameterInfoAttribute(Type t)
		{
			Type = t;

			bool wasarray = t.IsArray;
			if (wasarray)
				t = t.GetElementType();

			if (t.IsEnum)
				Access = ParameterAccessor.Int;
			else if (t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort) || t == typeof(int) ||t == typeof(uint) || t == typeof(bool))
				Access = ParameterAccessor.Int;
			else if (t == typeof(float) || t == typeof(double))
				Access = ParameterAccessor.Float;
			else
				Access = ParameterAccessor.Int;

			if (wasarray && Access == ParameterAccessor.Float)
				Access = ParameterAccessor.FloatArray;
			if (wasarray && Access == ParameterAccessor.Int)
				Access = ParameterAccessor.IntArray;
		}

		public static object Get(Parameter p, IntPtr handle)
		{
			object[] attrs = typeof(Parameter).GetField(typeof(Parameter).GetEnumName(p)).GetCustomAttributes(typeof(ParameterInfoAttribute), false);
			ParameterInfoAttribute item;
			if (attrs.Length > 0)
				item = (ParameterInfoAttribute)attrs[0];
			else
				item = new ParameterInfoAttribute(ParameterAccessor.Int);

			int size;
			object ret;

			switch (item.Access)
			{
				case ParameterAccessor.Int:
					if (handle == IntPtr.Zero)
						ret = VG.vgGeti(p);
					else
						ret = VG.vgGeti(handle, p);
					break;
				case ParameterAccessor.Float:
					if (handle == IntPtr.Zero)
						ret = VG.vgGetf(p);
					else
						ret = VG.vgGetf(handle, p);
					break;
				case ParameterAccessor.IntArray:
					if (handle == IntPtr.Zero)
						size = VG.vgGetArraySize(p);
					else
						size = VG.vgGetArraySize(handle, p);
					if (VG.vgGetError() == ErrorCode.NoError)
					{
						ret = new int[size];
						if (size > 0)
							if (handle == IntPtr.Zero)
								VG.vgGet(p, size, (int[])ret);
							else
								VG.vgGet(handle, p, size, (int[])ret);
					}
					else
						ret = null;
					break;
				case ParameterAccessor.FloatArray:
					if (handle == IntPtr.Zero)
						size = VG.vgGetArraySize(p);
					else
						size = VG.vgGetArraySize(handle, p);
					if (VG.vgGetError() == ErrorCode.NoError)
					{
						ret = new float[size];
						if (size > 0)
							if (handle == IntPtr.Zero)
								VG.vgGet(p, size, (float[])ret);
							else
								VG.vgGet(handle, p, size, (float[])ret);
					}
					else
						ret = null;
					break;
				default:
					return null;
			}
			if (VG.vgGetError() != ErrorCode.NoError)
				ret = null;
			if (ret != null && ret.GetType() != item.Type)
				if (item.Type.IsEnum)
					ret = Enum.ToObject(item.Type, ret);
				else
					ret = Convert.ChangeType(ret, item.Type);
			return ret;
		}
	}

	enum ParameterAccessor
	{
		Int,
		Float,
		IntArray,
		FloatArray
	}
}
