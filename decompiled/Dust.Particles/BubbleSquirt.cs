using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class BubbleSquirt : Particle
	{
		private SpriteEffects spriteDir;

		private float lifeSpan;

		private Vector2 pLoc;

		private Rectangle sRect;

		private Rectangle fadeRect;

		private bool bounce;

		private byte type;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float a;

		private float size;

		private float rotation;

		private Rectangle renderRect;

		public BubbleSquirt(Vector2 loc, Vector2 traj, float size, int owner, int type)
		{
			this.Reset(loc, traj, size, owner, type);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size, int owner, int type)
		{
			base.exists = Exists.Init;
			this.lifeSpan = Rand.GetRandomFloat(0.6f, 2f);
			this.r = (this.g = (this.b = (this.a = 1f)));
			this.bounce = false;
			base.background = false;
			this.size = size * 1.25f;
			base.location = (this.pLoc = loc + Rand.GetRandomVector2(-40f, 40f, -40f, 40f) * size);
			this.frame = this.lifeSpan;
			base.trajectory = traj * size;
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory);
			base.renderState = RenderState.AllEffects;
			base.flag = Rand.GetRandomInt(1, 5);
			base.owner = owner;
			if (owner == -1)
			{
				int num = (int)Math.Min((float)Game1.character[0].MaxHP * Game1.stats.bonusHealth, 9999f);
				base.strength = (int)MathHelper.Clamp((float)num * 0.15f, 0f, 60f);
				this.type = (byte)type;
				if (type == 1)
				{
					this.r = 0.4f;
				}
				else
				{
					this.g = (this.b = 0.4f);
					if (Rand.GetRandomInt(0, 10) < 4)
					{
						base.statusType = StatusEffects.Poison;
					}
				}
			}
			else
			{
				this.size *= 1.25f;
				base.strength = Game1.character[owner].Strength / 4;
				this.g = (this.b = 0.4f);
				if (Rand.GetRandomInt(0, 10) < 4)
				{
					base.statusType = StatusEffects.Poison;
				}
			}
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.spriteDir = SpriteEffects.FlipVertically;
			}
			else
			{
				this.spriteDir = SpriteEffects.None;
			}
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.sRect = new Rectangle(3780, 538, 180, 62);
			}
			else
			{
				this.sRect = new Rectangle(3960, 538, 117, 62);
			}
			base.maskGlow = 0.01f;
			this.renderRect = new Rectangle(-200, -100, Game1.screenWidth + 400, Game1.screenHeight + 200);
			this.fadeRect = new Rectangle(3780 + Rand.GetRandomInt(0, 3) * 100, 228, 100, 100);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (base.flag == Game1.longSkipFrame)
			{
				if (base.owner == -1)
				{
					if (this.type == 0)
					{
						if (this.size > 1f && !this.bounce && HitManager.CheckHazard(this, c, pMan, canEvade: true))
						{
							base.Reset();
						}
					}
					else
					{
						for (int i = 0; i < c.Length; i++)
						{
							if (c[i].Exists == CharExists.Exists && c[i].InHitBounds(base.location) && c[i].Ethereal == EtherealState.Normal && c[i].DyingFrame == -1f)
							{
								c[i].HP++;
								if (Game1.hud.hudDetails)
								{
									pMan.AddHP(c[i].Location - new Vector2(0f, 200f * Game1.worldScale), 1, critical: false, StatusEffects.Normal, 9);
								}
								if (i == 0 && Rand.GetRandomInt(0, 3) == 0)
								{
									Game1.hud.HealEffect(pMan);
								}
								base.Reset();
							}
						}
					}
				}
				else if (this.size > 0.2f && !this.bounce && HitManager.CheckHit(this, c, pMan))
				{
					this.size /= 1.75f;
					base.trajectory.Y = -600f;
					base.trajectory.X /= 4f;
					this.bounce = true;
					Sound.PlayDropCue("bubbleplant_splash", base.location, base.trajectory.Y * 2f);
				}
			}
			this.frame -= gameTime;
			if (!this.bounce)
			{
				base.trajectory.X /= 1.001f;
				this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory);
				if (map.CheckPCol(base.location, this.pLoc, canFallThrough: false, init: false) > 0f)
				{
					base.trajectory = new Vector2(0f, -120f);
					this.frame = (this.lifeSpan = 0.45f);
					this.size = Rand.GetRandomFloat(0.5f, 1.2f);
					this.sRect = new Rectangle(3780 + Rand.GetRandomInt(0, 3) * 100, 228, 100, 100);
					base.renderState = RenderState.AllEffects;
					this.bounce = true;
					if (Rand.GetRandomInt(0, 4) == 0)
					{
						Sound.PlayDropCue("footstep_water", base.location, base.trajectory.Y * 2f);
					}
				}
			}
			base.trajectory.Y += gameTime * 2000f;
			base.location += base.trajectory * gameTime;
			this.size += Game1.FrameTime / 4f;
			this.pLoc = base.location;
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float num = (this.frame - 0.2f) / this.lifeSpan;
			Vector2 position = base.GameLocation(l);
			if (!this.bounce)
			{
				float x = Math.Max(this.size * ((Math.Abs(base.trajectory.X) + Math.Abs(base.trajectory.Y)) / 2000f) * 2f, 0.5f);
				if (!Game1.refractive)
				{
					sprite.Draw(particlesTex[2], position, this.sRect, new Color(new Vector4(this.r, this.g, this.b, this.a * (1f - num) * (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan)))), this.rotation, new Vector2(30f, 31f), new Vector2(x, this.size) * (num + 0.2f) * worldScale, this.spriteDir, 1f);
				}
				float num2 = 1f;
				if (Game1.refractive)
				{
					num2 = 10f;
				}
				sprite.Draw(particlesTex[2], position, this.fadeRect, new Color(this.r, this.g, this.b, this.a * num / num2), this.frame, new Vector2(50f, 50f), (this.size - (1f - num)) * worldScale, this.spriteDir, 1f);
			}
			else if (Game1.pManager.renderingAdditive || Game1.refractive)
			{
				float num3 = num;
				float num4 = this.size;
				if (Game1.refractive)
				{
					num3 /= 8f;
					num4 = 1.5f;
				}
				sprite.Draw(particlesTex[2], position, this.sRect, new Color(this.r, this.g, this.b, this.a * num3), 0f, new Vector2(50f, 75f), new Vector2(1f - num, num * 1.5f) * num4 * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
