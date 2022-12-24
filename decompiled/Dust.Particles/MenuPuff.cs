using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class MenuPuff : Particle
	{
		private SpriteEffects spriteDir;

		private byte animFrame;

		private float animFrameTime;

		private float size;

		public MenuPuff(Vector2 loc, float size)
		{
			this.Reset(loc, size);
		}

		public void Reset(Vector2 loc, float size)
		{
			base.exists = Exists.Init;
			this.animFrame = 0;
			this.animFrameTime = 0f;
			base.location = loc;
			this.size = size * Rand.GetRandomFloat(0.5f, 1.5f);
			base.renderState = RenderState.AdditiveOnly;
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
			if (!new Rectangle(-50, -50, Game1.screenWidth + 100, Game1.screenHeight + 100).Contains((int)base.location.X, (int)base.location.Y))
			{
				base.Reset();
			}
			this.animFrameTime += Game1.HudTime * 48f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 22)
				{
					base.Reset();
				}
				this.animFrameTime = 0f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.location, new Rectangle(700 + 140 * this.animFrame, 1240, 140, 140), Color.White, 0f, new Vector2(70f, 70f), this.size, this.spriteDir, 1f);
		}
	}
}
