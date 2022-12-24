using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	internal class Assassin : AI
	{
		private float alpha;

		public Assassin(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 8000;
			c.Strength = 400;
			c.Speed = 550f;
			c.Aggression = 40;
			c.JumpVelocity = 1800f;
			c.DefaultHeight = 250;
			c.DefaultWidth = 200;
			c.ShadowWidth = 1f;
			c.DamageType = DamageTypes.NotTeam;
			c.RandomWeapons = 4;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.8f, 1f), Rand.GetRandomFloat(0.8f, 1f), Rand.GetRandomFloat(0.8f, 1f), 1f);
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.AnimName.StartsWith("attack") || base.me.AnimName.StartsWith("hurt"))
			{
				this.alpha = 2f;
			}
			else
			{
				this.alpha = Math.Max(this.alpha - Game1.FrameTime * 4f, 0.2f);
			}
			base.me.defaultColor = new Color(this.alpha, this.alpha, this.alpha, this.alpha);
			if (base.me.renderable && Rand.GetRandomInt(0, 10) == 0)
			{
				Game1.pManager.AddElectricBolt(base.me.Location, base.me.ID, 0.5f, 200, 50, 0.2f, 5);
			}
			base.DodgeAttack(c, base.targ, 400f, 1000f, base.me.JumpVelocity / 2f, faceTarg: true, base.me.Aggression);
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
	}
}
