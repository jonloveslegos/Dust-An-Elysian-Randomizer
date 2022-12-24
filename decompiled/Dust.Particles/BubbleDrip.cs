using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class BubbleDrip : Particle
	{
		private SpriteEffects spriteDir;

		private Rectangle sRect;

		private float lifeSpan;

		private float dripScale;

		private float frame;

		private float size;

		public BubbleDrip(Vector2 loc, float size)
		{
			this.Reset(loc, size);
		}

		public void Reset(Vector2 loc, float size)
		{
			base.exists = Exists.Init;
			this.lifeSpan = Rand.GetRandomFloat(1f, 4f);
			this.dripScale = 0.1f;
			base.background = true;
			base.location = loc;
			base.trajectory = Vector2.Zero;
			this.size = size;
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
			this.sRect = new Rectangle(3780 + Rand.GetRandomInt(0, 3) * 100, 427, 99, 111);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			this.dripScale += gameTime * 0.75f;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float a = (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan));
			sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(1f, 1f, 1f, a), 0f, new Vector2(55f, 0f), new Vector2(this.size - this.dripScale / 4f, this.dripScale * this.size) * worldScale, this.spriteDir, 1f);
		}
	}
}
