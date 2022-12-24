using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FootStepWater : Particle
	{
		private SpriteEffects spriteDir;

		private float lifeSpan;

		private Rectangle sRect;

		private float frame;

		private float size;

		public FootStepWater(Vector2 loc, float size)
		{
			this.Reset(loc, size);
		}

		public void Reset(Vector2 loc, float _size)
		{
			base.exists = Exists.Init;
			base.background = false;
			base.location = loc;
			this.frame = (this.lifeSpan = Rand.GetRandomFloat(0.35f, 0.65f));
			this.size = Rand.GetRandomFloat(0.9f, 1.2f) * _size;
			this.sRect = new Rectangle(3780 + Rand.GetRandomInt(0, 3) * 100, 228, 100, 100);
			base.renderState = RenderState.AllEffects;
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
			this.frame -= gameTime;
			this.size += Game1.FrameTime / 4f;
			if (this.frame < 0f)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (Game1.pManager.renderingAdditive || Game1.refractive)
			{
				float num = (this.frame - 0.2f) / this.lifeSpan;
				Vector2 position = base.GameLocation(l);
				float num2 = num;
				float num3 = this.size;
				if (Game1.refractive)
				{
					num2 /= 8f;
					num3 = 1.5f;
				}
				sprite.Draw(particlesTex[2], position, this.sRect, new Color(1f, 1f, 1f, num2), 0f, new Vector2(50f, 75f), new Vector2(1f - num, num) * num3 * worldScale, this.spriteDir, 1f);
			}
		}
	}
}
