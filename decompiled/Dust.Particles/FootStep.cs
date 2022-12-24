using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FootStep : Particle
	{
		private SpriteEffects flip;

		private byte animFrame;

		private float animFrameTime;

		private float alpha;

		private float color;

		private float size;

		public FootStep(Vector2 loc, float size, float a)
		{
			this.Reset(loc, size, a);
		}

		public void Reset(Vector2 loc, float size, float a)
		{
			base.exists = Exists.Init;
			this.animFrame = (byte)Rand.GetRandomInt(0, 3);
			this.animFrameTime = 0f;
			base.location = loc;
			this.size = size * Rand.GetRandomFloat(0.9f, 1.1f);
			this.color = a * Rand.GetRandomFloat(0.07f, 0.12f);
			this.alpha = a * Rand.GetRandomFloat(0.8f, 1.1f);
			base.renderState = RenderState.Additive;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.flip = SpriteEffects.FlipHorizontally;
			}
			else
			{
				this.flip = SpriteEffects.None;
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.GameLocation(l);
			this.animFrameTime += gameTime * 24f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 9)
				{
					this.animFrame = 9;
					base.Reset();
				}
				this.animFrameTime = 0f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(2040 + 192 * this.animFrame, 1448, 192, 112), new Color(this.color, this.color, this.color, this.alpha), 0f, new Vector2(96f, 90f), this.size * worldScale, this.flip, 1f);
		}
	}
}
