﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace PiGameSharp.VG
{
	public static class VG
	{
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern ErrorCode vgGetError();
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgFinish();
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgClear(int x, int y, int width, int height);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgGetVectorSize")] internal static extern int vgGetArraySize(Parameter type);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgGetParameterVectorSize")] internal static extern int vgGetArraySize(IntPtr handle, Parameter type);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgGeti")]           internal static extern int   vgGeti(Parameter type);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgSeti")]           internal static extern void  vgSet(Parameter type, int value);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgGetf")]           internal static extern float vgGetf(Parameter type);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgSetf")]           internal static extern void  vgSet(Parameter type, float value);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgGetiv")]          internal static extern void  vgGet(Parameter type, int count, [In] int[] values);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgSetiv")]          internal static extern void  vgSet(Parameter type, int count, [Out] int[] values);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgGetfv")]          internal static extern void  vgGet(Parameter type, int count, [In] float[] values);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgSetfv")]          internal static extern void  vgSet(Parameter type, int count, [Out] float[] values);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgGetParameteri")]  internal static extern int   vgGeti(IntPtr handle, Parameter type);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgSetParameteri")]  internal static extern void  vgSet(IntPtr handle, Parameter type, int value);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgGetParameterf")]  internal static extern float vgGetf(IntPtr handle, Parameter type);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgSetParameterf")]  internal static extern void  vgSet(IntPtr handle, Parameter type, float value);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgGetParameteriv")] internal static extern void  vgGet(IntPtr handle, Parameter type, int count, [In] int[] values);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgSetParameteriv")] internal static extern void  vgSet(IntPtr handle, Parameter type, int count, [In, Out] int[] values);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgGetParameterfv")] internal static extern void  vgSet(IntPtr handle, Parameter type, int count, [In] float[] values);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint="vgSetParameterfv")] internal static extern void  vgGet(IntPtr handle, Parameter type, int count, [In, Out] float[] values);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgLoadIdentity();
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgLoadMatrix(ref Matrix m);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgGetMatrix(ref Matrix m);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgMultMatrix(ref Matrix m);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern IntPtr vgCreateImage(ImageFormat format, int width, int height, ImageRenderQuality acceptable);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgImageSubData(IntPtr image, byte[] data, int dataStride, ImageFormat dataFormat, uint x, uint y, uint width, uint height);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgDrawImage(IntPtr image);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgDestroyImage(IntPtr image);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern IntPtr vgCreatePath(int pathFormat /*0*/, int datatype /*float = 3*/, float scale /*1*/, float bias /*0*/, int segCapacityHint /*0*/, int coordCapacityHint /*0*/, int capabilities);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgAppendPathData(IntPtr path, int numSegments, byte[] pathSegments, float[] pathData);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgDrawPath(IntPtr path);
		[DllImport("libOpenVG.dll", CallingConvention=CallingConvention.Cdecl)] internal static extern void vgDestroyPath(IntPtr path);

		public static int MaxImageWidth
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return vgGeti(Parameter.MAX_IMAGE_WIDTH);
			}
		}
		public static int MaxImageHeight
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return vgGeti(Parameter.MAX_IMAGE_HEIGHT);
			}
		}
		public static int MaxImagePixels
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return vgGeti(Parameter.MAX_IMAGE_PIXELS);
			}
		}
		public static int MaxImageBytes
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return vgGeti(Parameter.MAX_IMAGE_BYTES);
			}
		}

		public static BlendMode Blending
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (BlendMode)vgGeti(Parameter.BLEND_MODE);
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
				vgSet(Parameter.BLEND_MODE, (int)value);
				DetectError("Error setting blend mode");
			}
		}

		public static ImageRenderQuality ImageQuality
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (ImageRenderQuality)vgGeti(Parameter.IMAGE_QUALITY);
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
				vgSet(Parameter.IMAGE_QUALITY, (int)value);
				DetectError("Error setting image quality");
			}
		}

		public static ImageRenderQuality RenderQuality
		{
			get
			{
				int q = vgGeti(Parameter.RENDERING_QUALITY);
				if (q == 0x1200)
					return ImageRenderQuality.NonAntialiased;
				else if (q == 0x1201)
					return ImageRenderQuality.Faster;
				else
					return ImageRenderQuality.Better;
			}
			set
			{
				if (value == ImageRenderQuality.NonAntialiased)
					vgSet(Parameter.RENDERING_QUALITY, 0x1200);
				else if (value == ImageRenderQuality.Faster)
					vgSet(Parameter.RENDERING_QUALITY, 0x1201);
				else if (value == ImageRenderQuality.Better)
					vgSet(Parameter.RENDERING_QUALITY, 0x1202);
				DetectError("Error setting render quality");
			}
		}

		public static Vector2 RenderSize { get; set; }

		public static Matrix ImageToSurfaceTransform
		{
			set
			{
				vgLoadMatrix(ref value);
				DetectError("Load Matrix");
			}
		}

		public static Matrix PathToSurfaceTransform
		{
			set
			{
				vgSet(Parameter.MATRIX_MODE, (int)MatrixMode.PathUserToSurface);
				DetectError("Set Matrix Mode failed");
				vgLoadMatrix(ref value);
				DetectError("Load Matrix");
				vgSet(Parameter.MATRIX_MODE, (int)MatrixMode.ImageUserToSurface);
				DetectError("Set Matrix Mode back failed");
			}
		}

		public static Matrix GlyphToSurfaceTransform
		{
			set
			{
				vgSet(Parameter.MATRIX_MODE, (int)MatrixMode.GlyphUserToSurface);
				DetectError("Set Matrix Mode failed");
				vgLoadMatrix(ref value);
				DetectError("Load Matrix");
				vgSet(Parameter.MATRIX_MODE, (int)MatrixMode.ImageUserToSurface);
				DetectError("Set Matrix Mode back failed");
			}
		}


		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Show()
		{
			ErrorCode err;
			vgFinish();
			if ((err = vgGetError()) != ErrorCode.NoError)
				throw new Exception("Error detected on show frame (" + err + ")");
			PiGameSharp.EGL.EGL.Swap();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Clear()
		{
			vgSet(Parameter.CLEAR_COLOR, 4, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
			DetectError("Error setting clear color");
			vgClear(0, 0, (int)RenderSize.x, (int)RenderSize.y);
			DetectError("Error clearing");
			vgSet(Parameter.MATRIX_MODE, (int)MatrixMode.ImageUserToSurface);
			DetectError("Error setting default matrix mode");
		}

		[Conditional("DEBUG")]
		public static void DumpContext() => DumpContext(null);

		[Conditional("DEBUG")]
		internal static void DumpContext(Handle h)
		{
			foreach (Parameter p in Enum.GetValues(typeof(Parameter)))
				DumpParameter(p, h==null?IntPtr.Zero:h);
		}

		[Conditional("DEBUG")]
		private static void DumpParameter(Parameter p, IntPtr handle)
		{
			object val = ParameterInfoAttribute.Get(p, handle);
			if (val == null)
				return;

			if (val is int[])
			{
				string[] data = new string[((int[])val).Length];
				for (int i = 0; i < data.Length; i++)
					data[i] = "0x" + ((int[])val)[i].ToString("X");
				val = "{" + string.Join(", ", data) + "}";
			}
			else if (val is float[])
				val = "{" + string.Join(", ", (float[])val) + "}";
			System.Diagnostics.Debug.WriteLine("Parameter: " + p + " value: " + val);
		}

		[Conditional("DEBUG")]
		public static void DetectError(string msg)
		{
			ErrorCode err;
			if ((err = vgGetError()) != ErrorCode.NoError)
				throw new Exception(msg + " (" + err + ")");
		}
	}
}