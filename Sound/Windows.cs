using System.IO;
using System.Media;

namespace PiGameSharp.Sound
{
	internal static class Windows
	{
		internal static void Play(PCM item)
		{
			int datalength = item.samples.Length * sizeof(short);

			using (MemoryStream ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				bw.Write(0x46464952); //RIFF
				bw.Write(36 + datalength);
				bw.Write(0x45564157); //WAVE

				bw.Write(0x20746d66); //fmt_
				bw.Write(16);
				bw.Write((ushort)1); //PCM
				bw.Write((ushort)1); //mono
				bw.Write(48000);     //sample rate
				bw.Write(48000 * 1 * 16 / 8); //byte rate
				bw.Write((ushort)(1 * 16 /8)); //block align
				bw.Write((ushort)16); //16 bits

				bw.Write(0x61746164); //data
				bw.Write(datalength);

				for (int i = 0; i < item.samples.Length; i++)
					bw.Write((short)(item.samples[i] * (float)short.MaxValue));
				bw.Flush();
				ms.Position = 0;

				new SoundPlayer(ms).Play();
			}
		}
	}
}
