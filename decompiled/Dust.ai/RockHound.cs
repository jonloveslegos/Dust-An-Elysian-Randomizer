using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	internal class RockHound : AI
	{
		public RockHound(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 500;
			c.Strength = 60;
			c.Speed = 500f;
			c.JumpVelocity = 1600f;
			c.DefaultHeight = 180;
			c.DefaultWidth = 180;
			c.DamageType = DamageTypes.Everyone;
			c.ShadowWidth = 1f;
			c.RandomTextures = 2;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), 1f);
			string regionName;
			if ((regionName = Game1.map.regionName) != null && regionName == "trial")
			{
				base.PrepareStats(c, 0.5f);
			}
			else
			{
				base.PrepareStats(c, 1f);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (c[ID].DyingFrame == -1f && Rand.GetRandomInt(0, 200) == 0)
			{
				Sound.PlayCue("rockhound_idle", c[ID].Location, (c[0].Location - c[ID].Location).Length());
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
		}
	}
}
