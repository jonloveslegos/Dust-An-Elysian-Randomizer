using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class KaneGhost : AI
	{
		private int fidgetHit;

		public KaneGhost(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 30000;
			c.Strength = 150;
			c.Speed = 240f;
			c.JumpVelocity = 1400f;
			c.Aggression = 100;
			c.DefaultHeight = 600;
			c.DefaultWidth = 400;
			c.ShadowWidth = 3f;
			c.maxAnimFrames = 24;
			c.SpawnType = SpawnTypes.NoEffect;
			c.defaultColor = Color.Green;
			c.LiftType = CanLiftType.NoLift;
			c.MaskGlow = 1.5f;
			base.jobType = JobType.MeleeChase;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			base.jobType = JobType.MeleeChase;
			base.me.Ethereal = EtherealState.EtherealVulnerable;
			if (base.me.DyingFrame > -1f)
			{
				Game1.events.InitEvent(390, isSideEvent: false);
				base.me.Ethereal = EtherealState.Ethereal;
				base.me.Trajectory = new Vector2(0f, -200f);
			}
			else if (!Game1.events.anyEvent)
			{
				base.me.defaultColor = Color.White;
				if (!base.me.AnimName.StartsWith("attack"))
				{
					if ((Rand.GetRandomInt(0, 100) == 0 || (base.me.Location - Game1.character[0].Location).Length() > 1000f) && this.CanSummon())
					{
						base.me.SetAnim("attack02", 0, 2);
					}
					if (this.fidgetHit > 30)
					{
						this.fidgetHit = 0;
						base.me.SetAnim("attack03", 0, 0);
					}
				}
			}
			base.randomInt = 300;
			base.FaceToTarg(c);
			base.Update(c, ID, map);
		}

		private bool CanSummon()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 1; i < Game1.character.Length; i++)
			{
				if (Game1.character[i].Exists == CharExists.Exists)
				{
					num++;
					if (Game1.character[i].renderable)
					{
						num2++;
					}
					if (num2 > 3)
					{
						return false;
					}
				}
			}
			if (num2 < 3 && num < Game1.character.Length)
			{
				return true;
			}
			return false;
		}

		public override bool CheckHit(Particle p)
		{
			if (base.me.DyingFrame > -1f)
			{
				return false;
			}
			if (p != null && p is Hit)
			{
				return true;
			}
			if (base.me.AnimName != "attack03" && !Game1.events.anyEvent)
			{
				this.fidgetHit++;
			}
			byte b = (byte)Math.Min(base.me.defaultColor.R + 20, 255);
			base.me.defaultColor = new Color(b, b, b, 255);
			return true;
		}

		public override void SpecialTrigger(Trigger trig, Vector2 loc, float rot, float scale, ParticleManager pMan)
		{
			if (!base.me.renderable)
			{
				return;
			}
			switch (trig)
			{
			case Trigger.Special0:
				pMan.AddStarburst(loc, Rand.GetRandomFloat(0.4f, 1f), 1f, 1f, 1f, 0.5f, Rand.GetRandomFloat(0.2f, 0.8f), base.me.ID, background: true, 6);
				pMan.AddElectricBolt(loc, -1, 1f, 1000, 100, 0.2f, 6);
				break;
			case Trigger.Special1:
			{
				for (int j = 0; j < 4; j++)
				{
					int num = Rand.GetRandomInt(0, 10);
					CharacterType type = ((num >= 2) ? ((Rand.GetRandomInt(0, 2) == 0) ? CharacterType.Remains : CharacterType.RemainsHalf) : CharacterType.RemainsBomb);
					Vector2 vector = loc + new Vector2(-1200 + j * 600, 0f);
					Game1.events.SpawnCharacter(vector, "enemy", type, Team.Enemy, ground: true);
					bool flag = false;
					do
					{
						Vector2 vector2 = vector;
						vector.Y += 16f;
						if (Game1.map.CheckPCol(vector + new Vector2(0f, 40f), vector2 + new Vector2(0f, 40f), canFallThrough: false, init: true) > 0f)
						{
							flag = true;
						}
					}
					while (!flag && vector.Y < Game1.map.bottomEdge);
					pMan.AddSpawnBeam(vector, 300f, 6);
					pMan.AddShockRing(vector, 0.4f, 6);
					VibrationManager.SetScreenShake(1f);
					VibrationManager.SetBlast(0.5f, loc);
				}
				break;
			}
			case Trigger.Special2:
			{
				for (int i = 0; i < 4; i++)
				{
					pMan.AddElectricBolt(loc + Rand.GetRandomVector2(-100f, 100f, -100f, 100f), -1, 0.2f, 1000, 100, 0.2f, 6);
				}
				break;
			}
			case Trigger.Special3:
				HitManager.DamageRegion(Game1.character, 0, 0, StatusEffects.Silent, base.me.ID, forceDamage: true);
				pMan.AddShockRing(loc, 1f, 6);
				break;
			}
		}
	}
}
