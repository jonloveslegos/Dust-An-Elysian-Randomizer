using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Kush : AI
	{
		public Kush(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 15000;
			c.Strength = 240;
			c.Speed = 800f;
			c.JumpVelocity = 400f;
			c.DefaultHeight = 400;
			c.DefaultWidth = 400;
			c.ShadowWidth = 1.75f;
			c.Aggression = 2;
			c.LiftType = CanLiftType.NoLift;
			c.RandomWeapons = 2;
			c.MaxDownTime = 0f;
			c.defaultColor = new Color(1f, Rand.GetRandomFloat(0.8f, 1f), Rand.GetRandomFloat(0.8f, 1f), 1f);
			string regionName;
			if ((regionName = Game1.map.regionName) != null && regionName == "lava")
			{
				base.PrepareStats(c, 1.5f);
			}
			else
			{
				base.PrepareStats(c, 1f);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (Game1.longSkipFrame > 3)
			{
				if (base.me.DownTime > 0f)
				{
					base.me.DefaultHeight = 400;
				}
				else
				{
					base.me.DefaultHeight = 600;
				}
				if (base.me.DyingFrame == -1f && Rand.GetRandomInt(0, 50) == 0)
				{
					Sound.PlayCue("kush_idle", base.me.Location, (c[0].Location - base.me.Location).Length() / 2f);
				}
			}
			if (base.jobFrame < 0f)
			{
				base.jobType = JobType.MeleeChaseLunging;
				base.jobFrame = Rand.GetRandomFloat(2f, 4f);
			}
			base.Update(c, ID, map);
		}
	}
}
