using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Blomb : AI
	{
		public Blomb(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 1;
			c.Speed = 40f;
			c.JumpVelocity = 1400f;
			c.Strength = (int)Math.Max(Math.Min((float)Game1.character[0].MaxHP * Game1.stats.bonusHealth, 9999f) * 0.1f, 200f);
			c.DefaultHeight = 180;
			c.DefaultWidth = 180;
			c.FlyType = FlyingType.CanFlySwaying;
			c.maxAnimFrames = 6;
			c.MaskGlow = 1.5f;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.5f, 1f), 1f, 1f, 1f);
			c.AnimFrame = Rand.GetRandomInt(0, 25);
			base.jobFrame = Rand.GetRandomInt(2, 20);
			switch (Game1.map.regionName)
			{
			case "lava":
				base.PrepareStats(c, 2f);
				break;
			case "snow":
				base.PrepareStats(c, 1.5f);
				break;
			default:
				base.PrepareStats(c, 1f);
				break;
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			int num = 450;
			if (base.me.DyingFrame > -1f)
			{
				base.me.Trajectory *= 0.8f;
			}
			if (base.me.AnimName == "die")
			{
				base.me.Ethereal = EtherealState.Ethereal;
			}
			if (base.me.AnimName != "inhale")
			{
				base.me.Defense = DefenseStates.BlockingOnly;
			}
			Vector2 vector = base.me.Location - new Vector2(0f, 100f);
			if (base.me.AnimName != "explode")
			{
				float num2 = (vector - (c[0].Location - new Vector2(0f, 50f))).Length();
				if (num2 < (float)num && base.me.DyingFrame == -1f && base.me.AnimName != "die")
				{
					base.me.SetAnim("die", 0, 0);
					Game1.pManager.AddShockRing(vector, 0.4f, 5);
					VibrationManager.SetScreenShake(0.4f);
					Sound.PlayCue("blomb_startle", base.me.Location, (base.me.Location - c[0].Location).Length() / 2f);
				}
				if (base.jobFrame < 0f)
				{
					base.jobFrame = Rand.GetRandomInt(1, 8);
					if (base.me.AnimName != "die" && new Rectangle(50, 150, Game1.screenWidth - 100, Game1.screenHeight - 100).Contains((int)(base.me.Location.X * Game1.worldScale - Game1.Scroll.X), (int)(base.me.Location.Y * Game1.worldScale - Game1.Scroll.Y)))
					{
						base.me.SetAnim("inhale", 0, 1);
					}
				}
			}
			else
			{
				base.me.KillMe(instantly: true);
				int l = Rand.GetRandomInt(5, 7);
				Game1.pManager.AddShockRing(vector, 1f, 5);
				VibrationManager.SetBlast(1f, vector);
				float num3 = (base.me.Location - c[0].Location).Length() / 300f;
				Game1.worldDark = MathHelper.Max(Game1.worldDark - 2f / num3, 0f);
				VibrationManager.SetScreenShake(MathHelper.Min(6f / num3, 2f));
				Game1.map.MapSegFrameSpeed = 0.4f;
				Sound.PlayCue("blomb_explode", base.me.Location, (base.me.Location - c[0].Location).Length() / 4f);
				int num4 = num / 2 + 100;
				for (int i = 0; i < 18; i++)
				{
					Game1.pManager.AddExplosion(vector + Rand.GetRandomVector2(-num4, num4, -num4, num4 + 50), Rand.GetRandomFloat(1.8f, 2.8f), (Rand.GetRandomInt(0, 3) != 0) ? true : false, l);
				}
				for (int j = 0; j < 4; j++)
				{
					Vector2 loc = vector + Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
					Game1.pManager.AddBounceSpark(loc, Rand.GetRandomVector2(-800f, 800f, -500f, 10f), 0.5f, l);
					if (j < 3)
					{
						Game1.pManager.AddBlood(loc, Rand.GetRandomVector2(-400f, 400f, -800f, -400f), 1f, 1f, 1f, 1f, 0.4f, (CharacterType)1000, 0, l);
					}
				}
				for (int k = 0; k < c.Length; k++)
				{
					float num5 = (vector - (c[k].Location - new Vector2(0f, 50f))).Length();
					if (num5 < (float)num)
					{
						if (k > 0)
						{
							c[k].Ethereal = EtherealState.Normal;
						}
						int strength = (int)Math.Min(((float)num - num5) / (float)num * (float)base.me.Strength * 2f, base.me.Strength);
						HitManager.DamageRegion(c, k, strength, StatusEffects.Burning, ID, forceDamage: true);
					}
				}
			}
			base.Update(c, ID, map);
		}

		public override void MapCollision(Map map, Character[] c, Character me)
		{
		}
	}
}
