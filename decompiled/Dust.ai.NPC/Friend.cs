using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai.NPC
{
	internal class Friend : AI
	{
		private float glowTimer = 2f;

		public Friend(Character c)
		{
			if (c.Definition.charType == CharacterType.HyperChris || c.Definition.charType == CharacterType.HyperDan)
			{
				Sound.PlayFollowCharCue("duck_headphones", c.ID);
			}
			base.me = c;
			c.Speed = 200f;
			c.Team = Team.Friendly;
			c.NPC = NPCType.WildLife;
			c.DefaultHeight = 220;
			c.DefaultWidth = 100;
			c.PatrolType = PatrollingType.CannotFalloff;
			base.initPos = c.Location.X;
			c.MaskGlow = 1f;
			if (Game1.map.path.StartsWith("sanc"))
			{
				this.glowTimer = 0f;
			}
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			this.glowTimer -= Game1.FrameTime * 2f;
			if (this.glowTimer > 0f)
			{
				Game1.pManager.AddStarburst(base.me.Location - new Vector2(0f, 50f), Rand.GetRandomFloat(0.5f, 1f) * this.glowTimer, 1f, 1f, 1f, 0.2f, Rand.GetRandomFloat(0.2f, 0.8f), base.me.ID, background: true, 5);
			}
			else if (base.me.State == CharState.Air)
			{
				base.me.Trajectory.X = 0f;
				base.me.KeyLeft = (base.me.KeyRight = false);
				base.jobType = JobType.Idle;
			}
			base.me.JumpVelocity = 1200f;
			if (base.jobFrame < 0f)
			{
				float randomFloat = Rand.GetRandomFloat(0f, 1f);
				if (randomFloat < 0.7f && base.jobType != JobType.RunRight && base.jobType != JobType.RunLeft)
				{
					if (map.path.StartsWith("sanc"))
					{
						base.jobType = ((Rand.GetRandomInt(0, 2) == 0) ? JobType.RunLeft : JobType.RunRight);
					}
					else if (base.me.Location.X > base.initPos)
					{
						base.jobType = JobType.RunLeft;
					}
					else
					{
						base.jobType = JobType.RunRight;
					}
					base.jobFrame = Rand.GetRandomFloat(0.5f, 3f);
				}
				else if (randomFloat < 0.9f)
				{
					base.jobType = JobType.Idle;
					base.jobFrame = Rand.GetRandomFloat(4f, 6f);
				}
				else if (base.me.JumpVelocity != 0f && base.me.State == CharState.Grounded && base.me.Ai.CanJumpWall(base.me.JumpVelocity))
				{
					base.me.SetJump(base.me.JumpVelocity, jumped: true);
				}
			}
			base.Update(c, ID, map);
		}
	}
}
