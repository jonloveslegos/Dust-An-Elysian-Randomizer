using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class RaveBeam : Particle
	{
		private float lifeSpan;

		private float frame;

		private float r;

		private float g;

		private float b;

		private SpriteEffects direction;

		private float rot;

		public RaveBeam(Vector2 loc, float _lifeSpan)
		{
			this.Reset(loc, _lifeSpan);
		}

		public void Reset(Vector2 loc, float _lifeSpan)
		{
			base.exists = Exists.Init;
			base.location = loc;
			switch (Rand.GetRandomInt(0, 4))
			{
			default:
				this.r = (this.b = 1f);
				this.g = 0f;
				break;
			case 1:
				this.r = 0f;
				this.g = (this.b = 1f);
				break;
			case 2:
				this.r = (this.g = 1f);
				this.b = 0f;
				break;
			case 3:
				this.r = (this.g = (this.b = 1f));
				break;
			}
			this.frame = (this.lifeSpan = _lifeSpan);
			base.renderState = RenderState.AdditiveOnly;
			this.direction = ((Rand.GetRandomInt(0, 2) != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			this.rot = 0.5f * (float)((this.direction == SpriteEffects.None) ? 1 : (-1));
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			this.rot += gameTime * 0.2f * (float)((this.direction != 0) ? 1 : (-1));
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(4077, 336, 19, 264), new Color(this.r, this.g, this.b, (float)Math.Sin(6.28 * (double)this.frame / 2.0 / (double)this.lifeSpan) / 2f), this.rot, new Vector2(10f, 0f), 8f * worldScale, this.direction, 1f);
		}
	}
}
