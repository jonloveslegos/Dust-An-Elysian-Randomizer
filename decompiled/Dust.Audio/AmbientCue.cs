using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Dust.Audio
{
	public class AmbientCue
	{
		private Cue cue;

		private Rectangle cueRect;

		private bool stereo;

		private int maxDistance = Rand.GetRandomInt(3000, 4000);

		private AudioEmitter emitter = new AudioEmitter();

		private AudioListener listener = new AudioListener();

		public AmbientCue(string _cue, Rectangle rect, bool _stereo)
		{
			this.cueRect = rect;
			this.stereo = _stereo;
			this.cue = Sound.ambienceSound.GetCue(_cue);
			if (this.stereo)
			{
				this.cue.Apply3D(this.listener, this.emitter);
			}
			this.cue.Play();
			this.cue.Pause();
		}

		public string GetCueName()
		{
			return this.cue.Name;
		}

		public void Stop()
		{
			this.cue.Stop(AudioStopOptions.AsAuthored);
		}

		public void Update(Vector2 listenPos)
		{
			Vector2 vector = new Vector2(MathHelper.Clamp(listenPos.X, this.cueRect.X, this.cueRect.X + this.cueRect.Width), MathHelper.Clamp(listenPos.Y, this.cueRect.Y, this.cueRect.Y + this.cueRect.Height));
			float num = Math.Abs(listenPos.Length() - vector.Length());
			if (num > (float)this.maxDistance || Game1.FrameTime == 0f)
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
			if (this.stereo)
			{
				this.listener.Position = new Vector3(listenPos.X, listenPos.Y, 200f);
				this.emitter.Position = new Vector3(vector.X, vector.Y, 0f);
				this.cue.Apply3D(this.listener, this.emitter);
			}
		}
	}
}
