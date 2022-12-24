using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Dust.Audio
{
	public class VoiceCue
	{
		private Cue cue;

		public string Name;

		private Vector2 emitterLoc;

		private AudioEmitter emitter = new AudioEmitter();

		private AudioListener listener = new AudioListener();

		public bool Playing()
		{
			if (this.cue != null && this.cue.IsPlaying)
			{
				return true;
			}
			return false;
		}

		public VoiceCue(string _cue, Vector2 _emitterLoc, SoundBank voiceBank)
		{
			this.Name = _cue;
			this.emitterLoc = _emitterLoc;
			try
			{
				this.cue = voiceBank.GetCue(_cue);
				this.cue.Play();
				this.listener.Position = new Vector3(Game1.screenWidth / 2, Game1.screenHeight / 2, 0f);
				this.emitter.Position = new Vector3(this.emitterLoc.X, this.emitterLoc.Y, 0f);
				if (Game1.FrameTime == 0f)
				{
					this.cue.Pause();
				}
			}
			catch (Exception)
			{
			}
		}

		public void Stop()
		{
			try
			{
				if (this.cue.IsPlaying && !this.cue.IsDisposed)
				{
					this.cue.Stop(AudioStopOptions.Immediate);
				}
				this.cue.Dispose();
			}
			catch (Exception)
			{
			}
		}

		public void Update()
		{
			if (this.cue == null || this.cue.IsDisposed)
			{
				return;
			}
			if (Game1.FrameTime == 0f && Game1.gameMode != Game1.GameModes.WorldMap)
			{
				if (!this.cue.IsPaused)
				{
					this.cue.Pause();
				}
			}
			else if (this.cue.IsPaused)
			{
				this.cue.Resume();
			}
		}
	}
}
