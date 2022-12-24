using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Psylph : AI
	{
		public Psylph(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 4000;
			c.Strength = 250;
			c.Speed = 500f;
			c.JumpVelocity = 1600f;
			c.DefaultHeight = 180;
			c.DefaultWidth = 180;
			c.ShadowWidth = 1f;
			c.Aggression = 40;
			c.maxAnimFrames = 3;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.6f, 1f), Rand.GetRandomFloat(0.7f, 1f), Rand.GetRandomFloat(0.7f, 1f), 1f);
			string regionName;
			if ((regionName = Game1.map.regionName) != null && regionName == "trial")
			{
				base.PrepareStats(c, 0.2f);
			}
			else
			{
				base.PrepareStats(c, 1f);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.DyingFrame == -1f && Rand.GetRandomInt(0, 300) == 0)
			{
				Sound.PlayCue("psylph_hiss", base.me.Location, (c[0].Location - base.me.Location).Length());
			}
			if (base.me.AnimName == "attack01" && base.me.AnimFrame > 5 && Game1.longSkipFrame > 2)
			{
				Sound.PlayCue("metallic_swing", base.me.Location, (c[0].Location - base.me.Location).Length() / 2f);
			}
			if (base.DodgeAttack(c, base.targ, 400f, 1000f, base.me.JumpVelocity / 2f, faceTarg: true, base.me.Aggression))
			{
				Sound.PlayCue("psylph_dodge", base.me.Location, (c[0].Location - base.me.Location).Length());
			}
			if (base.jobFrame < 0f)
			{
				float randomFloat = Rand.GetRandomFloat(0f, 1f);
				if (randomFloat < 0.4f)
				{
					base.jobType = JobType.MeleeChaseLunging;
					base.jobFrame = Rand.GetRandomFloat(2f, 4f);
				}
				else if (randomFloat < 0.7f)
				{
					base.FaceToTarg(c);
					base.jobType = JobType.Idle;
					base.jobFrame = Rand.GetRandomFloat(0.5f, 2f);
				}
				else
				{
					base.jobType = JobType.Avoid;
					base.jobFrame = Rand.GetRandomFloat(0.2f, 1f);
					base.FaceToTarg(c);
				}
			}
			base.Update(c, ID, map);
			if (base.targ > -1 && c[base.targ].Location.Y < base.me.Location.Y - 100f && base.me.State == CharState.Grounded && base.CanAttack() && Rand.GetRandomInt(0, 20) == 0)
			{
				base.FaceTarg(c);
				base.me.KeySecondary = true;
			}
		}
	}
}
