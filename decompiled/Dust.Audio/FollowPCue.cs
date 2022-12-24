using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Dust.Audio
{
	public class FollowPCue
	{
		private Cue cue;

		private int particleID;

		private byte particleLayer;

		private Vector2 emitterLoc;

		private AudioEmitter emitter = new AudioEmitter();

		private AudioListener listener = new AudioListener();

		public bool Playing => this.cue.IsPlaying;

		public FollowPCue(string _cue, int id, int l)
		{
			this.particleID = id;
			this.particleLayer = (byte)l;
			this.EmitterExists();
			this.cue = Sound.soundBank.GetCue(_cue);
			this.listener.Position = new Vector3(0f, 0f, 0f);
			this.emitter.Position = new Vector3(10000f, 10000f, 0f);
			this.cue.Apply3D(this.listener, this.emitter);
			this.cue.Play();
			if (Game1.FrameTime == 0f)
			{
				this.cue.Pause();
			}
		}

		public void Stop()
		{
			if (!this.cue.IsDisposed)
			{
				this.cue.Stop(AudioStopOptions.Immediate);
			}
			this.cue.Dispose();
		}

		private bool EmitterExists()
		{
			Vector2 traj = Vector2.Zero;
			bool exists = false;
			Game1.pManager.GetParticleAudio(this.particleLayer, this.particleID, ref this.emitterLoc, ref traj, ref exists);
			if (exists)
			{
				return true;
			}
			return false;
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
			if (this.EmitterExists())
			{
				this.listener.Position = new Vector3(listenPos.X, listenPos.Y, 200f);
				this.emitter.Position = new Vector3(this.emitterLoc.X, this.emitterLoc.Y, 0f);
				this.cue.Apply3D(this.listener, this.emitter);
			}
			else if (this.cue.IsPlaying)
			{
				this.Stop();
			}
		}
	}
}
