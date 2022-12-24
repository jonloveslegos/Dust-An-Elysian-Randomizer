using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.ai
{
	public class LadyBoss : AI
	{
		private int hurtCount;

		private float hitTime;

		public LadyBoss(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 8000;
			c.Strength = 100;
			c.Speed = 240f;
			c.JumpVelocity = 1400f;
			c.Aggression = 100;
			c.DefaultHeight = 300;
			c.DefaultWidth = 200;
			c.RandomTextures = 1;
			c.maxAnimFrames = 40;
			c.FlyType = FlyingType.CanFly;
			c.SpawnType = SpawnTypes.NoEffect;
			c.defaultColor = Color.White;
			base.jobType = JobType.FlyShoot;
			c.MaskGlow = 1.5f;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (this.hitTime > 0f)
			{
				this.hitTime -= Game1.FrameTime;
			}
			else
			{
				this.hurtCount = 0;
			}
			base.me.terrainType = TerrainType.Water;
			if (base.me.DyingFrame > -1f)
			{
				Game1.events.InitEvent(240, isSideEvent: false);
				base.me.Ethereal = EtherealState.Ethereal;
				base.me.Trajectory = new Vector2(0f, -200f);
			}
			else if (!Game1.events.anyEvent)
			{
				if (Rand.GetRandomInt(0, 40) == 0)
				{
					Game1.pManager.AddElectricBolt(base.me.Location, base.me.ID, 1f, 1000, 100, 0.1f, 6);
				}
				if (Rand.GetRandomInt(0, 200) == 0 && base.me.AnimName != "biggrab")
				{
					Sound.PlayPersistCue("lady_taunt", new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
				}
			}
			else
			{
				base.me.CharRotation = 3.14f;
			}
			if (!base.overrideAI && !Game1.events.anyEvent)
			{
				base.jobType = JobType.FlyKeepDistance;
				if ((base.me.Location - c[0].Location).Length() > 1500f && !base.me.AnimName.StartsWith("attack"))
				{
					base.me.SetAnim("flyattack", 0, 0);
				}
			}
			base.randomInt = 300;
			base.FaceToTarg(c);
			base.Update(c, ID, map);
		}

		public override string CheckAnimName(string animName)
		{
			if (base.me.AnimName == "biggrab" && Rand.GetRandomInt(0, 3) > 0)
			{
				base.me.Slide(-1000f);
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 2f);
			}
			if (animName == "hurtup")
			{
				Sound.StopPersistCue("lady_taunt");
				Sound.PlayCue("lady_hurt");
				this.hurtCount++;
				this.hitTime = 0.5f;
				if (base.me.Location.X < Game1.map.leftBlock + 1000f)
				{
					base.me.Face = CharDir.Left;
					base.me.Slide(-2000f);
					base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 2f);
				}
				if (base.me.Location.X > Game1.map.rightBlock - 1000f)
				{
					base.me.Face = CharDir.Right;
					base.me.Slide(-2000f);
					base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 2f);
				}
			}
			if (this.hurtCount > 20)
			{
				base.me.Slide(-1000f);
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 2f);
				this.hurtCount = 0;
				return "fly";
			}
			if (animName == "flyattack")
			{
				if ((base.me.Location - Game1.character[0].Location).Length() < 800f)
				{
					return "attack01";
				}
				return "attack00";
			}
			return animName;
		}

		public override void Die(ParticleManager pMan, Texture2D texture)
		{
			Sound.StopPersistCue("lady_taunt");
			Sound.PlayCue("lady_die");
			base.Die(pMan, texture);
		}

		public override bool CheckHit(Particle p)
		{
			if (base.me.DyingFrame > -1f)
			{
				return false;
			}
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
			{
				for (int i = 0; i < 2; i++)
				{
					pMan.AddStarburst(loc, Rand.GetRandomFloat(0.1f, 0.3f), 1f, 1f, 1f, 0.2f, Rand.GetRandomFloat(0.2f, 0.8f), base.me.ID, background: true, 6);
				}
				pMan.AddElectricBolt(loc, -1, 1f, 1000, 100, 0.2f, 6);
				break;
			}
			case Trigger.Special1:
				if (Game1.events.anyEvent)
				{
					pMan.AddLadySphere(Game1.character[0].Location + new Vector2(-250f, -400f), 1f, base.me.Strength, base.me.ID, 5);
				}
				else
				{
					pMan.AddLadySphere(Game1.character[0].Location + new Vector2(0f, -100f), 1f, base.me.Strength, base.me.ID, 5);
				}
				break;
			case Trigger.Special2:
			{
				float num = rot + Rand.GetRandomFloat(-0.5f, 0.5f);
				pMan.AddPlayerDebris(loc + Rand.GetRandomVector2(-50f, 50f, -50f, 50f), new Vector2((float)Math.Cos(num), (float)Math.Sin(num)) * 1200f, Game1.character[base.me.ID].GetSheet(1, base.me.Definition.Sprites_01_Index), new Rectangle(1800, Rand.GetRandomInt(0, 4) * 32, 42, 32), 1f, Rand.GetRandomFloat(0.4f, 1f), 0, byte.MaxValue, byte.MaxValue, byte.MaxValue, 6);
				break;
			}
			}
		}
	}
}
