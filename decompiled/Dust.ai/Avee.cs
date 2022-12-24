using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Avee : AI
	{
		public Avee(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 160;
			c.Speed = 800f;
			c.JumpVelocity = 1400f;
			c.Strength = 30;
			c.DefaultHeight = 160;
			c.DefaultWidth = 160;
			c.RandomTextures = 2;
			c.FlyType = FlyingType.CanFlySwaying;
			c.maxAnimFrames = 8;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), 1f);
			string regionName;
			if ((regionName = Game1.map.regionName) != null && regionName == "trial")
			{
				base.PrepareStats(c, 2f);
			}
			else
			{
				base.PrepareStats(c, 1f);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (c[ID].DyingFrame == -1f && Rand.GetRandomInt(0, 300) == 0)
			{
				Sound.PlayCue("avee_chirp", c[ID].Location, (c[0].Location - c[ID].Location).Length());
			}
			if (base.jobFrame < 0f)
			{
				base.randomInt = Rand.GetRandomInt(100, 500);
				base.jobType = JobType.FlyMelee;
				base.jobFrame = Rand.GetRandomFloat(2f, 6f);
			}
			base.Update(c, ID, map);
		}
	}
}
