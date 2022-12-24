using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Explosion : Particle
	{
		private byte animFrame;

		private byte frameRate;

		private float animFrameTime;

		private float lifeSpan;

		private float frame;

		private float size;

		private float rotation;

		private Rectangle renderRect;

		private SpriteEffects spriteDir;

		public Explosion(Vector2 loc, float size, bool makeSmoke)
		{
			this.Reset(loc, size, makeSmoke);
		}

		public void Reset(Vector2 loc, float size, bool makeSmoke)
		{
			base.exists = Exists.Init;
			base.flag = (makeSmoke ? 1 : 0);
			if (base.flag > 0)
			{
				base.flag = Rand.GetRandomInt(1, 5);
				this.frame = (this.lifeSpan = 2f * Rand.GetRandomFloat(0.75f, 1.25f));
				base.trajectory = Rand.GetRandomVector2(-20f, 20f, -120f, 0f);
			}
			this.animFrame = 0;
			this.animFrameTime = 0f;
			base.location = loc;
			this.size = size * Rand.GetRandomFloat(0.75f, 1.25f);
			this.renderRect = new Rectangle(-100, -100, Game1.screenWidth + 200, Game1.screenHeight + 200);
			if (Rand.GetRandomInt(0, 10) == 0 && base.flag == 0)
			{
				base.renderState = RenderState.RefractOnly;
				this.frameRate = 48;
				this.size *= 1.5f;
			}
			else
			{
				if (base.flag > 0)
				{
					base.renderState = RenderState.Additive;
				}
				else
				{
					base.renderState = RenderState.Normal;
				}
				this.frameRate = (byte)Rand.GetRandomInt(24, 36);
			}
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.spriteDir = SpriteEffects.FlipHorizontally;
			}
			else
			{
				this.spriteDir = SpriteEffects.None;
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || (base.flag > 0 && this.frame < 0f))
			{
				base.Reset();
			}
			this.animFrameTime += gameTime * (float)(int)this.frameRate;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				this.animFrameTime = 0f;
				if (this.animFrame > 18)
				{
					this.animFrame = 18;
					if (base.flag == 0)
					{
						base.Reset();
					}
				}
			}
			if (base.flag > 0)
			{
				this.frame -= gameTime;
				base.location += base.trajectory * gameTime;
				if (base.trajectory.X < 0f)
				{
					this.rotation += base.trajectory.Y * Game1.FrameTime / 200f;
				}
				else
				{
					this.rotation -= base.trajectory.Y * Game1.FrameTime / 200f;
				}
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 position = base.GameLocation(l);
			if (base.flag > 0)
			{
				int num = base.flag - 1;
				sprite.Draw(particlesTex[2], position, new Rectangle(910 + num * 120 - num / 2 * 240, 1000 + num / 2 * 120, 120, 120), new Color(1f, 1f, 1f, 0.5f * (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan))), this.rotation, new Vector2(60f, 60f), this.size, this.spriteDir, 1f);
			}
			if (this.animFrame < 19 && !Game1.pManager.renderingAdditive)
			{
				sprite.Draw(particlesTex[2], position, new Rectangle(2940 + 60 * this.animFrame, 1660, 60, 200), Color.White, this.rotation, new Vector2(30f, 100f), new Vector2(this.size * 3.33f, this.size) * worldScale, this.spriteDir, 1f);
			}
		}
	}
}
