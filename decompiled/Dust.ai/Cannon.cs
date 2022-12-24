using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Cannon : AI
	{
		public Cannon(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 20000;
			c.Speed = 0f;
			c.JumpVelocity = 0f;
			c.Strength = 1;
			c.DefaultHeight = 400;
			c.DefaultWidth = 600;
			c.LiftType = CanLiftType.Immovable;
			c.DieType = DyingTypes.BodyStay;
			c.alwaysUpdatable = true;
			c.Face = CharDir.Right;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.AnimName == "die")
			{
				ref Color defaultColor = ref base.me.defaultColor;
				ref Color defaultColor2 = ref base.me.defaultColor;
				byte b2 = (base.me.defaultColor.B = (byte)Math.Max(base.me.defaultColor.R - 5, 80));
				byte r = (defaultColor2.G = b2);
				defaultColor.R = r;
			}
			base.Update(c, ID, map);
		}

		public override bool PlaySound(string sound)
		{
			if (sound == "cannon_fire")
			{
				float num = (Game1.character[0].Location - base.me.Location).Length();
				Sound.PlayCue(sound, base.me.Location, num / 1.5f);
				Sound.PlayCue("cannon_fire_distant", base.me.Location, 100f / num);
				return false;
			}
			return true;
		}

		public override void SpecialTrigger(Trigger trig, Vector2 loc, float rot, float scale, ParticleManager pMan)
		{
			switch (trig)
			{
			case Trigger.Special0:
			{
				float num = 500f / (loc - Game1.character[0].Location).Length();
				VibrationManager.SetBlast(Math.Min(num, 0.6f), loc);
				VibrationManager.SetScreenShake(MathHelper.Clamp(num, 0.2f, 0.5f));
				if (Game1.character[0].Location.Y > loc.Y + 2000f)
				{
					Vector2 vector = (Game1.Scroll + new Vector2(Rand.GetRandomFloat(100f, Game1.screenWidth - 200), 0f)) / Game1.worldScale;
					for (int k = 0; k < 10; k++)
					{
						pMan.AddBlood(vector + new Vector2(Rand.GetRandomFloat(-60f, 60f), 0f), new Vector2(0f, Rand.GetRandomFloat(0f, 1000f)), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(0.1f, 0.5f), (CharacterType)1000, 0, 5);
					}
				}
				pMan.AddCannonFireBall(loc, new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)) * 10000f, 0.6f, 0, base.me.ID, 5);
				pMan.AddShockRing(loc, 0.5f, 6);
				break;
			}
			case Trigger.Special1:
				rot += Rand.GetRandomFloat(-0.4f, 0.4f);
				pMan.AddCannonShell(loc, new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)) * 1000f, 1.3f, 5);
				break;
			case Trigger.Special2:
				if (base.me.renderable)
				{
					pMan.AddSmoke(loc, Rand.GetRandomVector2(-60f, 60f, -300f, -100f), 0f, 0f, 0f, 0.5f, Rand.GetRandomFloat(1f, 2f), 3f, 6);
					if (Rand.GetRandomInt(0, 4) == 0)
					{
						pMan.AddElectricBolt(loc + Rand.GetRandomVector2(-40f, 40f, -40f, 40f), -1, Rand.GetRandomFloat(0.1f, 0.3f), 600, 30, 0.3f, 6);
					}
				}
				break;
			case Trigger.Special3:
				if (base.me.renderable)
				{
					for (int i = 0; i < 4; i++)
					{
						pMan.AddExplosion(loc + Rand.GetRandomVector2(-100f, 100f, -100f, 100f), Rand.GetRandomFloat(1.5f, 2f), (Rand.GetRandomInt(0, 4) != 0) ? true : false, 6);
					}
					for (int j = 0; j < 2; j++)
					{
						Vector2 loc2 = loc + Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
						pMan.AddBounceSpark(loc2, Rand.GetRandomVector2(-800f, 800f, -500f, 10f), 0.5f, 6);
					}
					VibrationManager.SetBlast(1f, loc);
					VibrationManager.SetScreenShake(2f);
				}
				break;
			}
		}
	}
}
