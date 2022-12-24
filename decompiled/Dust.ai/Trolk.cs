using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Trolk : AI
	{
		public Trolk(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 3000;
			c.Speed = 70f;
			c.Strength = 120;
			c.DefaultHeight = 500;
			c.DefaultWidth = 600;
			c.ShadowWidth = 1.75f;
			c.Aggression = 4;
			c.LiftType = CanLiftType.NoLift;
			c.RandomTextures = 2;
			c.RandomWeapons = 2;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.75f, 1f), 1f, 1f);
			switch (Game1.map.regionName)
			{
			case "trial":
				base.PrepareStats(c, 0.75f);
				break;
			case "grave":
				base.PrepareStats(c, 2f);
				break;
			default:
				base.PrepareStats(c, 1f);
				break;
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			base.me = c[ID];
			if (Game1.longSkipFrame > 3)
			{
				if (base.me.DownTime > 0f)
				{
					base.me.DefaultHeight = 400;
				}
				else
				{
					base.me.DefaultHeight = 500;
				}
				if (base.me.DyingFrame == -1f && Rand.GetRandomInt(0, 50) == 0)
				{
					Sound.PlayCue("giantgrowl", c[ID].Location, (c[0].Location - c[ID].Location).Length() / 2f);
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
