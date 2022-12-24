using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class EmitFlame : Particle
	{
		private int sRectOffsetX;

		private int type;

		private int frameRate;

		private int animFrame;

		private float animFrameTime;

		private float lifeSpan;

		private float heatColor;

		private float g;

		private float b;

		private float size;

		private float rotation;

		private bool inCollision;

		private Vector2 ploc;

		private Rectangle sRect;

		private Rectangle renderRect;

		public EmitFlame(Vector2 loc, Vector2 traj, float rot, float size, bool colliding, int type)
		{
			this.Reset(loc, traj, rot, size, colliding, type);
		}

		public void Reset(Vector2 loc, Vector2 traj, float rot, float size, bool colliding, int type)
		{
			base.exists = Exists.Init;
			this.type = type;
			this.animFrame = 0;
			this.sRectOffsetX = 1740 + Rand.GetRandomInt(0, 2) * 1024;
			this.frameRate = 0;
			base.location = (this.ploc = loc);
			this.rotation = rot + 1.57f;
			this.g = Rand.GetRandomFloat(0.25f, 1f);
			this.b = Rand.GetRandomFloat(0f, 0.5f);
			this.animFrameTime = 0f;
			this.inCollision = colliding;
			if (type == 0)
			{
				this.lifeSpan = Rand.GetRandomFloat(0.5f, 2f);
				this.animFrameTime = this.lifeSpan;
				base.renderState = RenderState.Normal;
				this.size = size * Rand.GetRandomFloat(0.5f, 1.2f);
				base.trajectory = traj + Rand.GetRandomVector2(-300f, 300f, -300f, 300f);
				this.sRect = new Rectangle(1728, 128, 32, 32);
			}
			else
			{
				switch (type)
				{
				case 1:
					this.size = size / 4f;
					base.trajectory = traj / 10f;
					break;
				case 2:
					this.size = size;
					base.trajectory = traj;
					base.strength = (int)MathHelper.Clamp(Math.Min((float)Game1.character[0].MaxHP * Game1.stats.bonusHealth, 9999f) * 0.15f, 0f, 60f);
					this.heatColor = Rand.GetRandomFloat(0f, 0.2f);
					base.flag = Rand.GetRandomInt(1, 5);
					base.statusType = StatusEffects.Burning;
					break;
				}
				if (Rand.GetRandomInt(0, 10) == 0)
				{
					this.frameRate = Rand.GetRandomInt(12, 36);
					base.renderState = RenderState.RefractOnly;
				}
				else
				{
					this.frameRate = 28;
					base.renderState = RenderState.Additive;
				}
				this.sRect = new Rectangle(0, 2074, 64, 126);
			}
			this.renderRect = new Rectangle((int)(-100f * Game1.hiDefScaleOffset), (int)(-100f * Game1.hiDefScaleOffset), Game1.screenWidth + (int)(200f * Game1.hiDefScaleOffset), Game1.screenHeight + (int)(200f * Game1.hiDefScaleOffset));
			base.exists = Exists.Exists;
		}

		private bool CheckCol(Map map)
		{
			if (!this.inCollision)
			{
				if (map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) > 0f)
				{
					return true;
				}
			}
			else if (map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) == 0f)
			{
				this.inCollision = false;
			}
			return false;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.location * Game1.worldScale - Game1.Scroll;
			if ((vector.X > (float)this.renderRect.Width && base.trajectory.X > 0f) || (vector.X < (float)this.renderRect.X && base.trajectory.X < 0f) || (vector.Y > (float)this.renderRect.Height && base.trajectory.Y > 0f) || (vector.Y < (float)this.renderRect.Y && base.trajectory.Y < 0f))
			{
				base.Reset();
				return;
			}
			bool flag = this.renderRect.Contains((int)vector.X, (int)vector.Y);
			if (this.type == 0)
			{
				base.trajectory.Y += gameTime * 1000f;
				base.location += base.trajectory * gameTime;
				this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f;
				this.animFrameTime -= gameTime;
				if (this.animFrameTime < 0f)
				{
					base.Reset();
				}
				if (!this.CheckCol(map))
				{
					return;
				}
				if (base.trajectory.Y > 500f && flag)
				{
					this.Reset(base.location, Rand.GetRandomVector2(-100f, 100f, -300f, -100f), 1.57f, 0.3f, colliding: false, 0);
					for (int i = 0; i < 4; i++)
					{
						pMan.AddEmitFlame(base.location, Rand.GetRandomVector2(-100f, 100f, -300f, -100f), 1.57f, 0.3f, colliding: false, 0, 6);
					}
				}
				else
				{
					base.Reset();
				}
				return;
			}
			this.ploc = base.location;
			base.trajectory.Y -= gameTime * 200f;
			base.location += base.trajectory * gameTime;
			if (this.type == 2 && base.renderState == RenderState.Additive)
			{
				this.heatColor += gameTime * 1.5f;
				if (this.heatColor > 1f)
				{
					this.heatColor = 1f;
				}
				if (this.CheckCol(map))
				{
					if (flag && Rand.GetRandomInt(0, 2) == 0)
					{
						pMan.AddExplosion(base.location, 1.5f, (Rand.GetRandomInt(0, 3) == 0) ? true : false, 6);
						this.Reset(base.location, Rand.GetRandomVector2(-400f, 400f, -1000f, -200f), 1.57f, 0.3f, colliding: false, 0);
					}
					else
					{
						base.Reset();
					}
				}
				if (base.flag == Game1.longSkipFrame && this.animFrame < 13 && HitManager.CheckHazard(this, c, pMan, canEvade: false))
				{
					base.Reset();
					Sound.PlayCue("enemy_die_flame", base.location, (c[0].Location - base.location).Length());
					for (int j = 0; j < 6; j++)
					{
						pMan.AddExplosion(base.location + Rand.GetRandomVector2(-100f, 100f, -c[0].Height, 0f), Rand.GetRandomFloat(0.5f, 1.5f), makeSmoke: false, 5);
					}
				}
			}
			this.animFrameTime += gameTime * (float)this.frameRate;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 15)
				{
					base.Reset();
				}
				this.animFrameTime = 0f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.type > 0)
			{
				this.sRect.X = this.animFrame * 64 + this.sRectOffsetX;
				if (!Game1.refractive)
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(this.heatColor, this.g * this.heatColor, this.b, 1f), this.rotation, new Vector2(32f, 96f), new Vector2(this.size * 2f, this.size * 1.2f) * worldScale, SpriteEffects.None, 1f);
				}
				else
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(1f, 1f, 1f, 0.1f), this.rotation, new Vector2(32f, 64f), new Vector2(this.size * 6f, this.size * 4f) * worldScale, SpriteEffects.None, 1f);
				}
			}
			else
			{
				sprite.Draw(particlesTex[1], base.GameLocation(l), this.sRect, new Color(1f, 1f, 1f, 1f * (this.animFrameTime / (this.lifeSpan * 0.2f))), this.rotation, new Vector2(16f, 16f), new Vector2(this.size, this.size * 3f) * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
