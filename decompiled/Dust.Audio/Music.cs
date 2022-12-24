using System;
using Dust.HUD;
using Microsoft.Xna.Framework.Audio;

namespace Dust.Audio
{
	internal static class Music
	{
		private static WaveBank musicWave;

		private static SoundBank musicSound;

		private static Cue musicCue;

		private static string musicString;

		private static string prevMusic;

		public static string PrevMusic
		{
			get
			{
				return Music.prevMusic;
			}
			set
			{
				Music.prevMusic = value;
			}
		}

		public static void Initialize(AudioEngine engine)
		{
			Music.musicWave = new WaveBank(engine, "Content/sfx/music_wavs.xwb", 0, 16);
			Music.musicSound = new SoundBank(engine, "Content/sfx/music_sounds.xsb");
		}

		public static string GetMusicName()
		{
			if (Music.musicString == null)
			{
				return "silent";
			}
			return Music.musicString;
		}

		public static void Play(string _musicString, float fadeIn)
		{
			Music.Play(_musicString);
			if (Music.musicString != null && Music.musicCue.IsPlaying)
			{
				Sound.FadeMusicIn(fadeIn);
			}
		}

		public static void Play(string _musicString)
		{
			Sound.FadeMusicOut(0f);
			if (_musicString == "silent")
			{
				_musicString = null;
			}
			if (!(Music.musicString != _musicString))
			{
				return;
			}
			Music.musicString = _musicString;
			if (Music.musicCue != null)
			{
				Music.musicCue.Dispose();
			}
			if (Music.musicString != null)
			{
				try
				{
					Music.musicCue = Music.musicSound.GetCue(Music.musicString);
					Music.musicCue.Play();
				}
				catch (Exception)
				{
				}
			}
		}

		public static void Update()
		{
			if (Music.musicString == null)
			{
				return;
			}
			if (Game1.hud.isPaused && Game1.hud.inventoryState == InventoryState.None)
			{
				if (Music.musicCue.IsPlaying)
				{
					Music.musicCue.Pause();
				}
			}
			else if (Music.musicCue.IsPaused)
			{
				Music.musicCue.Resume();
			}
		}
	}
}
