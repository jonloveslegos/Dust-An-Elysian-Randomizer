using Dust.CharClasses;
using Dust.MapClasses;

namespace Dust.ai.NPC
{
	internal class NPCStill : AI
	{
		public NPCStill(Character c)
		{
			base.me = c;
			c.Speed = 200f;
			c.Team = Team.Friendly;
			c.NPC = NPCType.Friendly;
			base.jobType = JobType.StillCanTurn;
			c.DefaultHeight = 220;
			c.DefaultWidth = 370;
			c.PatrolType = PatrollingType.CannotFalloff;
			base.initPos = c.Location.X;
			c.MaskGlow = 1f;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			base.Update(c, ID, map);
		}
	}
}
