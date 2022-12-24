using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	internal class Bunny : AI
	{
		public Bunny(Character c)
		{
			base.me = c;
			c.Speed = 300f;
			c.JumpVelocity = 1000f;
			c.Team = Team.Friendly;
			c.NPC = NPCType.WildLife;
			c.DefaultHeight = 60;
			c.DefaultWidth = 60;
			c.ShadowWidth = 0.4f;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 1f);
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.jobFrame < 0f && base.me.State == CharState.Grounded)
			{
				float randomFloat = Rand.GetRandomFloat(0f, 1f);
				if (randomFloat < 0.3f)
				{
					base.jobType = JobType.RunLeft;
					base.jobFrame = Rand.GetRandomFloat(0.25f, 1f);
				}
				else if (randomFloat < 0.6f)
				{
					base.jobType = JobType.RunRight;
					base.jobFrame = Rand.GetRandomFloat(0.25f, 1f);
				}
				else if (randomFloat < 0.8f)
				{
					base.jobType = JobType.Idle;
					base.jobFrame = Rand.GetRandomFloat(0.5f, 3f);
				}
				else if (base.me.Trajectory.X != 0f && base.me.Ai.CanJumpWall(700f))
				{
					base.me.Trajectory.X = MathHelper.Clamp(base.me.Trajectory.X * 10f, (0f - base.me.Speed) * 3f, base.me.Speed * 3f);
					base.me.SetJump(700f, jumped: true);
				}
			}
			base.Update(c, ID, map);
		}
	}
}
