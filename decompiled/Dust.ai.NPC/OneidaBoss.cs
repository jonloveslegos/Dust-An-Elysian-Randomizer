using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai.NPC
{
	internal class OneidaBoss : AI
	{
		public OneidaBoss(Character c)
		{
			base.me = c;
			c.Speed = 200f;
			c.Team = Team.Friendly;
			c.NPC = NPCType.Friendly;
			c.DefaultHeight = 220;
			c.DefaultWidth = 300;
			base.initPos = c.Location.X;
			base.jobType = JobType.Still;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (Game1.events.currentEvent == 35)
			{
				c[ID].NPC = NPCType.WildLife;
				if (Game1.events.subEvent > 4)
				{
					float num = MathHelper.Clamp((float)(int)c[ID].defaultColor.R / 255f - Game1.FrameTime, 0.5f, 1f);
					c[ID].defaultColor = new Color(num, num, num, 1f);
				}
			}
			else if (Game1.events.currentEvent >= 40)
			{
				c[ID].NPC = NPCType.Friendly;
				float num2 = MathHelper.Clamp((float)(int)c[ID].defaultColor.R / 255f + Game1.FrameTime, 0f, 1f);
				c[ID].defaultColor = new Color(num2, num2, num2, 1f);
			}
			base.Update(c, ID, map);
		}
	}
}
