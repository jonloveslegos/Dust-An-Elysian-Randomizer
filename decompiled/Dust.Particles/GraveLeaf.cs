using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class GraveLeaf : Particle
	{
		private byte animFrame;

		private float animFrameTime;

		private float lifeSpan;

		private float frame;

		private float size;

		private float rotation;

		private float originOffset;

		private Rectangle renderRect;

		public GraveLeaf(Vector2 loc, Vector2 traj, float size)
		{
			this.Reset(loc, traj, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size)
		{
			base.exists = Exists.Init;
			this.lifeSpan = Rand.GetRandomFloat(12f, 20f);
			this.animFrame = (byte)Rand.GetRandomInt(0, 45);
			this.animFrameTime = 0f;
			this.ResetPos(loc);
			if (!base.CanPrecipitate(Game1.character[0].Location, 5))
			{
				base.exists = Exists.Dead;
				base.Reset();
				return;
			}
			base.trajectory = traj;
			this.size = size;
			base.flag = Rand.GetRandomInt(0, 1000);
			this.frame = this.lifeSpan;
			base.renderState = RenderState.Normal;
			base.exists = Exists.Exists;
		}

		private void ResetPos(Vector2 loc)
		{
			this.renderRect = new Rectangle(-200, -1000, Game1.screenWidth + 1000, Game1.screenHeight + 1000);
			base.location = Game1.Scroll;
			base.location += new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - 1f / base.LayerScale(8)) * (Game1.hiDefScaleOffset - Game1.worldScale);
			base.location /= Game1.worldScale;
			base.location *= base.LayerScale(8);
			base.location += loc;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
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
			base.location += (base.trajectory - new Vector2(0f, this.originOffset)) * gameTime;
			this.originOffset = (float)Math.Cos((double)map.MapSegFrame * 20.0 + (double)base.flag) * 100f;
			this.rotation += gameTime * 5f;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(1468 + this.animFrame * 32, 4064, 32, 32), Color.Gray * (this.frame / this.lifeSpan) * 5f, this.rotation, new Vector2(16f + this.originOffset * this.size, 16f), this.size * worldScale, SpriteEffects.None, 1f);
		}
	}
}
