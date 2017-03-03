using System;
using System.Collections.Generic;
using System.Threading;

namespace PiGameSharp
{
	/// <summary>
	/// Used to periodically report counter delta's
	/// </summary>
	public sealed class PerformanceCounter : IComparable<PerformanceCounter>, IDisposable
	{
		private static List<PerformanceCounter> counters = new List<PerformanceCounter>();
		private static bool active = false;

		static PerformanceCounter()
		{
			ThreadPool.QueueUserWorkItem(PrintCounters);
		}

		/// <summary>
		/// Starts periodic sampling of all performance counters
		/// </summary>
		public static void StartStatsSampling()
		{
			active = true;
		}

		/// <summary>
		/// Stops periodic sampling of all performance counters
		/// </summary>
		public static void StopStatsSampling()
		{
			active = false;
		}

		private static void PrintCounters(object state)
		{
			while (true)
			{
				int sleep = 100;
				if (active)
					lock (counters)
					{
						if (counters.Count > 0)
						{
							foreach (PerformanceCounter c in counters)
								if (c.NextSample < DateTime.Now.Ticks)
									c.Sample();
							counters.Sort();
							sleep = (int)((counters[0].NextSample - DateTime.Now.Ticks) / TimeSpan.TicksPerMillisecond);
						}
					}

				// During debugging (or if all counters are disabled), sleep may be a large or negative number, which Thread.Sleep doesn't like or never wakes up from
				// So clamp it to [0, 1sec] interval
				Thread.Sleep(Math.Min(1000, Math.Max(0, sleep)));
			}
		}

		private double unitduration;
		private long sampleinterval;
		private long lastsample;
		private double counter;

		/// <summary>
		/// Initializes a PerformanceCounter with a name, no unit name, 1 second presentation interval and default sample action (print)
		/// </summary>
		/// <param name="name">The name for this performance counter</param>
		public PerformanceCounter(string name) : this(name, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), null)
		{ }

		/// <summary>
		/// Initializes a PerformanceCounter with a name, unit name, presentation interval and sample action
		/// </summary>
		/// <param name="name">The name for this performance counter</param>
		/// <param name="unit">The name of the unit that this performace counter measures (frames, calls, keystrokes, etc.)</param>
		/// <param name="unitduration">The duration of the unit that this counter measures (usually 1 second or milisecond)</param>
		/// <param name="sampleinterval">The interval at which to sample/report this counter</param>
		/// <param name="act">The activity to perform when a sample is taken</param>
		public PerformanceCounter(string name, string unit, TimeSpan unitduration, TimeSpan sampleinterval, Action<double> act)
		{
			Enabled = true;
			Name = name;
			Unit = unit;
			UnitDuration = unitduration;
			SampleInterval = sampleinterval;
			lastsample = DateTime.Now.Ticks;
			if (act == null)
				act = PrintCounter;
			SampleAction = act;
			lock (counters)
				counters.Add(this);
		}
		
		/// <summary>
		/// The state of this counter for periodic sample reporting
		/// </summary>
		/// <remarks>It can take upto a second before a reenabled counter resumes periodic reporting</remarks>
		public bool Enabled { get; set; }

		/// <summary>
		/// The name for this performance counter
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The name of the unit that this performace counter measures (frames, calls, keystrokes, etc.)
		/// </summary>
		public string Unit { get; set; }

		/// <summary>
		/// The activity to perform when a sample is taken
		/// </summary>
		public Action<double> SampleAction { get; set; }
		
		/// <summary>
		/// The interval at which to sample/report this counter
		/// </summary>
		public TimeSpan SampleInterval
		{
			get
			{
				return TimeSpan.FromTicks(sampleinterval);
			}
			set
			{
				sampleinterval = value.Ticks;
			}
		}

		/// <summary>
		/// The duration of the unit that this counter measures (usually 1 second or milisecond)
		/// </summary>
		public TimeSpan UnitDuration
		{
			get
			{
				return TimeSpan.FromSeconds(unitduration);
			}
			set
			{
				unitduration = value.TotalSeconds;
			}
		}

		private long NextSample
		{
			get
			{
				if (Enabled)
					return lastsample + sampleinterval;
				else
					return lastsample + (long)int.MaxValue * TimeSpan.TicksPerMillisecond;
			}
		}

		/// <summary>
		/// Takes a sample and performs the sample action
		/// </summary>
		public void Sample()
		{
			lock (this)
			{
				long ticks = DateTime.Now.Ticks;
				long interval = ticks - lastsample;
				lastsample = ticks;
				if (SampleAction != null)
					SampleAction(counter / (((double)interval/(double)TimeSpan.TicksPerSecond)/unitduration));
				counter = 0;
			}
		}

		/// <summary>
		/// Add to or substract from this counter
		/// </summary>
		/// <param name="value">The value to add or subtract</param>
		public void Add(double value)
		{
			lock (this)
			{
				counter += value;
			}
		}

		private void PrintCounter(double value)
		{
			string line = Name + " " + value.ToString("#0.000") + " " + Unit + "/";
			if (unitduration != 1.0)
				line += unitduration.ToString();
			line += "s";
			Console.WriteLine(line);
			System.Diagnostics.Debug.WriteLine(line);
		}

		/// <summary>
		/// Compares this instance of performance counter to another. Results in an ordering on the next sample time of the counters.
		/// </summary>
		/// <param name="that">The other performance counter to compare against</param>
		/// <returns>Less than zero if this instance should sample next before the other performance counter, zero if they sample at the same time, greater than zero otherwise</returns>
		public int CompareTo(PerformanceCounter that)
		{
			if (that == null)
				return 1;
			return this.NextSample.CompareTo(that.NextSample);
		}

		/// <summary>
		/// Increment a performance counter.
		/// </summary>
		/// <param name="item">The performance counter to increment</param>
		/// <returns>The performance counter</returns>
		public static PerformanceCounter operator ++(PerformanceCounter item)
		{
			item.Add(1.0);
			return item;
		}

		/// <summary>
		/// Decrement a performance counter.
		/// </summary>
		/// <param name="item">The performance counter to decrement</param>
		/// <returns>The performance counter</returns>
		public static PerformanceCounter operator --(PerformanceCounter item)
		{
			item.Add(-1.0);
			return item;
		}

		/// <summary>
		/// Represents the performance counter as it's counter value
		/// </summary>
		/// <param name="item">The performance counter to convert</param>
		/// <returns>The counter field of the performance counter</returns>
		public static explicit operator double(PerformanceCounter item)
		{
			return item.counter;
		}

		/// <summary>
		/// Removes this counter from the periodic sampling list, allowing the performance counter to be GC'ed
		/// </summary>
		public void Dispose()
		{
			lock (counters)
				counters.Remove(this);
		}
	}
}
