using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace PiGameSharp.Sound
{
	internal static class ALSA
	{
		[DllImport("libasound")] static extern int snd_pcm_open (ref IntPtr handle, string pcm_name, int stream, int mode); 
		[DllImport("libasound")] static extern int snd_pcm_close (IntPtr handle); 
		[DllImport("libasound")] static extern int snd_pcm_drain (IntPtr handle); 
		[DllImport("libasound")] static extern int snd_pcm_writei (IntPtr handle, byte[] buf, int size); 
		[DllImport("libasound")] static extern int snd_pcm_set_params (IntPtr handle, int format, int access, int channels, int rate, int soft_resample, int latency); 
		[DllImport("libasound")] static extern int snd_pcm_prepare (IntPtr handle); 
		[DllImport("libasound")] static extern int snd_pcm_hw_params (IntPtr handle, IntPtr param); 
		[DllImport("libasound")] static extern int snd_pcm_hw_params_malloc (ref IntPtr param); 
		[DllImport("libasound")] static extern void snd_pcm_hw_params_free (IntPtr param);
		[DllImport("libasound")] static extern int snd_pcm_sw_params(IntPtr handle, IntPtr param);
		[DllImport("libasound")] static extern int snd_pcm_sw_params_malloc(ref IntPtr param);
		[DllImport("libasound")] static extern void snd_pcm_sw_params_free(IntPtr param);
		[DllImport("libasound")] static extern int snd_pcm_sw_params_current(IntPtr handle, IntPtr param);
		[DllImport("libasound")] static extern int snd_pcm_sw_params_set_start_threshold(IntPtr handle, IntPtr param, int threshold);
		[DllImport("libasound")] static extern int snd_pcm_hw_params_any (IntPtr handle, IntPtr param); 
		[DllImport("libasound")] static extern int snd_pcm_hw_params_set_access (IntPtr handle, IntPtr param, int access); 
		[DllImport("libasound")] static extern int snd_pcm_hw_params_set_format (IntPtr handle, IntPtr param, int format); 
		[DllImport("libasound")] static extern int snd_pcm_hw_params_set_channels (IntPtr handle, IntPtr param, int channel); 
		[DllImport("libasound")] static extern int snd_pcm_hw_params_set_rate(IntPtr handle, IntPtr param, int rate, int dir);
		[DllImport("libasound")] static extern int snd_pcm_hw_params_set_period_size(IntPtr handle, IntPtr param, int frames_per_period, int dir);
		[DllImport("libasound")] static extern int snd_pcm_hw_params_set_buffer_size (IntPtr handle, IntPtr param, int frames);

		private static Handle device_handle;
		private static Handle hardware_params;
		private static Handle software_params;
		private static Thread playback;
		private static List<float> sfx_buffer = null;
		private static int channels;
		private static int framelength;
		private static int periodlength;

		/// <summary>
		/// Performance counter to measure alsa write speed
		/// </summary>
		public readonly static PerformanceCounter sample_counter = new PerformanceCounter("ALSA.Playback()")
		{
			Unit = "Samples",
			SampleInterval = TimeSpan.FromSeconds(10)
		};

		private static string snd_strerror(int err)
		{
			if (err < 0)
				err = -err;
			if (err == 32)
				return "E PIPE";
			else if (err == 77)
				return "E BADFD";
			else
				return err.ToString();
		}

		internal static void Init()
		{
			int err;
			IntPtr handle = IntPtr.Zero;

			if (device_handle != null)
				return;

			if ((err = snd_pcm_open(ref handle, "default", 0, 0)) < 0)
				throw new Exception("Unable to open sound device: " + snd_strerror(err));
			device_handle = new Handle("ALSA device", handle, delegate(IntPtr h) { snd_pcm_drain(h); snd_pcm_close(h); });

			handle = IntPtr.Zero;
			if ((err = snd_pcm_hw_params_malloc(ref handle)) < 0)
				throw new Exception("Unable to allocate hw params: " + snd_strerror(err));
			hardware_params = new Handle("ALSA HW Params", handle, snd_pcm_hw_params_free);
			
			if ((err = snd_pcm_hw_params_any(device_handle, hardware_params)) < 0)
				throw new Exception("Unable to fill hw params: " + snd_strerror(err));

			if ((err = snd_pcm_hw_params_set_access(device_handle, hardware_params, 3)) < 0)
				throw new Exception("Unable to set access: " + snd_strerror(err));

			if ((err = snd_pcm_hw_params_set_format(device_handle, hardware_params, 2)) < 0)
				throw new Exception("Unable to set format: " + snd_strerror(err));

			int dir = 0;
			int rate = 48000;
			if ((err = snd_pcm_hw_params_set_rate(device_handle, hardware_params, rate, dir)) < 0)
				throw new Exception("Cannot set sample rate: " + snd_strerror(err));

			channels = 1;
			if ((err = snd_pcm_hw_params_set_channels(device_handle, hardware_params, channels)) < 0)
				throw new Exception("Cannot set channels to mono: " + snd_strerror(err));
			
			framelength = channels * sizeof(short);
			periodlength = 1024;

			if ((err = snd_pcm_hw_params_set_period_size(device_handle, hardware_params, periodlength, 0)) < 0)
				throw new Exception("Cannot set period size: " + snd_strerror(err));

			if ((err = snd_pcm_hw_params_set_buffer_size(device_handle, hardware_params, periodlength * 4)) < 0)
				throw new Exception("Cannot set buffer size: " + snd_strerror(err));

			if ((err = snd_pcm_hw_params(device_handle, hardware_params)) < 0)
				throw new Exception("Cannot apply hardware settings: " + snd_strerror(err));

			hardware_params.Dispose();
			hardware_params = null;

			if ((err = snd_pcm_sw_params_malloc(ref handle)) < 0)
				throw new Exception("Unable to allocate sw params: " + snd_strerror(err));
			software_params = new Handle("ALSA SW Params", handle, snd_pcm_sw_params_free);

			if ((err = snd_pcm_sw_params_current(device_handle, software_params)) < 0)
				throw new Exception("Unable to fill sw params: " + snd_strerror(err));

			if ((err = snd_pcm_sw_params_set_start_threshold(device_handle, software_params, 0)) < 0)
				throw new Exception("Unable to set start mode: " + snd_strerror(err));

			if ((err = snd_pcm_sw_params(device_handle, software_params)) < 0)
				throw new Exception("Unable to apply sw params: " + snd_strerror(err));

			software_params.Dispose();
			software_params = null;

			playback = new Thread(StartPlayback)
			{
				Priority = ThreadPriority.Highest
			};
			playback.Start();
		}

		private static void StartPlayback()
		{
			byte[] data = new byte[periodlength * framelength];
			byte[] clear = new byte[periodlength * framelength];
			int samples = 0;
			int err;

			Handle h = device_handle;
			if (h == null)
				return;
			if ((err = snd_pcm_prepare(h)) < 0)
				throw new Exception("Unable to prepare device " + snd_strerror(err));
			while (true)
			{
				lock (h)
				{

					if (sfx_buffer == null || sfx_buffer.Count == 0)
					{
						sfx_buffer = null;
						samples = snd_pcm_writei(h, clear, clear.Length / framelength);
					}
					else
					{
						samples = Math.Min(data.Length / framelength, sfx_buffer.Count / channels);
						for (int i = 0; i < samples; i++)
						{
							short sample = (short)(sfx_buffer[i] * (float)short.MaxValue);
							data[i << 1] = (byte)(sample & 0xff);
							data[1 + (i << 1)] = (byte)((sample >> 8) & 0xff);
						}
						sfx_buffer.RemoveRange(0, samples);

						samples = snd_pcm_writei(h, data, samples);
					}
					if (samples == -32) //EPIPE
					{
						if ((err = snd_pcm_prepare(h)) < 0)
							throw new Exception("Unable to prepare device after underrun " + snd_strerror(err));
					}
					else if (samples == -77) //EBADFD when the sound device is closed
						break;
					else if (samples < 0)
						throw new Exception("Unable to write frame " + snd_strerror(samples));
					else
						sample_counter.Add(samples);
				}
				// Without this wait, the sample writer gets starved for lock(h). (Since playback thread is high prio.) And playing a sound can be delayed for 10's of seconds.
				Thread.Sleep(10);  //TODO: This is a cheap, workable solution but not ideal. Revisit this with later resource abstraction refactoring. Mixing could happen in this loop?
			}
		}

		internal static void Play(PCM item)
		{
			Handle h = device_handle;
			if (h == null)
				return;
			lock (h)
			{
				if (sfx_buffer == null)
					sfx_buffer = new List<float>();
				for (int i = 0; i < item.samples.Length; i++)
				{
					if (sfx_buffer.Count <= i)
						sfx_buffer.Add(item.samples[i]);
					else
						sfx_buffer[i] += item.samples[i];
				}
			}
		}

		internal static void Deinit()
		{
			Handle h = device_handle;
			if (h == null)
				return;

			lock (h)
			{
				sfx_buffer = null;
				h.Dispose();
			}
			device_handle = null;

			if (playback != null)
			{
				playback.Join();
				playback = null;
			}
		}
	}
}
