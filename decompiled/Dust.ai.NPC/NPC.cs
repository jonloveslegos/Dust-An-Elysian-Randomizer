using Dust.CharClasses;
using Dust.MapClasses;

namespace Dust.ai.NPC
{
	internal class NPC : AI
	{
		public NPC(Character c)
		{
			base.me = c;
			c.Speed = 200f;
			c.Team = Team.Friendly;
			c.NPC = NPCType.Friendly;
			c.DefaultHeight = 220;
			c.DefaultWidth = 300;
			c.PatrolType = PatrollingType.CannotFalloff;
			base.initPos = c.Location.X;
			c.MaskGlow = 1f;
			base.PrepareStats(c, 1f);
			base.jobType = JobType.Idle;
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.jobFrame < 0f)
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
