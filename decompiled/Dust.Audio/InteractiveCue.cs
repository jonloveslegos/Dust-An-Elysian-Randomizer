using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Dust.Audio
{
	public class InteractiveCue
	{
		private Cue cue;

		public int segmentID;

		public byte segmentLayer;

		private AudioEmitter emitter = new AudioEmitter();

		private AudioListener listener = new AudioListener();

		public InteractiveCue(string _cue, int l, int i, Vector2 emitterLoc)
		{
			this.segmentID = i;
			this.segmentLayer = (byte)l;
			this.emitter.Position = new Vector3(emitterLoc.X, emitterLoc.Y, 0f);
			this.cue = Sound.ambienceSound.GetCue(_cue);
			this.cue.Apply3D(this.listener, this.emitter);
			this.cue.Play();
			if (Game1.FrameTime == 0f)
			{
				this.cue.Pause();
			}
		}

		public void Stop()
		{
			try
			{
				if (!this.cue.IsDisposed)
				{
					this.cue.Stop(AudioStopOptions.Immediate);
				}
				this.cue.Dispose();
			}
			catch (Exception)
			{
			}
		}

		public void SetHazardStage(int stage)
		{
			this.cue.SetVariable("Interactive", stage);
		}

		public void Update(Vector2 listenPos)
		{
			if (Game1.FrameTime == 0f)
			{
				if (!this.cue.IsPaused)
				{
					this.cue.Pause();
				}
				return;
			}
			if (this.cue.IsPaused)
			{
				this.cue.Resume();
			}
			this.listener.Position = new Vector3(listenPos.X, listenPos.Y, 200f);
			this.cue.Apply3D(this.listener, this.emitter);
		}
	}
}
