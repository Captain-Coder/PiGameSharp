using System;
using System.Collections.Generic;

namespace PiGameSharp.VG
{
	public class Path : Resource
	{
		/// <summary>
		/// Performance counter to measure image draw rate
		/// </summary>
		public static PerformanceCounter draws = new PerformanceCounter("Path.Draw()")
		{
			SampleInterval = TimeSpan.FromSeconds(10),
			Unit = "calls"
		};

		private Handle handle;
		private Func<Segment[]> source;

		public Path(Func<Segment[]> datasource)
		{
			source = datasource;
		}

		protected override void Load()
		{
			IntPtr hnd = VG.vgCreatePath(0, 3, 1, 0, 0, 0, 6);
			if (hnd == IntPtr.Zero)
				throw new Exception("Error creating Path " + VG.vgGetError());
			handle = new Handle("OpenVG Path", hnd, delegate (IntPtr h)
			{
				VG.vgDestroyPath(h);
			});
			List<byte> segments = new List<byte>();
			List<float> data = new List<float>();
			foreach (Segment s in source())
			{
				segments.Add((byte)s.type);
				data.AddRange(s.data);
			}
			VG.vgAppendPathData(handle, segments.Count, segments.ToArray(), data.ToArray());
			VG.DetectError("Unable to append path data");
		}

		protected override void Unload()
		{
			if (handle != null)
			{
				handle.Dispose();
				handle = null;
			}
		}

		internal void Draw()
		{
			if (handle == null)
				throw new ObjectDisposedException("Some Path");
			VG.vgDrawPath(handle);
			VG.DetectError("Draw Path failed");
			draws.Add(1.0);
		}

		public static Segment[] Rectangle(float width, float height)
		{
			return new Segment[] {
				new Segment() { type = SegmentsType.HorizontalLineToRelative, data = new float[] { width } },
				new Segment() { type = SegmentsType.VerticalLineToRelative, data = new float[] { height } },
				new Segment() { type = SegmentsType.HorizontalLineToRelative, data = new float[] { -width } },
				new Segment() { type = SegmentsType.VerticalLineToRelative, data = new float[] { -height } }
			};
		}

		public class Segment
		{
			public SegmentsType type;
			public float[] data;
		}

		public enum SegmentsType : byte
		{
			ClosePath = 0,
			MoveToAbsolute = 2,
			MoveToRelative = 3,
			LineToAbsolute = 4,
			LineToRelative = 5,
			HorizontalLineToAbsolute = 6,
			HorizontalLineToRelative = 7,
			VerticalLineToAbsolute = 8,
			VerticalLineToRelative = 9,
			QuadraticCurveToAbsolute = 10,
			QuadraticCurveToRelative = 11,
			CubicCurveToAbsolute = 12,
			CubicCurveToRelative = 13,
			SmoothQuadraticCurveToAbsolute = 14,
			SmoothQuadraticCurveToRelative = 15,
			SmoothCubicCurveToAbsolute = 16,
			SmoothCubicCurveToRelative = 17,
			SmallArcCounterClockwiseToAbsolute = 18,
			SmallArcCounterClockwiseToRelative = 19,
			SmallArcClockwiseToAbsolute = 20,
			SmallArcClockwiseToRelative = 21,
			LargeArcCounterClockwiseToAbsolute = 22,
			LargeArcCounterClockwiseToRelative = 23,
			LargeArcClockwiseToAbsolute = 24,
			LargeArcClockwiseToRelative = 25
		}
	}
}
