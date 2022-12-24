using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class CheckPoint : Particle
	{
		private SpriteEffects spriteDir;

		private Rectangle sRect;

		private float lifeSpan;

		private float floatScale;

		private float frame;

		private float size;

		public CheckPoint(Vector2 loc, float size)
		{
			this.Reset(loc, size);
		}

		public void Reset(Vector2 loc, float size)
		{
			base.exists = Exists.Init;
			this.lifeSpan = Rand.GetRandomFloat(2f, 5f);
			this.floatScale = 0.1f;
			base.background = true;
			base.location = loc;
			base.trajectory = Vector2.Zero;
			this.size = size;
			this.frame = this.lifeSpan;
			base.renderState = RenderState.AdditiveOnly;
			this.spriteDir = ((Rand.GetRandomInt(0, 2) == 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			base.background = Rand.GetRandomInt(0, 2) == 0;
			this.sRect = ((Rand.GetRandomInt(0, 2) == 0) ? new Rectangle(510, 1000, 229, 240) : new Rectangle(739, 1000, 173, 240));
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			this.floatScale += gameTime * 0.75f;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float a = Math.Min((float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan)), 0.3f);
			sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(0f, 1f, 1f, a), 0f, new Vector2(this.sRect.Width / 2, 240f), new Vector2(this.size - this.floatScale / 4f, this.floatScale * this.size) * worldScale, this.spriteDir, 1f);
		}
	}
}
