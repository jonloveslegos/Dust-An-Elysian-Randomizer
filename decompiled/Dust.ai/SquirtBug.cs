using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class SquirtBug : AI
	{
		public SquirtBug(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 160;
			c.Speed = 60f;
			c.JumpVelocity = 1400f;
			c.Strength = 50;
			c.DefaultHeight = 180;
			c.DefaultWidth = 180;
			c.RandomTextures = 2;
			c.FlyType = FlyingType.CanFlySwaying;
			c.maxAnimFrames = 26;
			c.MaskGlow = 1.5f;
			c.defaultColor = new Color(1f, 1f, Rand.GetRandomFloat(0.5f, 1f), 1f);
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.DyingFrame == -1f && Rand.GetRandomInt(0, 200) == 0)
			{
				Sound.PlayCue("squirtbug_idle", base.me.Location, (c[0].Location - base.me.Location).Length());
			}
			if (base.me.renderable && Game1.longSkipFrame > 3)
			{
				Game1.pManager.AddGlowSpark(base.me.Location + Rand.GetRandomVector2(-80f, 80f, -200f, -100f), new Vector2(0f, Rand.GetRandomFloat(4f, 40f)), Rand.GetRandomFloat(0.5f, 2f), 1f, 1f, 6);
			}
			if (base.jobFrame < 0f)
			{
				float randomFloat = Rand.GetRandomFloat(0f, 1f);
				if (randomFloat < 0.2f)
				{
					base.randomInt = Rand.GetRandomInt(100, 500);
					base.jobType = JobType.FlyShoot;
					base.jobFrame = Rand.GetRandomFloat(1f, 2f);
				}
				else
				{
					base.jobType = JobType.DartAround;
					base.FaceToTarg(c);
				}
			}
			base.Update(c, ID, map);
		}
	}
}
