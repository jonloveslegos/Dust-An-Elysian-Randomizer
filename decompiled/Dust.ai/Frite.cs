using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Frite : AI
	{
		public Frite(Character c)
		{
			Sound.PlayFollowCharCue("frite_idle", c.ID);
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 15000;
			c.Strength = 400;
			c.Speed = 240f;
			c.JumpVelocity = 1400f;
			c.Aggression = 10;
			c.DefaultHeight = 300;
			c.DefaultWidth = 200;
			c.RandomTextures = 1;
			c.maxAnimFrames = 20;
			c.FlyType = FlyingType.CanFly;
			c.DieType = DyingTypes.BodyStay;
			c.defaultColor = Color.White;
			base.jobType = JobType.FlyShoot;
			c.MaskGlow = 1.5f;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.AnimName == "die")
			{
				base.me.Trajectory = new Vector2(0f, -200f);
			}
			base.jobType = JobType.FlyKeepDistance;
			base.randomInt = 300;
			base.FaceToTarg(c);
			base.Update(c, ID, map);
		}

		public override string CheckAnimName(string animName)
		{
			if (animName == "flyattack")
			{
				return "attack0" + Rand.GetRandomInt(0, 2);
			}
			return animName;
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
				for (int l = 0; l < 10; l++)
				{
					pMan.AddStarburst(loc, Rand.GetRandomFloat(1f, 2f), 1f, 1f, 1f, 0.2f, Rand.GetRandomFloat(0.2f, 0.8f), base.me.ID, background: true, 5);
				}
				break;
			}
			case Trigger.Special1:
			{
				int num = ((loc.X > base.me.Location.X) ? 1 : (-1));
				for (int m = 0; m < 8; m++)
				{
					pMan.AddPlayerDebris(loc + Rand.GetRandomVector2(-50f, 50f, -100f, 50f), new Vector2(Rand.GetRandomInt(200, 400) * num, Rand.GetRandomInt(-1200, -1000)), Game1.character[base.me.ID].GetSheet(1, base.me.Definition.Sprites_01_Index + base.me.RandomSkin), new Rectangle(3150, 54 * Rand.GetRandomInt(0, 3), 46, 54), 1.5f, Rand.GetRandomFloat(0.4f, 1.5f), 1, base.me.defaultColor.R, base.me.defaultColor.G, base.me.defaultColor.B, 5);
				}
				pMan.AddSpray(loc + new Vector2(0f, -100f), new Vector2(200 * num, -500f), 0.3f, 2, 10, 5);
				pMan.AddSpray(loc + new Vector2(0f, -100f), new Vector2(200 * num, -400f), 0.6f, 1, 6, 5);
				if (base.targ > -1 && (loc - Game1.character[base.targ].Location).Length() < 1000f)
				{
					if (base.me.AnimName == "attack00")
					{
						HitManager.DamageRegion(Game1.character, base.targ, base.me.Strength, StatusEffects.Normal, base.me.ID, forceDamage: true);
					}
					else
					{
						HitManager.DamageRegion(Game1.character, base.targ, 0, StatusEffects.Silent, base.me.ID, forceDamage: true);
					}
				}
				break;
			}
			case Trigger.Special2:
			{
				for (int k = 0; k < 4; k++)
				{
					pMan.AddStarburst(loc, Rand.GetRandomFloat(0.7f, 1f) * scale, 1f, 1f, 1f, 0.2f, Rand.GetRandomFloat(0.1f, 0.4f), base.me.ID, background: true, 5);
				}
				break;
			}
			case Trigger.Special3:
			{
				pMan.AddShockRing(loc, 0.7f, 6);
				VibrationManager.SetBlast(0.85f, loc);
				for (int i = 0; i < 40; i++)
				{
					pMan.AddPlayerDebris(loc + Rand.GetRandomVector2(-100f, 100f, -300f, 200f), new Vector2(Rand.GetRandomInt(200, 800) * ((Rand.GetRandomInt(0, 2) != 0) ? 1 : (-1)), Rand.GetRandomInt(-1600, -1000)), Game1.character[base.me.ID].GetSheet(1, base.me.Definition.Sprites_01_Index + base.me.RandomSkin), new Rectangle(190 + 150 * i - i / 3 * 450, 16, 64, 84), 0.6f, Rand.GetRandomFloat(0.4f, 1.5f), 1, base.me.defaultColor.R, base.me.defaultColor.G, base.me.defaultColor.B, 6);
				}
				for (int j = 0; j < 11; j++)
				{
					pMan.AddPlayerDebris(loc + Rand.GetRandomVector2(-100f, 100f, -300f, 200f), new Vector2(Rand.GetRandomInt(200, 800) * ((Rand.GetRandomInt(0, 2) != 0) ? 1 : (-1)), Rand.GetRandomInt(-1200, -1000)), Game1.character[base.me.ID].GetSheet(1, base.me.Definition.Sprites_01_Index + base.me.RandomSkin), new Rectangle(640 + 150 * j, 32, 116, 110), 1f, Rand.GetRandomFloat(1f, 2f), 1, base.me.defaultColor.R, base.me.defaultColor.G, base.me.defaultColor.B, 5);
				}
				break;
			}
			}
		}
	}
}
