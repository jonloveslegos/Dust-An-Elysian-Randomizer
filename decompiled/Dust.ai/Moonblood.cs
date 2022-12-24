using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	internal class Moonblood : AI
	{
		private bool leaving;

		public Moonblood(Character c)
		{
			base.me = c;
			c.Team = Team.Friendly;
			c.MaxHP = 1200;
			c.Speed = Rand.GetRandomFloat(600f, 900f);
			c.Aggression = 40;
			c.JumpVelocity = Rand.GetRandomFloat(1800f, 2600f);
			c.Strength = 100;
			c.DefaultHeight = 250;
			c.DefaultWidth = 200;
			c.ShadowWidth = 1f;
			c.DamageType = DamageTypes.NotTeam;
			c.maxAnimFrames = 24;
			c.SpawnType = SpawnTypes.NoEffect;
			c.RandomTextures = 4;
			c.RandomWeapons = 2;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), 1f);
			this.leaving = false;
			base.jobType = JobType.Idle;
			base.PrepareStats(c, 1f);
		}

		public override bool CheckHit(Particle p)
		{
			if (p != null && p.owner > -1 && Game1.character[p.owner].Team == base.me.Team)
			{
				return false;
			}
			return true;
		}

		public override void MapCollision(Map map, Character[] c, Character me)
		{
			if (!this.leaving)
			{
				base.MapCollision(map, c, me);
			}
		}

		public override string CheckAnimName(string animName)
		{
			if (this.leaving)
			{
				return "superjump";
			}
			if (animName == "jump" && base.me.IsFalling)
			{
				base.me.Trajectory.Y = (0f - base.me.JumpVelocity) * Rand.GetRandomFloat(0.6f, 1f);
				base.me.Trajectory.X *= 1.4f;
			}
			if (animName == "run")
			{
				if (Rand.GetRandomInt(0, 2) != 0)
				{
					return "run2";
				}
				return "run";
			}
			return animName;
		}

		public override bool TriggerAI(int trigger)
		{
			this.LeaveScene();
			return true;
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.DyingFrame == -1f)
			{
				base.me.Ethereal = EtherealState.EtherealVulnerable;
			}
			if (base.me.Name.StartsWith("idle"))
			{
				if (base.me.Name == "idle")
				{
					if (base.me.AnimName != "atease")
					{
						base.me.SetAnim("atease", 0, 0);
					}
				}
				else if (base.me.AnimName != "idle00")
				{
					base.me.SetAnim("idle00", 0, 0);
				}
				base.Update(c, ID, map);
				return;
			}
			if (this.leaving)
			{
				base.me.Slide(1000f);
				if (!base.me.renderable)
				{
					base.me.Exists = CharExists.Dead;
				}
				return;
			}
			if (base.me.AnimName == "run2")
			{
				base.me.AnimName = "run";
			}
			if (base.targ == -1)
			{
				if (base.me.Name == "runner")
				{
					if (base.me.Location.X > map.rightEdge)
					{
						base.me.Exists = CharExists.Dead;
						return;
					}
					if (base.me.Location.X > c[0].Location.X && !base.me.renderable)
					{
						base.me.Exists = CharExists.Dead;
						return;
					}
					if ((base.me.State == CharState.Grounded || base.me.Boosting == 0) && base.me.WallInWay && !base.CanJumpWall(base.me.JumpVelocity))
					{
						this.LeaveScene();
						return;
					}
					base.jobType = JobType.RunRight;
					if (base.me.AnimName == "run" && Rand.GetRandomInt(0, 100) == 0)
					{
						base.me.SetLunge(LungeStates.Lunging, base.me.Speed * 2f, base.me.JumpVelocity / 2f);
					}
				}
				base.targ = base.FindTarg(c);
			}
			else if (base.jobFrame < 0f)
			{
				if (!base.me.renderable && base.me.Name == "runner")
				{
					base.me.Exists = CharExists.Dead;
					return;
				}
				float randomFloat = Rand.GetRandomFloat(0f, 1f);
				if (randomFloat < 0.4f)
				{
					base.jobType = JobType.MeleeChaseLunging;
					base.jobFrame = Rand.GetRandomFloat(2f, 6f);
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
				base.targ = base.FindTarg(c);
			}
			base.Update(c, ID, map);
		}

		private void LeaveScene()
		{
			this.leaving = true;
			base.me.WallInWay = true;
			base.me.JumpVelocity = 1000000f;
			base.me.SetJump(Rand.GetRandomInt(1000, 2200), jumped: true);
			base.me.Slide(1000f);
			base.me.SetAnim("superjump", 0, 0);
			base.me.LungeState = LungeStates.Lunging;
		}
	}
}
