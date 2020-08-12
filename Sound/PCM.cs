using System;
using System.Collections.Generic;

namespace PiGameSharp.Sound
{
	public class PCM : Resource
	{
		internal static Action<PCM> play;

		internal float[] samples;
		private object source;

		public PCM(Dictionary<string, string> arguments) { }

		/// <summary>
		/// Play this PCM sound
		/// </summary>
		public void Play()
		{
			lock (sync_key)
			{
				if (samples == null)
					return;

				play(this);
			}
		}

		protected override void Load()
		{
			if (samples != null)
				return;

			if (DataSource == null)
			{
				Console.Error.WriteLine("Unable to load sound with no datasource");
				return;
			}

			byte[] data = DataSource();
			samples = new float[data.Length / sizeof(short)];
			for (int i = 0; i < samples.Length; i++)
				samples[i] = (float)BitConverter.ToInt16(data, i * sizeof(short)) / (float)short.MaxValue;
		}

		protected override void Unload() => samples = null;
	}
}
