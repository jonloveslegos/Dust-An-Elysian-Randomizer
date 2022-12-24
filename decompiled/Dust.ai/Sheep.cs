using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	internal class Sheep : AI
	{
		public Sheep(Character c)
		{
			base.me = c;
			c.Speed = 300f;
			c.JumpVelocity = 1000f;
			c.Team = Team.Friendly;
			c.NPC = NPCType.WildLife;
			c.PatrolType = PatrollingType.CannotFalloff;
			c.DefaultHeight = 220;
			c.DefaultWidth = 200;
			c.LiftType = CanLiftType.NoLift;
			c.ShadowWidth = 1f;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 1f);
			base.initPos = c.Location.X;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.renderable && base.me.Definition.charType == CharacterType.GappySheep && Game1.longSkipFrame == 1)
			{
				Game1.pManager.AddSparkle(base.me.Location + Rand.GetRandomVector2(-80f, 80f, -200f, -100f), 1f, 1f, 1f, 0.5f, Rand.GetRandomFloat(0.2f, 1f), 48, 6);
			}
			if (base.jobFrame < 0f && base.me.State == CharState.Grounded)
			{
				float randomFloat = Rand.GetRandomFloat(0f, 1f);
				if (randomFloat < 0.8f && base.jobType != JobType.RunRight && base.jobType != JobType.RunLeft)
				{
					if (base.me.Location.X > base.initPos)
					{
						base.jobType = JobType.RunLeft;
					}
					else
					{
						base.jobType = JobType.RunRight;
					}
					base.jobFrame = Rand.GetRandomFloat(0.5f, 3f);
				}
				else
				{
					base.jobType = JobType.Idle;
					base.jobFrame = Rand.GetRandomFloat(2f, 6f);
				}
			}
			base.Update(c, ID, map);
		}
	}
}
