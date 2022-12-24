using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class EmitLava : Particle
	{
		private int type;

		private float lifeSpan;

		private float frame;

		private float g;

		private float b;

		private float size;

		private float rotation;

		private bool inCollision;

		private Vector2 ploc;

		private Rectangle sRect;

		private Rectangle renderRect;

		public EmitLava(Vector2 loc, Vector2 traj, float size, bool colliding, int type)
		{
			this.Reset(loc, traj, size, colliding, type);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size, bool colliding, int type)
		{
			base.exists = Exists.Init;
			this.type = type;
			base.location = (this.ploc = loc + Rand.GetRandomVector2(-40f, 40f, -40f, 40f));
			this.inCollision = colliding;
			if (type < 2)
			{
				this.rotation = Rand.GetRandomFloat(0f, 6.14f);
				this.lifeSpan = Rand.GetRandomFloat(0.5f, 2f);
				this.frame = this.lifeSpan;
				this.sRect = new Rectangle(594 + 12 * Rand.GetRandomInt(0, 4), 3628, 12, 20);
				if (type == 0)
				{
					this.size = _size * Rand.GetRandomFloat(1f, 6f);
					base.renderState = RenderState.AdditiveOnly;
					if (traj.Y < -300f)
					{
						traj.Y *= 1.5f;
					}
				}
				else
				{
					base.renderState = RenderState.Normal;
					this.g = 0.7f;
					this.b = 0f;
					if (Rand.GetRandomInt(0, 3) == 0)
					{
						this.size = _size * Rand.GetRandomFloat(0.2f, 1f);
						this.sRect = new Rectangle(964 + 120 * Rand.GetRandomInt(0, 3), 3840, 120, 120);
						traj *= 0.8f;
					}
					else
					{
						this.size = _size * Rand.GetRandomFloat(0.5f, 1.5f);
					}
				}
				base.trajectory = traj + Rand.GetRandomVector2(-100f, 100f, -20f, 20f);
			}
			else
			{
				this.g = Rand.GetRandomFloat(0.5f, 1f);
				this.b = Rand.GetRandomFloat(0f, Math.Min(this.g, 0.5f));
				this.size = _size * Rand.GetRandomFloat(0.75f, 1.2f);
				base.trajectory = traj * Rand.GetRandomFloat(1.2f, 1.4f);
				base.strength = (int)MathHelper.Clamp(Math.Min((float)Game1.character[0].MaxHP * Game1.stats.bonusHealth, 9999f) * 0.4f, 0f, 200f);
				base.flag = Rand.GetRandomInt(1, 5);
				base.statusType = StatusEffects.Burning;
				this.frame = 0f;
				base.renderState = RenderState.Normal;
				this.sRect = new Rectangle(964 + 120 * Rand.GetRandomInt(0, 3), 3840, 120, 120);
				if (type == 3)
				{
					base.flag = 0;
					this.frame = 1f;
					this.Splash(inBounds: true, 0);
					base.renderState = RenderState.Normal;
					base.location = loc;
				}
			}
			this.renderRect = new Rectangle((int)(-100f * Game1.hiDefScaleOffset), (int)(-100f * Game1.hiDefScaleOffset), Game1.screenWidth + (int)(200f * Game1.hiDefScaleOffset), Game1.screenHeight + (int)(200f * Game1.hiDefScaleOffset));
			base.exists = Exists.Exists;
		}

		private bool CheckCol(Map map, int l)
		{
			Vector2 vector = ((this.type == 0) ? Vector2.Zero : new Vector2(0f, (float)this.sRect.Height * this.size / 3f));
			float num = 0f;
			if (base.trajectory.Y > 0f)
			{
				num = ((l != 5 && l != 6) ? map.CheckSpecialLedge(base.PlayerLayerLoc(base.location, l) + vector, base.PlayerLayerLoc(this.ploc, l) + vector, LedgeFlags.SpecialPath) : map.CheckPCol(base.location + vector, this.ploc + vector, canFallThrough: false, init: false));
			}
			if (!this.inCollision)
			{
				if (num > 0f)
				{
					return true;
				}
			}
			else if (num == 0f)
			{
				this.inCollision = false;
			}
			return false;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if ((vector.X > (float)this.renderRect.Width && base.trajectory.X > 0f) || (vector.X < (float)this.renderRect.X && base.trajectory.X < 0f) || (vector.Y > (float)this.renderRect.Height && base.trajectory.Y > 0f))
			{
				base.Reset();
				return;
			}
			bool flag = this.renderRect.Contains((int)vector.X, (int)vector.Y);
			if (this.type == 0)
			{
				base.trajectory.Y += gameTime * 1200f;
				this.ploc = base.location;
				base.location += base.trajectory * gameTime;
				this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f;
				this.frame -= gameTime;
				if (this.frame < 0f)
				{
					base.Reset();
				}
				if (!this.CheckCol(map, l))
				{
					return;
				}
				if (base.trajectory.Y > 700f && flag)
				{
					this.Reset(this.ploc, Rand.GetRandomVector2(-200f, 200f, -400f, -200f), 0.3f, colliding: false, 0);
					for (int i = 0; i < 3; i++)
					{
						pMan.AddEmitLava(this.ploc, Rand.GetRandomVector2(-200f, 200f, -400f, -200f), 0.3f, colliding: false, 0, (l > 4) ? 6 : l);
					}
				}
				else
				{
					base.Reset();
				}
				return;
			}
			if (this.type == 3)
			{
				base.trajectory.Y += gameTime * 2000f * (this.size / 8f);
				base.location += base.trajectory * gameTime;
				this.g -= gameTime * 2f;
				this.frame -= gameTime * 2f;
				if (this.frame < 0f)
				{
					base.Reset();
				}
				return;
			}
			this.ploc = base.location;
			base.trajectory.X *= 0.99f;
			if (base.trajectory.Y < 1000f)
			{
				base.trajectory.Y += gameTime * 1000f;
			}
			base.location += base.trajectory * gameTime;
			if (base.trajectory.X > 0f)
			{
				this.rotation += gameTime / this.size;
			}
			else
			{
				this.rotation -= gameTime / this.size;
			}
			if (base.trajectory.Y < 0f)
			{
				this.frame += gameTime;
			}
			else
			{
				this.frame += gameTime * 2f;
			}
			if (this.frame > 1f)
			{
				this.frame = 1f;
			}
			this.g -= gameTime * 0.3f;
			this.b -= gameTime * 0.4f;
			this.size += gameTime * 0.4f;
			if (this.CheckCol(map, l))
			{
				if (this.sRect.Width == 12)
				{
					base.Reset();
				}
				else
				{
					this.Splash(flag, (this.type == 2) ? Rand.GetRandomInt(-100, 100) : 0);
				}
			}
			if (this.type == 2 && l == 5 && base.flag == Game1.longSkipFrame && HitManager.CheckHazard(this, c, pMan, canEvade: false))
			{
				base.Reset();
				for (int j = 0; j < 6; j++)
				{
					pMan.AddExplosion(base.location + Rand.GetRandomVector2(-100f, 100f, -c[0].Height, 0f), Rand.GetRandomFloat(0.5f, 1.5f), makeSmoke: false, 5);
				}
			}
		}

		private void Splash(bool inBounds, int xOffset)
		{
			if (inBounds && base.flag < 3)
			{
				base.location.X += xOffset;
				base.location.Y += (float)this.sRect.Height * this.size / 3f;
				this.sRect = new Rectangle(642 + Rand.GetRandomInt(0, 2) * 120, 3630 + Rand.GetRandomInt(0, 2) * 60, 120, 60);
				base.trajectory.X = MathHelper.Clamp(base.trajectory.X, -200f, 200f);
				base.trajectory.Y = -400f * (this.size / 4f);
				this.g = 1f;
				this.b = 0f;
				this.size = Math.Min(this.size * 4f, 4f);
				base.renderState = RenderState.AdditiveOnly;
				this.type = 3;
			}
			else
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			switch (this.type)
			{
			default:
				sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, Color.White, this.rotation, new Vector2(6f, 10f), this.size * worldScale * this.frame, SpriteEffects.None, 1f);
				break;
			case 1:
				sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(1f, this.g, this.b, 1f), this.rotation, new Vector2(this.sRect.Width, this.sRect.Height) / 2f, this.size * worldScale * Math.Min(this.frame, 1f), SpriteEffects.None, 1f);
				break;
			case 2:
			{
				float num = 1f - this.frame;
				Vector2 position = base.GameLocation(l);
				if (num > 0f)
				{
					sprite.Draw(particlesTex[2], position, new Rectangle(1324, 3900, 120, 60), new Color(1f, this.g, this.b, num), GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory), new Vector2(40f, 30f), this.size * 2f * worldScale, SpriteEffects.None, 1f);
				}
				sprite.Draw(particlesTex[2], position, this.sRect, new Color(1f, this.g, this.b, this.frame * 2f * (1f - num)), this.rotation, new Vector2(32f, 96f), this.size * worldScale, SpriteEffects.None, 1f);
				break;
			}
			case 3:
				sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(1f, this.g, this.b, this.frame), 0f, new Vector2(60f, 50f), this.size * new Vector2(1f - this.frame, Math.Max(this.frame, 0.5f)) * worldScale, SpriteEffects.None, 1f);
				break;
			}
		}
	}
}
