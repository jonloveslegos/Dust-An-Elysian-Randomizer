using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class StoneCutter : AI
	{
		public StoneCutter(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 800;
			c.Speed = 380f;
			c.Strength = 80;
			c.DefaultHeight = 360;
			c.DefaultWidth = 300;
			c.ShadowWidth = 1.75f;
			c.maxAnimFrames = 20;
			c.Aggression = 4;
			c.LiftType = CanLiftType.NoLift;
			c.RandomTextures = 2;
			c.MaxDownTime = 2f;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.75f, 1f), 1f, 1f);
			switch (Game1.map.regionName)
			{
			case "grave":
				base.PrepareStats(c, 3f);
				break;
			case "smith":
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
				string animName;
				if ((animName = base.me.AnimName) != null && animName == "attack01")
				{
					base.me.DefaultWidth = 200;
				}
				else
				{
					base.me.DefaultWidth = 400;
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
