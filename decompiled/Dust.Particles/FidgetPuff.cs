using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FidgetPuff : Particle
	{
		private SpriteEffects spriteDir;

		private byte animFrame;

		private float animFrameTime;

		private float size;

		public FidgetPuff(Vector2 loc)
		{
			this.Reset(loc);
		}

		public void Reset(Vector2 loc)
		{
			base.exists = Exists.Init;
			this.animFrame = 0;
			this.animFrameTime = 0f;
			base.location = loc;
			this.size = Rand.GetRandomFloat(0.5f, 1.5f);
			base.renderState = RenderState.AdditiveOnly;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.spriteDir = SpriteEffects.FlipHorizontally;
			}
			else
			{
				this.spriteDir = SpriteEffects.None;
			}
			base.maskGlow = 0.8f;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.maskGlow -= gameTime * 2f;
			this.animFrameTime += gameTime * 36f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 22)
				{
					this.animFrame = 22;
					base.Reset();
				}
				this.animFrameTime = 0f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(700 + 140 * this.animFrame, 1240, 140, 140), Color.White, 0f, new Vector2(70f, 70f), this.size * worldScale, this.spriteDir, 1f);
		}
	}
}
