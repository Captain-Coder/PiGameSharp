using System;
using System.IO;

namespace PiGameSharp.Sound
{
	public class PCM : Resource
	{
		internal static Action<PCM> play;

		private Func<Stream> source;
		internal float[] samples;

		public PCM(Func<Stream> datasource) => source = datasource ?? throw new ArgumentNullException("datasource");

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

			using (Stream data = source())
			{
				BinaryReader br = new BinaryReader(data);
				samples = new float[data.Length / sizeof(short)];
				for (int i = 0; i < samples.Length; i++)
					samples[i] = (float)br.ReadInt16() / (float)short.MaxValue;
			}
		}

		protected override void Unload() => samples = null;
	}
}
