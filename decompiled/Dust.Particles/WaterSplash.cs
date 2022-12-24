using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class WaterSplash : Particle
	{
		private int sRectX;

		private float lifeSpan;

		private float frame;

		private float alpha;

		private float size;

		private float rotation;

		private SpriteEffects spriteDir;

		private Rectangle renderRect;

		public WaterSplash(Vector2 loc, float size, int flag)
		{
			this.Reset(loc, size, flag);
		}

		public void Reset(Vector2 loc, float size, int flag)
		{
			base.exists = Exists.Init;
			base.flag = flag;
			base.background = true;
			base.location = loc;
			this.size = size;
			if (flag == 0)
			{
				this.lifeSpan = Rand.GetRandomFloat(0.5f, 0.7f);
				base.trajectory = new Vector2(Rand.GetRandomFloat(-100f, 100f), Rand.GetRandomFloat(-400f, -200f)) * size;
			}
			else
			{
				this.lifeSpan = Rand.GetRandomFloat(0.4f, 0.7f);
				base.trajectory = new Vector2(Rand.GetRandomFloat(-200f, 200f), Rand.GetRandomFloat(-700f, -300f)) * size;
			}
			this.alpha = Rand.GetRandomFloat(0.1f, 0.3f);
			this.frame = this.lifeSpan;
			base.renderState = RenderState.AdditiveOnly;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.spriteDir = SpriteEffects.FlipHorizontally;
			}
			else
			{
				this.spriteDir = SpriteEffects.None;
			}
			this.renderRect = new Rectangle((int)(-200f * Game1.hiDefScaleOffset), (int)(-200f * Game1.hiDefScaleOffset), Game1.screenWidth + (int)(400f * Game1.hiDefScaleOffset), Game1.screenHeight + (int)(600f * Game1.hiDefScaleOffset));
			this.sRectX = 546 + Rand.GetRandomInt(0, 3) * 120;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				base.Reset();
			}
			if (base.flag == 0)
			{
				if (base.trajectory.Y < 40f)
				{
					base.trajectory.Y += gameTime * 1000f;
				}
				this.size += Game1.FrameTime / 2f;
			}
			else
			{
				base.trajectory.Y += gameTime * 1000f;
				this.size += Game1.FrameTime;
			}
			if (base.trajectory.X > 0f)
			{
				this.rotation += gameTime / 1f;
			}
			else
			{
				this.rotation -= gameTime / 1f;
			}
			this.size += Game1.FrameTime;
			base.location += base.trajectory * gameTime;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l);
			Color color = new Color(new Vector4(1f, 1f, 1f, this.alpha * (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan))));
			for (int i = 0; i < 3; i++)
			{
				int num = ((i == 1) ? 1 : 0);
				sprite.Draw(particlesTex[2], vector + new Vector2((float)i * (60f * this.size) - 60f * this.size, (float)(-num) * (40f * this.size)) * worldScale, new Rectangle(this.sRectX, 3750, 120, 120), color, this.rotation, new Vector2(60f, 60f), new Vector2(this.size + (float)num * 0.2f, this.size + (float)num * 0.6f) * worldScale, this.spriteDir, 1f);
			}
		}
	}
}
