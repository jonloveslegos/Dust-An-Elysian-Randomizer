using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class EmitRocks : Particle
	{
		private Rectangle sRect;

		private float sizeY;

		private SpriteEffects flip;

		private Vector2 ploc;

		private int rotateSpeed;

		private int contactFrame;

		private float size;

		private float rotation;

		private bool inCollision;

		private Rectangle renderRect;

		public EmitRocks(Vector2 loc, Vector2 traj, float rot, float _size, int type)
		{
			this.Reset(loc, traj, rot, _size, type);
		}

		public void Reset(Vector2 loc, Vector2 traj, float rot, float _size, int type)
		{
			base.exists = Exists.Init;
			this.flip = SpriteEffects.None;
			this.inCollision = true;
			base.location = (this.ploc = loc);
			base.trajectory = traj * Rand.GetRandomFloat(0.7f, 1.1f);
			traj.Y /= 2f;
			this.contactFrame = Rand.GetRandomInt(1, 5);
			this.rotation = Rand.GetRandomFloat(0f, 6.28f);
			this.rotateSpeed = Rand.GetRandomInt(1, 6);
			base.flag = type;
			if (base.flag == 2)
			{
				this.size = Rand.GetRandomFloat(0.3f, 1f) * _size;
				base.strength = (int)MathHelper.Clamp(Math.Min((float)Game1.character[0].MaxHP * Game1.stats.bonusHealth, 9999f) * 0.15f, 0f, 60f);
				this.sRect = new Rectangle(Rand.GetRandomInt(0, 3) * 72 + 2368, 2724, 72, 72);
				this.sRect = new Rectangle(392 + Rand.GetRandomInt(0, 3) * 64 + 1984, 2660, 64, 64);
			}
			else
			{
				this.size = Rand.GetRandomFloat(0.2f, 0.8f) * _size;
				this.sRect = new Rectangle(2584, Rand.GetRandomInt(0, 3) * 24 + 2724, 40, 24);
				this.sRect = new Rectangle(392 + Rand.GetRandomInt(0, 3) * 64 + 1984, 2660, 64, 64);
				this.size = Rand.GetRandomFloat(0.1f, 0.2f) * _size;
			}
			this.sizeY = Rand.GetRandomFloat(0.5f, 1f) * this.size;
			base.renderState = RenderState.Normal;
			this.renderRect = new Rectangle((int)(-20f * Game1.hiDefScaleOffset), (int)(-100f * Game1.hiDefScaleOffset), Game1.screenWidth + (int)(40f * Game1.hiDefScaleOffset), Game1.screenHeight + (int)(200f * Game1.hiDefScaleOffset));
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
			}
			base.trajectory.Y += gameTime * 2000f;
			if (base.trajectory.X > 0f)
			{
				this.rotation += gameTime * (float)this.rotateSpeed;
			}
			else
			{
				this.rotation -= gameTime * (float)this.rotateSpeed;
			}
			if (map.CheckCol(base.location) > 0)
			{
				base.location.X = this.ploc.X;
				base.trajectory.X = 0f - base.trajectory.X;
			}
			if (this.CheckCol(map) || base.location.Y > map.bottomEdge)
			{
				if (this.size > 0.4f)
				{
					float num = base.trajectory.Y * -0.5f;
					base.trajectory.Y = num;
					this.size /= 1.5f;
					this.sizeY /= 1.5f;
					this.sRect = new Rectangle(2584, Rand.GetRandomInt(0, 3) * 24 + 2724, 40, 24);
					this.sRect = new Rectangle(392 + Rand.GetRandomInt(0, 3) * 64 + 1984, 2660, 64, 64);
					this.size = Rand.GetRandomFloat(0.1f, 0.4f) * this.size;
					this.sizeY = this.size;
					base.flag = 0;
					if (Rand.GetRandomInt(0, 2) == 0)
					{
						base.trajectory.X *= -1f;
						pMan.AddEmitRocks(base.location - new Vector2(0f, 20f), new Vector2(Rand.GetRandomFloat(0f - base.trajectory.X, base.trajectory.X), num / 1.5f), 0f, this.size, 1, l);
					}
					if (Rand.GetRandomInt(0, 6) == 0)
					{
						Sound.PlayDropCue("destruct_debris", base.location, this.size * 1200f);
					}
				}
				else
				{
					base.Reset();
				}
			}
			if (base.flag == 2 && this.contactFrame == Game1.longSkipFrame)
			{
				HitManager.CheckHazard(this, c, pMan, canEvade: false);
			}
			this.ploc = base.location;
			base.location += base.trajectory * gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, Color.White, this.rotation, new Vector2(32f, 32f), new Vector2(this.size, this.sizeY) * worldScale, SpriteEffects.None, 1f);
		}
	}
}
