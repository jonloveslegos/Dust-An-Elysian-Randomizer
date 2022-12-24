using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	internal class Deer : AI
	{
		public Deer(Character c)
		{
			base.me = c;
			c.Speed = Rand.GetRandomInt(600, 1000);
			c.Team = Team.Friendly;
			c.NPC = NPCType.WildLife;
			c.DefaultHeight = 200;
			c.DefaultWidth = 200;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				c.RandomTextures = 2;
			}
			c.defaultColor = new Color(1f, Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), 1f);
			base.initPos = c.Location.X;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (c[ID].AnimName == "run" && !c[ID].renderable)
			{
				c[ID].AnimFrame = Rand.GetRandomInt(0, 6);
			}
			if (c[ID].Name == "deerrunright")
			{
				base.jobType = JobType.RunRight;
				base.jobFrame = 3f;
			}
			else if (c[ID].Name == "deerrunleft")
			{
				base.jobType = JobType.RunLeft;
				base.jobFrame = 3f;
			}
			else if (base.jobFrame < 0f)
			{
				float randomFloat = Rand.GetRandomFloat(0f, 1f);
				if (randomFloat < 0.4f && base.jobType != JobType.RunRight && base.jobType != JobType.RunLeft)
				{
					if (c[ID].AnimName != "idle01")
					{
						if (base.me.Location.X > base.initPos)
						{
							base.jobType = JobType.RunLeft;
						}
						else
						{
							base.jobType = JobType.RunRight;
						}
						base.jobFrame = Rand.GetRandomFloat(0.5f, 2f);
					}
				}
				else
				{
					if (!c[ID].AnimName.StartsWith("run") && c[ID].AnimName != "idle01")
					{
						c[ID].SetAnim("idle0" + Rand.GetRandomInt(0, 2), 0, 2);
					}
					base.jobType = JobType.Idle;
					base.jobFrame = Rand.GetRandomFloat(2f, 6f);
				}
			}
			base.Update(c, ID, map);
		}
	}
}
