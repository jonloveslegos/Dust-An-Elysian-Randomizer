using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.ai
{
	internal class Remains : AI
	{
		public Remains(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 1800;
			if (c.Definition.charType != CharacterType.RemainsBomb)
			{
				c.Speed = 300f;
				c.DefaultHeight = 180;
				c.Strength = 200;
				c.defaultColor = new Color(Rand.GetRandomFloat(0.2f, 1f), Rand.GetRandomFloat(0.7f, 1f), Rand.GetRandomFloat(0.7f, 1f), 1f);
				c.DamageType = DamageTypes.NotTeam;
			}
			else
			{
				c.Speed = 200f;
				c.DefaultHeight = 380;
				c.Strength = 400;
				c.defaultColor = new Color(Rand.GetRandomFloat(0.8f, 1f), Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), 1f);
				c.MaskGlow = 1.5f;
				c.DamageType = DamageTypes.Everyone;
			}
			if (c.Definition.charType == CharacterType.RemainsHalf)
			{
				c.RandomTextures = 1;
				c.DefaultWidth = 150;
				c.DefaultHeight = 220;
			}
			else
			{
				c.RandomTextures = 2;
				c.DefaultWidth = 180;
			}
			c.JumpVelocity = 1800f;
			c.SpawnType = SpawnTypes.NoEffect;
			c.DieType = DyingTypes.BodyVanish;
			c.ShadowWidth = 1f;
			string regionName;
			if ((regionName = Game1.map.regionName) != null && regionName == "trial")
			{
				base.PrepareStats(c, 0.05f);
			}
			else
			{
				base.PrepareStats(c, 1f);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.Definition.charType == CharacterType.RemainsBomb)
			{
				base.me.MaskGlow = Rand.GetRandomFloat(1.2f, 1.6f);
				int num = 400;
				Vector2 vector = base.me.Location - new Vector2(0f, 300f);
				if (base.me.AnimName == "idle00" || base.me.AnimName.StartsWith("run"))
				{
					float num2 = (vector - (c[0].Location - new Vector2(0f, 50f))).Length();
					if (num2 < (float)num && base.me.DyingFrame == -1f && base.me.AnimName != "prepboom")
					{
						base.me.SetAnim("prepboom", 0, 0);
						Game1.pManager.AddShockRing(vector, 0.4f, 5);
						VibrationManager.SetScreenShake(0.4f);
						Sound.PlayCue("zombie_stretch", base.me.Location, (base.me.Location - c[0].Location).Length() / 2f);
					}
				}
			}
			if (base.me.AnimName == "prepboom")
			{
				base.jobType = JobType.Idle;
			}
			else
			{
				if (c[ID].DyingFrame == -1f && Rand.GetRandomInt(0, 200) == 0)
				{
					Sound.PlayCue("zombie_groan", c[ID].Location, (c[0].Location - c[ID].Location).Length());
				}
				if (base.jobFrame < 0f)
				{
					float randomFloat = Rand.GetRandomFloat(0f, 1f);
					if (randomFloat < 0.4f)
					{
						base.jobType = JobType.MeleeChaseLunging;
						base.jobFrame = Rand.GetRandomFloat(2f, 4f);
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
				}
			}
			base.Update(c, ID, map);
		}

		private void Explode(Character[] c, int range, Vector2 orig)
		{
			int l = Rand.GetRandomInt(5, 7);
			Game1.pManager.AddShockRing(orig, 1f, 5);
			VibrationManager.SetBlast(1f, orig);
			float num = (base.me.Location - c[0].Location).Length() / 300f;
			Game1.worldDark = MathHelper.Max(Game1.worldDark - 2f / num, 0f);
			VibrationManager.SetScreenShake(MathHelper.Min(6f / num, 2f));
			Game1.map.MapSegFrameSpeed = 0.4f;
			Sound.PlayCue("blomb_explode", base.me.Location, (base.me.Location - c[0].Location).Length() / 4f);
			int num2 = range / 2 + 100;
			for (int i = 0; i < 18; i++)
			{
				Game1.pManager.AddExplosion(orig + Rand.GetRandomVector2(-num2, num2, -num2, num2 + 50), Rand.GetRandomFloat(1.8f, 2.8f), (Rand.GetRandomInt(0, 3) != 0) ? true : false, l);
			}
			for (int j = 0; j < 4; j++)
			{
				Vector2 loc = orig + Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
				Game1.pManager.AddBounceSpark(loc, Rand.GetRandomVector2(-800f, 800f, -500f, 10f), 0.5f, l);
			}
			for (int k = 0; k < c.Length; k++)
			{
				float num3 = (orig - (c[k].Location - new Vector2(0f, 50f))).Length();
				if (num3 < (float)range && c[k].Definition.charType != CharacterType.Summoner && c[k].Definition.charType != CharacterType.KaneGhost)
				{
					if (k > 0)
					{
						c[k].Ethereal = EtherealState.Normal;
					}
					else if (!c[0].AnimName.StartsWith("hurt") && c[0].CanHurtFrame < 3f)
					{
						c[0].Ethereal = EtherealState.Normal;
					}
					int strength = (int)Math.Min(((float)range - num3) / (float)range * (float)base.me.Strength * 4f, base.me.Strength);
					HitManager.DamageRegion(c, k, strength, StatusEffects.Burning, base.me.ID, forceDamage: true);
				}
			}
		}

		public override void Die(ParticleManager pMan, Texture2D texture)
		{
			if (base.me.Definition.charType == CharacterType.RemainsBomb)
			{
				this.Explode(Game1.character, 400, base.me.Location - new Vector2(0f, 300f));
			}
			int num = ((base.me.Definition.charType == CharacterType.RemainsHalf) ? 1 : 0);
			for (int i = num; i < 4; i++)
			{
				pMan.AddPlayerDebris(base.me.Location + Rand.GetRandomVector2(-base.me.DefaultWidth / 2, base.me.DefaultWidth / 2, -base.me.DefaultHeight, 0f), base.me.Trajectory + Rand.GetRandomVector2(-400f, 400f, -1000f, -400f), texture, new Rectangle(i * 150 + 210, 60, 90, 90), 1f, Rand.GetRandomFloat(0.75f, 1.5f), 1, base.me.defaultColor.R, base.me.defaultColor.G, base.me.defaultColor.B, 5);
			}
			for (int j = 0; j < 12; j++)
			{
				Rectangle sRect;
				switch (j)
				{
				default:
					sRect = new Rectangle(1950, 30, 38, 90);
					break;
				case 3:
				case 4:
				case 5:
					sRect = new Rectangle(1988, 30, 38, 90);
					break;
				case 6:
				case 7:
				case 8:
					sRect = new Rectangle(2024, 30, 25, 90);
					break;
				case 9:
				case 10:
				case 11:
					sRect = new Rectangle(2048, 62, 48, 48);
					break;
				}
				pMan.AddPlayerDebris(base.me.Location + Rand.GetRandomVector2(-base.me.DefaultWidth / 2, base.me.DefaultWidth / 2, -base.me.DefaultHeight, 0f), base.me.Trajectory + Rand.GetRandomVector2(-400f, 400f, -1000f, -400f), texture, sRect, 1f, Rand.GetRandomFloat(0.75f, 1.5f), 1, base.me.defaultColor.R, base.me.defaultColor.G, base.me.defaultColor.B, 5);
			}
		}
	}
}
