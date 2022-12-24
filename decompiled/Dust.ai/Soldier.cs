using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.ai
{
	internal class Soldier : AI
	{
		public Soldier(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 10000;
			c.Strength = 400;
			c.Speed = 550f;
			c.Aggression = 60;
			c.JumpVelocity = 1800f;
			c.DefaultHeight = 250;
			c.DefaultWidth = 200;
			c.ShadowWidth = 1f;
			c.DamageType = DamageTypes.NotTeam;
			c.SpawnType = SpawnTypes.NoEffect;
			c.RandomWeapons = 4;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.8f, 1f), Rand.GetRandomFloat(0.8f, 1f), Rand.GetRandomFloat(0.8f, 1f), 1f);
			base.jobType = JobType.Idle;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.Name == "atease")
			{
				if (base.me.AnimName != "atease")
				{
					base.me.SetAnim("atease", 0, 0);
				}
				base.me.Ethereal = EtherealState.Ethereal;
				base.Update(c, ID, map);
				return;
			}
			if (base.DodgeAttack(c, base.targ, 400f, 1000f, base.me.JumpVelocity / 2f, faceTarg: true, 80f))
			{
				Sound.PlayCue("soldier_evade", base.me.Location, (c[0].Location - base.me.Location).Length());
			}
			if (base.targ == -1)
			{
				base.targ = base.FindTarg(c);
			}
			else if (base.jobFrame < 0f)
			{
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

		public override string CheckAnimName(string animName)
		{
			if (base.me.AnimName == "biggrab" && Rand.GetRandomInt(0, 4) == 0)
			{
				base.me.AnimName = "idle00";
				base.me.Slide(-1200f);
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 1f);
				base.me.SetJump(1200f, jumped: true);
				return "jump";
			}
			if (animName.StartsWith("hurt") && Rand.GetRandomInt(0, 10) == 0)
			{
				Sound.PlayCue("soldier_hurt", base.me.Location, (Game1.character[0].Location - base.me.Location).Length());
			}
			return animName;
		}

		public override bool CheckHit(Particle p)
		{
			if (base.me.State == CharState.Air)
			{
				return true;
			}
			if (base.me.DownTime == 0f && Rand.GetRandomInt(0, 4) == 0 && p != null && p.flag != 20)
			{
				base.me.Defense = DefenseStates.Parrying;
				if (!(p is Hit) && base.me.State == CharState.Grounded)
				{
					base.me.SetAnim("parry", 0, 0);
				}
			}
			return true;
		}

		public override void Die(ParticleManager pMan, Texture2D texture)
		{
			Sound.PlayCue("soldier_hurt", base.me.Location, (Game1.character[0].Location - base.me.Location).Length());
			base.Die(pMan, texture);
		}
	}
}
