using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class CaveLeaf : Particle
	{
		private byte animFrame;

		private float animFrameTime;

		private float lifeSpan;

		private float frame;

		private float size;

		private float rotation;

		private Rectangle renderRect;

		public CaveLeaf(Vector2 loc, Vector2 traj, float size)
		{
			this.Reset(loc, traj, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size)
		{
			base.exists = Exists.Init;
			this.lifeSpan = 3f * Rand.GetRandomFloat(0.75f, 1.25f);
			this.animFrame = (byte)Rand.GetRandomInt(0, 45);
			this.animFrameTime = 0f;
			base.location = loc;
			base.trajectory = traj;
			this.size = size;
			this.frame = this.lifeSpan;
			base.renderState = RenderState.AdditiveOnly;
			this.renderRect = new Rectangle(-2000, -1000, Game1.screenWidth + 4000, Game1.screenHeight + 1000);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (Game1.Scroll.X < Game1.pScroll.X)
			{
				this.renderRect.X = -3900;
			}
			else if (Game1.Scroll.X > Game1.pScroll.X)
			{
				this.renderRect.X = -100;
			}
			else
			{
				this.renderRect.X = -2000;
			}
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				base.Reset();
			}
			this.animFrameTime += gameTime * 24f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 47)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			base.trajectory.X = (float)Math.Cos((double)map.MapSegFrame + (double)this.frame) * 40f;
			base.location += base.trajectory * gameTime;
			this.rotation = 2f * GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) - 135f;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float num = (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan));
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(1740 + this.animFrame * 32, 2042, 32, 32), new Color(1f, 1f, 1f, num * 2f), this.rotation, new Vector2(16f, 16f), this.size * worldScale, SpriteEffects.None, 1f);
		}
	}
}
