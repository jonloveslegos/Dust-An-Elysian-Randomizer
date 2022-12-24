using System;
using Dust.Dialogue;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Dust.Audio
{
	internal class VoiceManager
	{
		private static VisualizationData visualizationData;

		private static WaveBank voiceWaveBank;

		private static SoundBank voiceBank;

		public static VoiceCue voiceCue;

		private static string currentCue;

		private static string resourceName;

		private static string currentBankName;

		private static bool isLoading;

		public static void Initialize()
		{
			VoiceManager.visualizationData = new VisualizationData();
			MediaPlayer.IsVisualizationEnabled = true;
			MediaPlayer.IsRepeating = false;
		}

		public static bool PlayVoice(Song _voiceTrack)
		{
			if (_voiceTrack != null && !_voiceTrack.IsDisposed)
			{
				MediaPlayer.Play(_voiceTrack);
				MediaPlayer.Volume = Sound.masterSFXVolume * Sound.volumeOverride * 0.5f;
				if (Game1.VoiceNotReady && Game1.cutscene.SceneType == CutsceneType.None)
				{
					MediaPlayer.Volume = 0f;
				}
				return true;
			}
			MediaPlayer.Stop();
			return false;
		}

		public static void PauseVoice()
		{
			if (MediaPlayer.State == MediaState.Playing)
			{
				MediaPlayer.Pause();
			}
		}

		public static void ResumeVoice()
		{
			if (MediaPlayer.State == MediaState.Paused)
			{
				MediaPlayer.Resume();
			}
		}

		public static void StopVoice()
		{
			MediaPlayer.Stop();
		}

		public static bool CheckSpeaking()
		{
			if (MediaPlayer.State == MediaState.Playing)
			{
				return true;
			}
			return false;
		}

		public static byte UpdateSpeaking()
		{
			if (MediaPlayer.State == MediaState.Playing)
			{
				MediaPlayer.GetVisualizationData(VoiceManager.visualizationData);
				int count = VoiceManager.visualizationData.Frequencies.Count;
				for (int i = 0; i < count; i++)
				{
					if (VoiceManager.visualizationData.Frequencies[i] > 0.3f)
					{
						return 2;
					}
				}
				return 1;
			}
			return 0;
		}

		public static void PlayVoice(string cue, string _resourceName, bool loadingOnly)
		{
			VoiceManager.currentCue = cue;
			VoiceManager.resourceName = _resourceName;
			if (loadingOnly)
			{
				if (VoiceManager.ReloadVoice())
				{
					Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(SetVoice)));
				}
			}
			else if (VoiceManager.ReloadVoice())
			{
				VoiceManager.isLoading = true;
				Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(SetVoice), new TaskFinishedDelegate(LoadingFinished)));
			}
			else
			{
				VoiceManager.PlayCue();
			}
		}

		public static void LoadVoiceNoThread(string cue, string _resourceName)
		{
			VoiceManager.currentCue = cue;
			VoiceManager.resourceName = _resourceName;
			if (VoiceManager.ReloadVoice())
			{
				VoiceManager.SetVoice();
			}
		}

		private static bool ReloadVoice()
		{
			if (VoiceManager.voiceBank == null || VoiceManager.voiceBank.IsDisposed || VoiceManager.voiceWaveBank == null || VoiceManager.voiceWaveBank.IsDisposed || VoiceManager.currentBankName != VoiceManager.GetBankName())
			{
				return true;
			}
			return false;
		}

		private static void SetVoice()
		{
			VoiceManager.UnloadVoice();
			VoiceManager.currentBankName = VoiceManager.GetBankName();
			VoiceManager.voiceWaveBank = new WaveBank(Sound.engine, "Content/sfx/voice_wavs_" + VoiceManager.currentBankName + ".xwb");
			VoiceManager.voiceBank = new SoundBank(Sound.engine, "Content/sfx/voice_sounds_" + VoiceManager.currentBankName + ".xsb");
		}

		private static void LoadingFinished(int taskId)
		{
			VoiceManager.PlayCue();
		}

		public static void UnloadVoice()
		{
			VoiceManager.StopVoice();
			if (VoiceManager.voiceBank != null)
			{
				VoiceManager.voiceBank.Dispose();
			}
			if (VoiceManager.voiceWaveBank != null)
			{
				VoiceManager.voiceWaveBank.Dispose();
			}
		}

		private static string GetBankName()
		{
			if (VoiceManager.currentCue.StartsWith("events"))
			{
				string[] array = VoiceManager.currentCue.Split('_');
				int num = 0;
				if (array.Length > 1)
				{
					num = Convert.ToInt32(array[1]);
				}
				string text = ((num < 40) ? "000" : ((num < 150) ? "040" : ((num < 500) ? "150" : ((num >= 700) ? "700" : "500"))));
				return "events_" + text;
			}
			int num2 = 0;
			switch (VoiceManager.resourceName)
			{
			case "avgustin":
			case "bean":
			case "blop":
				num2 = 0;
				break;
			case "bopo":
				num2 = 1;
				break;
			case "bram":
				num2 = 2;
				break;
			case "calum":
			case "colleen":
			case "cora":
				num2 = 3;
				break;
			case "corbin":
			case "elder":
				num2 = 4;
				break;
			case "fale":
			case "flohop":
			case "geehan":
			case "gianni":
				num2 = 5;
				break;
			case "ginger":
				num2 = 6;
				break;
			case "haley":
			case "kane":
				num2 = 7;
				break;
			case "lady":
			case "mamop":
			case "matti":
			case "moska":
				num2 = 8;
				break;
			case "oldgappy":
			case "oneida":
			case "reed":
				num2 = 9;
				break;
			case "sanjin":
			case "sarahi":
			case "shopaurora":
				num2 = 10;
				break;
			case "shopwild":
			case "smobop":
				num2 = 11;
				break;
			}
			return "npc_" + $"{num2:D2}";
		}

		private static void PlayCue()
		{
			Vector2 emitterLoc = new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f;
			if (VoiceManager.voiceCue != null)
			{
				VoiceManager.voiceCue.Stop();
			}
			VoiceManager.voiceCue = new VoiceCue(VoiceManager.currentCue, emitterLoc, VoiceManager.voiceBank);
			VoiceManager.isLoading = false;
		}

		public static void StopVoiceCue()
		{
			if (VoiceManager.voiceCue != null)
			{
				VoiceManager.voiceCue.Stop();
			}
			VoiceManager.isLoading = false;
		}

		public static byte VoiceIsPlaying()
		{
			if (VoiceManager.isLoading)
			{
				return 1;
			}
			if (VoiceManager.voiceCue != null && VoiceManager.voiceCue.Playing())
			{
				return 2;
			}
			return 0;
		}

		public static void Update()
		{
			if (VoiceManager.voiceCue != null && !VoiceManager.isLoading)
			{
				if (VoiceManager.voiceBank == null || VoiceManager.voiceBank.IsDisposed || VoiceManager.voiceWaveBank == null || VoiceManager.voiceWaveBank.IsDisposed)
				{
					VoiceManager.StopVoiceCue();
				}
				else
				{
					VoiceManager.voiceCue.Update();
				}
			}
		}
	}
}
