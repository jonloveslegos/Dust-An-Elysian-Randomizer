using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class DarkVillager : AI
	{
		public DarkVillager(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 30;
			c.Speed = 550f;
			c.JumpVelocity = 1800f;
			c.Strength = 4;
			c.Aggression = 2;
			c.DefaultHeight = 220;
			c.DefaultWidth = 160;
			c.RandomTextures = 2;
			c.RandomWeapons = 3;
			c.MaxDownTime = 10f;
			c.alwaysUpdatable = true;
			c.defaultColor = Color.White;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
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
		}
	}
}
