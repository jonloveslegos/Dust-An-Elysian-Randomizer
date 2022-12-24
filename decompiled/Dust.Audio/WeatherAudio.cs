using Dust.MapClasses;
using Microsoft.Xna.Framework.Audio;

namespace Dust.Audio
{
	internal static class WeatherAudio
	{
		private static WaveBank weatherWave;

		private static SoundBank weatherSound;

		private static Cue weatherCue;

		private static string weatherString;

		public static void Initialize(AudioEngine engine)
		{
			WeatherAudio.weatherWave = new WaveBank(engine, "Content/sfx/weather_wavs.xwb", 0, 16);
			WeatherAudio.weatherSound = new SoundBank(engine, "Content/sfx/weather_sounds.xsb");
			WeatherAudio.BeginWeatherSound("weather");
		}

		public static void Play(WeatherAudioType weather)
		{
			bool flag = false;
			for (int i = 0; i < Game1.map.weatherRegion.Count; i++)
			{
				if (Game1.map.weatherRegion[i] != null && Game1.map.weatherRegion[i].Region.Contains((int)Game1.character[0].Location.X, (int)Game1.character[0].Location.Y))
				{
					flag = true;
				}
			}
			if (!flag || !(Game1.map.GetTransVal() > 0f))
			{
				Game1.wManager.weatherAudioType = weather;
				WeatherAudio.BeginWeatherSound("weather");
				WeatherAudio.weatherCue.SetVariable("WeatherType", (float)weather);
			}
		}

		public static void Stop()
		{
			Game1.wManager.weatherAudioType = WeatherAudioType.Silent;
			WeatherAudio.weatherCue.Stop(AudioStopOptions.Immediate);
			WeatherAudio.weatherString = null;
		}

		public static void BeginWeatherSound(string _weatherString)
		{
			if (WeatherAudio.weatherString != _weatherString)
			{
				WeatherAudio.weatherString = _weatherString;
				if (WeatherAudio.weatherCue != null)
				{
					WeatherAudio.weatherCue.Dispose();
				}
				if (WeatherAudio.weatherString != null)
				{
					WeatherAudio.weatherCue = WeatherAudio.weatherSound.GetCue(WeatherAudio.weatherString);
					WeatherAudio.weatherCue.Play();
				}
			}
		}

		public static void Update()
		{
			if (WeatherAudio.weatherString == null)
			{
				return;
			}
			if (!Sound.gameAudioUpdate)
			{
				if (WeatherAudio.weatherCue.IsPlaying && !WeatherAudio.weatherCue.IsPaused)
				{
					WeatherAudio.weatherCue.Pause();
				}
			}
			else if (WeatherAudio.weatherCue.IsPaused)
			{
				WeatherAudio.weatherCue.Resume();
			}
			if (!WeatherAudio.weatherCue.IsPlaying)
			{
				WeatherAudio.BeginWeatherSound("weather");
			}
		}
	}
}
