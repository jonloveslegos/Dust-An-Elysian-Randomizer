using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Wolf : AI
	{
		public Wolf(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 5000;
			c.Strength = 250;
			c.Speed = 700f;
			c.JumpVelocity = 1800f;
			c.DefaultHeight = 160;
			c.DefaultWidth = 250;
			c.MaxDownTime = 10f;
			c.DamageType = DamageTypes.NotTeam;
			c.Aggression = 70;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.8f, 1f), Rand.GetRandomFloat(0.8f, 1f), 1f, 1f);
			string regionName;
			if ((regionName = Game1.map.regionName) != null && regionName == "lava")
			{
				base.PrepareStats(c, 1.6f);
			}
			else
			{
				base.PrepareStats(c, 1f);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.targ > -1 && base.me.AnimName == "jump" && base.me.LungeState == LungeStates.None && base.me.Location.Y < c[base.targ].Location.Y && (base.me.Location - c[base.targ].Location).Length() < 400f)
			{
				base.me.SetAnim("attackair", 0, 1);
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
