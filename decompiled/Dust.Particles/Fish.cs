using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Fish : Particle
	{
		private float frameAlpha;

		private byte animFrame;

		private int facingRight;

		private int prevFacing;

		private int srectOffset;

		private byte moveSpeed;

		private bool isTurning;

		private float animFrameTime;

		private float r;

		private float g;

		private float b;

		private float size;

		private float rotation;

		private Rectangle renderRect;

		public Fish(Vector2 loc, Vector2 traj, float size, int id)
		{
			this.Reset(loc, traj, size, id);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size, int id)
		{
			base.exists = Exists.Init;
			this.frameAlpha = 0f;
			this.animFrame = 0;
			this.facingRight = 1;
			this.prevFacing = 0;
			this.srectOffset = 0;
			this.moveSpeed = (byte)Rand.GetRandomInt(20, 40);
			this.isTurning = false;
			base.location = loc;
			base.trajectory = traj;
			this.r = Rand.GetRandomFloat(0f, 0.25f);
			this.g = Rand.GetRandomFloat(0f, 0.25f);
			this.b = Rand.GetRandomFloat(0.25f, 0.75f);
			this.size = size;
			base.owner = id;
			this.animFrameTime = 0f;
			this.renderRect = new Rectangle(-500, 0, Game1.screenWidth + (int)(1000f * Game1.hiDefScaleOffset), Game1.screenHeight + 200);
			Game1.map.fishCount++;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			float num = (float)Math.Cos((double)map.MapSegFrame * 5.0 + (double)base.owner);
			float num2 = (float)Math.Sin((double)map.MapSegFrame * 30.0 + (double)base.owner);
			base.trajectory = new Vector2(num * 2000f, num2 * 100f);
			base.location += base.trajectory * gameTime / (int)this.moveSpeed;
			if (Game1.Scroll.X < Game1.pScroll.X)
			{
				this.renderRect.X = -this.renderRect.Width + Game1.screenWidth + 100;
			}
			else if (Game1.Scroll.X > Game1.pScroll.X)
			{
				this.renderRect.X = -100;
			}
			else
			{
				this.renderRect.X = -Game1.screenWidth / 2;
			}
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y))
			{
				base.Reset();
				map.fishCount--;
			}
			else
			{
				if (this.frameAlpha < 1f)
				{
					this.frameAlpha += Game1.FrameTime / 1f;
					if (this.frameAlpha > 1f)
					{
						this.frameAlpha = 1f;
					}
				}
				if (base.trajectory.X > 0f)
				{
					this.facingRight = 1;
					this.rotation += gameTime / 10f;
					if (this.rotation > 0.1f)
					{
						this.rotation = 0.1f;
					}
				}
				else
				{
					this.facingRight = -1;
					this.rotation -= gameTime / 10f;
					if (this.rotation < -0.1f)
					{
						this.rotation = -0.1f;
					}
				}
				this.animFrameTime += gameTime * 12f;
				if (this.prevFacing != this.facingRight)
				{
					this.isTurning = true;
					this.srectOffset = 50;
					this.animFrame = 0;
					this.animFrameTime = 0f;
				}
				if (this.isTurning && this.animFrameTime > 1f)
				{
					this.animFrame++;
					if (this.animFrame > 10)
					{
						this.animFrame = 0;
						this.isTurning = false;
						this.srectOffset = 0;
					}
					this.animFrameTime = 0f;
				}
				if (!this.isTurning && this.animFrameTime > 1f)
				{
					this.animFrame++;
					if (this.animFrame > 11)
					{
						this.animFrame = 0;
					}
					this.animFrameTime = 0f;
				}
			}
			this.prevFacing = this.facingRight;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (base.trajectory.X > 0f)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(540 + this.animFrame * 100, 2010 + this.srectOffset, 100, 50), new Color(this.r, this.g, this.b, 0.75f * this.frameAlpha), this.rotation, new Vector2(32f, 32f), this.size * this.frameAlpha * worldScale, SpriteEffects.None, 1f);
			}
			else
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(540 + this.animFrame * 100, 2010 + this.srectOffset, 100, 50), new Color(this.r, this.g, this.b, 0.75f * this.frameAlpha), this.rotation, new Vector2(32f, 32f), this.size * this.frameAlpha * worldScale, SpriteEffects.FlipHorizontally, 1f);
			}
		}
	}
}
