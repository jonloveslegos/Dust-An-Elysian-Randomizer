using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Dust.Audio
{
	public class PersistCue
	{
		private Cue cue;

		public string Name;

		private Vector2 emitterLoc;

		private AudioEmitter emitter = new AudioEmitter();

		private AudioListener listener = new AudioListener();

		public bool Playing => this.cue.IsPlaying;

		public PersistCue(string _cue, Vector2 _emitterLoc)
		{
			this.Name = _cue;
			this.emitterLoc = _emitterLoc;
			try
			{
				this.cue = Sound.soundBank.GetCue(_cue);
			}
			catch
			{
				this.cue = Sound.ambienceSound.GetCue(_cue);
			}
			this.cue.Apply3D(this.listener, this.emitter);
			this.cue.Play();
			this.listener.Position = new Vector3(Game1.screenWidth / 2, Game1.screenHeight / 2, 0f);
			this.emitter.Position = new Vector3(this.emitterLoc.X, this.emitterLoc.Y, 0f);
			this.cue.Apply3D(this.listener, this.emitter);
			if (Game1.FrameTime == 0f)
			{
				this.cue.Pause();
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
