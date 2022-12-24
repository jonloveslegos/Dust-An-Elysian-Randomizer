using Dust.Vibration;
using Microsoft.Xna.Framework.Audio;

namespace Dust.Audio
{
	public class MelodicHit
	{
		private Cue cue;

		private byte timer;

		public bool Playing => this.cue.IsPlaying;

		public MelodicHit(bool kill, bool fidget)
		{
			int num = -1;
			if (Game1.stats.melodicHitCount > -1)
			{
				num = Game1.stats.melodicHitCount - Game1.stats.melodicHitCount / 12 * 12;
			}
			if (num > -1 && kill)
			{
				num = 99;
			}
			else
			{
				Game1.stats.melodicHitCount++;
			}
			this.timer = 35;
			int setVariable = -1;
			this.cue = Sound.soundBank.GetCue(this.GetMelodicHit(num, ref Game1.stats.melodicHitTimer, ref setVariable, fidget));
			if (setVariable > -1)
			{
				this.cue.SetVariable("Interactive", num);
			}
			this.cue.Play();
		}

		public void Stop()
		{
			if (!this.cue.IsDisposed)
			{
				this.cue.Stop(AudioStopOptions.Immediate);
			}
			this.cue.Dispose();
		}

		private string GetMelodicHit(int stage, ref float duration, ref int setVariable, bool fidget)
		{
			switch (stage)
			{
			case -1:
				duration = 0.4f;
				return "melodic_hit_intro";
			case 99:
				duration = 0.25f;
				return "melodic_hit_kill";
			default:
				duration = 0.2f;
				setVariable = stage;
				if (Game1.stats.melodicHitCount < 24)
				{
					return "melodic_hit_light";
				}
				if (Game1.stats.melodicHitCount % 24 == 0)
				{
					duration = 1.5f;
					VibrationManager.SetBlast(1.5f, Game1.character[0].Location);
					this.cue = Sound.soundBank.GetCue("melodic_hit_transition");
					this.cue.Play();
					return "melodic_hit";
				}
				Game1.stats.UpdateMelodicTimer();
				if (fidget && Game1.stats.melodicHitCount > 24)
				{
					duration = 0.12f;
					return "melodic_hit_fidget";
				}
				VibrationManager.SetBlast(0.5f, Game1.character[0].Location);
				return "melodic_hit";
			}
		}

		public void Update()
		{
			this.timer--;
			if (this.timer < 1)
			{
				this.Stop();
			}
		}
	}
}
