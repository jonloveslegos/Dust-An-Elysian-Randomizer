using System;
using System.Collections.Generic;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Dust.Audio
{
	internal class Sound
	{
		public static AudioEngine engine;

		private static WaveBank waveBank;

		private static WaveBank enemyWaveBank;

		private static WaveBank worldBank;

		private static WaveBank uiWaveBank;

		public static SoundBank soundBank;

		private static AudioEmitter emitter = new AudioEmitter();

		private static AudioListener listener = new AudioListener();

		private static WaveBank ambienceWave;

		public static SoundBank ambienceSound;

		public static float masterMusicVolume = 1f;

		public static float masterSFXVolume = 1f;

		public static float curMusicVolume = 1f;

		public static float fadeMusic = 0f;

		public static float curGameVolume = 1f;

		public static float weatherVolume = 1f;

		private static float finalMusicVolume = 0f;

		private static float prevMusicVolume = -1f;

		public static float overrideCinematicVolume = 1f;

		public static float volumeOverride = 1.5f;

		private static List<Cue> activeCue = new List<Cue>();

		private static List<FollowCCue> followCCue = new List<FollowCCue>();

		private static List<FollowPCue> followPCue = new List<FollowPCue>();

		public static List<AmbientCue> ambientCue = new List<AmbientCue>();

		private static List<PersistCue> persistCue = new List<PersistCue>();

		private static List<InteractiveCue> interactiveCue = new List<InteractiveCue>();

		private static List<MelodicHit> melodicHit = new List<MelodicHit>();

		public static bool gameAudioUpdate;

		private static object syncObject = new object();

		public static void Initialize()
		{
			Sound.engine = new AudioEngine("Content/sfx/sfx_project.xgs");
			Sound.waveBank = new WaveBank(Sound.engine, "Content/sfx/sfx_wavs.xwb");
			Sound.enemyWaveBank = new WaveBank(Sound.engine, "Content/sfx/enemy_wavs.xwb");
			Sound.worldBank = new WaveBank(Sound.engine, "Content/sfx/world_wavs.xwb");
			Sound.uiWaveBank = new WaveBank(Sound.engine, "Content/sfx/ui_wavs.xwb");
			Sound.soundBank = new SoundBank(Sound.engine, "Content/sfx/sfx_sounds.xsb");
			Sound.ambienceWave = new WaveBank(Sound.engine, "Content/sfx/ambience_wavs.xwb");
			Sound.ambienceSound = new SoundBank(Sound.engine, "Content/sfx/ambience_sounds.xsb");
			Sound.ResetAmbience(null);
			Music.Initialize(Sound.engine);
			VoiceManager.Initialize();
			WeatherAudio.Initialize(Sound.engine);
		}

		public static void ResetCCues()
		{
			for (int i = 0; i < Sound.followCCue.Count; i++)
			{
				if (Sound.followCCue[i] != null)
				{
					Sound.followCCue[i].Stop();
				}
			}
			Sound.followCCue.Clear();
		}

		public static void ResetPCues()
		{
			for (int i = 0; i < Sound.followPCue.Count; i++)
			{
				if (Sound.followPCue[i] != null)
				{
					Sound.followPCue[i].Stop();
				}
			}
			Sound.followPCue.Clear();
		}

		public static void ResetAmbience(List<AmbientCue> newList)
		{
			for (int i = 0; i < Sound.ambientCue.Count; i++)
			{
				Sound.ambientCue[i].Stop();
			}
			Sound.ambientCue.Clear();
			if (newList != null)
			{
				for (int j = 0; j < newList.Count; j++)
				{
					Sound.ambientCue.Add(newList[j]);
				}
			}
			for (int k = 0; k < Sound.persistCue.Count; k++)
			{
				Sound.persistCue[k].Stop();
			}
			Sound.persistCue.Clear();
			for (int l = 0; l < Sound.interactiveCue.Count; l++)
			{
				Sound.interactiveCue[l].Stop();
			}
			Sound.interactiveCue.Clear();
		}

		public static void PlayCue(string cue)
		{
			Sound.soundBank.PlayCue(cue);
		}

		public static void PlayCue(string cue, Vector2 loc, float distance)
		{
			lock (Sound.syncObject)
			{
				Cue cue2 = Sound.soundBank.GetCue(cue);
				Sound.activeCue.Add(cue2);
				Vector2 vector = ((!(Game1.events.eventCamera != Vector2.Zero) && Game1.events.regionIntroStage <= 0) ? (Game1.character[0].Location - new Vector2(0f, 200f)) : ((Game1.Scroll + new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f) / Game1.worldScale));
				Sound.listener.Position = new Vector3(vector.X, vector.Y, 200f);
				Sound.emitter.Position = new Vector3(loc.X, loc.Y, 0f);
				cue2.Apply3D(Sound.listener, Sound.emitter);
				cue2.Play();
				cue2.SetVariable("Distance", distance);
			}
		}

		public static void PlayFollowCharCue(string cue, int id)
		{
			for (int i = 0; i < Sound.followCCue.Count; i++)
			{
				if (Sound.followCCue[i] != null && Sound.followCCue[i].charID == id)
				{
					Sound.followCCue[i].Stop();
					Sound.followCCue.Remove(Sound.followCCue[i]);
				}
			}
			Sound.followCCue.Add(new FollowCCue(cue, id));
		}

		public static void PlayFollowParticleCue(string cue, int id, int l)
		{
			Sound.followPCue.Add(new FollowPCue(cue, id, l));
		}

		public static void PlayPersistCue(string cue, Vector2 emitterLoc)
		{
			bool flag = true;
			for (int i = 0; i < Sound.persistCue.Count; i++)
			{
				if (Sound.persistCue[i].Name == cue)
				{
					flag = false;
				}
			}
			if (flag)
			{
				Sound.persistCue.Add(new PersistCue(cue, emitterLoc));
			}
		}

		public static void SetInteractiveStage(Vector2 loc, string cue, int stage)
		{
			int num = (int)(loc.X + loc.Y);
			bool flag = false;
			for (int i = 0; i < Sound.interactiveCue.Count; i++)
			{
				if (Sound.interactiveCue[i].segmentLayer == 0 && Sound.interactiveCue[i].segmentID == num)
				{
					Sound.interactiveCue[i].SetHazardStage(stage);
					flag = true;
				}
			}
			if (!flag)
			{
				Sound.interactiveCue.Add(new InteractiveCue(cue, 0, num, loc));
			}
		}

		public static void SetHazardStage(int l, int i, Vector2 loc, HazardSpeed hazardSpeed, int stage)
		{
			bool flag = false;
			for (int j = 0; j < Sound.interactiveCue.Count; j++)
			{
				if (Sound.interactiveCue[j].segmentLayer == l && Sound.interactiveCue[j].segmentID == i)
				{
					Sound.interactiveCue[j].SetHazardStage(stage);
					flag = true;
				}
			}
			if (!flag)
			{
				Sound.interactiveCue.Add(new InteractiveCue(Sound.GetHazardName(hazardSpeed), l, i, loc));
			}
		}

		private static string GetHazardName(HazardSpeed hazardSpeed)
		{
			switch (hazardSpeed)
			{


				case HazardSpeed.EmitRocks:
					return "hazard_rock";
				case HazardSpeed.EmitLava:
					return "hazard_lava";
			}
			return "hazard_fire";
		}

    public static void StopPersistCue(string _cue)
		{
			for (int i = 0; i < Sound.persistCue.Count; i++)
			{
				if (Sound.persistCue[i] == null)
				{
					Sound.persistCue.Remove(Sound.persistCue[i]);
				}
				else if (Sound.persistCue[i].Name == _cue)
				{
					Sound.persistCue[i].Stop();
					Sound.persistCue.Remove(Sound.persistCue[i]);
				}
			}
		}

		public static void StopFollowCharCue(int charID)
		{
			for (int i = 0; i < Sound.followCCue.Count; i++)
			{
				if (Sound.followCCue[i] == null)
				{
					Sound.followCCue.Remove(Sound.followCCue[i]);
				}
				else if (Sound.followCCue[i].charID == charID)
				{
					Sound.followCCue[i].Stop();
					Sound.followCCue.Remove(Sound.followCCue[i]);
				}
			}
		}

		public static void PlayDropCue(string cue, Vector2 loc, float dropSpeed)
		{
			lock (Sound.syncObject)
			{
				Cue cue2 = Sound.soundBank.GetCue(cue);
				Sound.activeCue.Add(cue2);
				Sound.listener.Position = new Vector3(Game1.character[0].Location.X, Game1.character[0].Location.Y, 200f);
				Sound.emitter.Position = new Vector3(loc.X, loc.Y, 0f);
				cue2.Apply3D(Sound.listener, Sound.emitter);
				cue2.Play();
				cue2.SetVariable("DropSpeed", Math.Abs(dropSpeed));
			}
		}

		public static void PlayMelodicHit(bool kill, bool fidget)
		{
			lock (Sound.syncObject)
			{
				Sound.melodicHit.Add(new MelodicHit(kill, fidget));
			}
		}

		public static void SetSFXVolume(float _sfxVolume)
		{
			Sound.masterSFXVolume = MathHelper.Clamp(_sfxVolume, 0f, 1f);
			Sound.engine.GetCategory("Instanced").SetVolume(Sound.masterSFXVolume * Sound.volumeOverride);
			Sound.engine.GetCategory("Voice").SetVolume(Sound.masterSFXVolume * 1.2f * Sound.volumeOverride);
		}

		public static void DimSFXVolume(float amount)
		{
			Sound.engine.GetCategory("Instanced").SetVolume(Sound.masterSFXVolume * amount * Sound.volumeOverride);
			Sound.engine.GetCategory("Ambience").SetVolume(Sound.masterSFXVolume * amount * Sound.volumeOverride * MathHelper.Clamp(Sound.weatherVolume, 0f, 1f));
			Sound.overrideCinematicVolume = Math.Min(amount, 1f);
		}

		public static void SetMusicVolume(float _musicVolume)
		{
			Sound.finalMusicVolume = MathHelper.Clamp(_musicVolume, 0f, 1f);
			Sound.engine.GetCategory("Music").SetVolume(Sound.finalMusicVolume * Sound.volumeOverride);
		}

		public static void FadeMusicOut(float time)
		{
			Sound.fadeMusic = time;
			if (time == 0f)
			{
				Sound.curMusicVolume = 1f;
			}
		}

		public static void FadeMusicIn(float time)
		{
			Sound.curMusicVolume = 0f;
			Sound.fadeMusic = 0f - time;
			if (time == 0f)
			{
				Sound.curMusicVolume = 1f;
			}
		}

		private static void UpdateActiveCues(Vector2 listenPos)
		{
			for (int i = 0; i < Sound.activeCue.Count; i++)
			{
				if (Sound.activeCue[i] == null)
				{
					Sound.activeCue.Remove(Sound.activeCue[i]);
				}
				else if (Sound.activeCue[i].IsStopped)
				{
					Sound.activeCue[i].Dispose();
					Sound.activeCue.Remove(Sound.activeCue[i]);
				}
			}
			for (int j = 0; j < Sound.followCCue.Count; j++)
			{
				if (Sound.followCCue[j] == null)
				{
					Sound.followCCue.Remove(Sound.followCCue[j]);
					continue;
				}
				if (Sound.followCCue[j].Playing)
				{
					Sound.followCCue[j].Update(listenPos);
					continue;
				}
				Sound.followCCue[j].Stop();
				Sound.followCCue.Remove(Sound.followCCue[j]);
			}
			for (int k = 0; k < Sound.followPCue.Count; k++)
			{
				if (Sound.followPCue[k] == null)
				{
					Sound.followPCue.Remove(Sound.followPCue[k]);
					continue;
				}
				if (Sound.followPCue[k].Playing)
				{
					Sound.followPCue[k].Update(listenPos);
					continue;
				}
				Sound.followPCue[k].Stop();
				Sound.followPCue.Remove(Sound.followPCue[k]);
			}
			for (int l = 0; l < Sound.persistCue.Count; l++)
			{
				if (Sound.persistCue[l] == null)
				{
					Sound.persistCue.Remove(Sound.persistCue[l]);
					continue;
				}
				if (Sound.persistCue[l].Playing)
				{
					Sound.persistCue[l].Update();
					continue;
				}
				Sound.persistCue[l].Stop();
				Sound.persistCue.Remove(Sound.persistCue[l]);
			}
			for (int m = 0; m < Sound.interactiveCue.Count; m++)
			{
				if (Sound.interactiveCue[m] == null)
				{
					Sound.interactiveCue.Remove(Sound.interactiveCue[m]);
				}
				else
				{
					Sound.interactiveCue[m].Update(listenPos);
				}
			}
			for (int n = 0; n < Sound.melodicHit.Count; n++)
			{
				if (Sound.melodicHit[n] == null)
				{
					Sound.melodicHit.Remove(Sound.melodicHit[n]);
					continue;
				}
				if (Sound.melodicHit[n].Playing)
				{
					Sound.melodicHit[n].Update();
					continue;
				}
				Sound.melodicHit[n].Stop();
				Sound.melodicHit.Remove(Sound.melodicHit[n]);
			}
			for (int num = 0; num < Sound.ambientCue.Count; num++)
			{
				if (Sound.ambientCue[num] == null)
				{
					Sound.ambientCue.Remove(Sound.ambientCue[num]);
				}
				else
				{
					Sound.ambientCue[num].Update(listenPos);
				}
			}
		}

		public static void Update(Character c)
		{
			Sound.engine.Update();
			if (Sound.fadeMusic > 0f)
			{
				Sound.curMusicVolume = MathHelper.Clamp(Sound.curMusicVolume - Game1.FrameTime * (1f / Sound.fadeMusic), 0f, 1f);
				if (Sound.curMusicVolume <= 0f)
				{
					Sound.fadeMusic = 0f;
					Music.Play(null);
					Sound.curMusicVolume = 1f;
				}
			}
			else if (Sound.fadeMusic < 0f)
			{
				Sound.curMusicVolume = MathHelper.Clamp(Sound.curMusicVolume + Game1.FrameTime * (1f / (0f - Sound.fadeMusic)), 0f, 1f);
				if (Sound.curMusicVolume >= 1f)
				{
					Sound.fadeMusic = 0f;
					Sound.curMusicVolume = 1f;
				}
			}
			if (!Game1.hud.isPaused && Game1.hud.inventoryState == InventoryState.None)
			{
				Sound.gameAudioUpdate = true;
			}
			else
			{
				Sound.gameAudioUpdate = false;
			}
			if (Game1.longSkipFrame != 3)
			{
				return;
			}
			Vector2 vector = ((!(Game1.events.eventCamera != Vector2.Zero) && Game1.events.regionIntroStage <= 0) ? (c.Location - new Vector2(0f, 200f)) : ((Game1.Scroll + new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f) / Game1.worldScale));
			if ((Game1.hud.isPaused && Game1.menu.menuMode != MenuMode.Settings) || Game1.hud.inventoryState != 0 || (Game1.hud.dialogueState == DialogueState.Active && Game1.hud.dialogue.topEdge < (float)Game1.screenHeight && Sound.masterSFXVolume > 0f) || Game1.hud.unlockState > 0)
			{
				Sound.finalMusicVolume = Sound.masterMusicVolume * Sound.curMusicVolume * 0.2f;
			}
			else
			{
				float num = 1f;
				if (Game1.hud.savePos != Vector2.Zero && (Game1.hud.savePos - vector).Length() < 1000f && !Game1.events.anyEvent && Game1.events.regionIntroStage == 0)
				{
					num = 1f - MathHelper.Clamp(400f / (Game1.hud.savePos - vector).Length(), 0f, 0.8f);
				}
				float num2 = 1f;
				if (Game1.stats.melodicHitCount > 24 && !Game1.hud.inBoss && Sound.masterSFXVolume > 0f)
				{
					num2 = MathHelper.Clamp((float)(64 - Game1.stats.melodicHitCount) / 64f, 0.2f, 1f);
				}
				Sound.finalMusicVolume += (Sound.masterMusicVolume * Sound.curMusicVolume * num * num2 - Sound.finalMusicVolume) * Game1.HudTime * 12f;
			}
			if (Sound.prevMusicVolume != Sound.finalMusicVolume)
			{
				Sound.SetMusicVolume(Sound.finalMusicVolume);
			}
			Sound.prevMusicVolume = Sound.finalMusicVolume;
			if (Game1.map.GetTransVal() < 0.5f)
			{
				Sound.UpdateActiveCues(vector);
			}
			Sound.engine.SetGlobalVariable("Zoom", Math.Max(Game1.worldScale * 1.2f, 0.65f));
			Sound.engine.SetGlobalVariable("Reverb", Game1.map.reverbPercent);
			if (Sound.overrideCinematicVolume == 1f)
			{
				Sound.engine.GetCategory("Ambience").SetVolume(Sound.masterSFXVolume * MathHelper.Clamp(Sound.weatherVolume, 0f, 1f));
			}
			WeatherAudio.Update();
			VoiceManager.Update();
		}
	}
}
