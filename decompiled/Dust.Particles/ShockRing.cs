using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class ShockRing : Particle
	{
		private float LifeSpan;

		private float frame;

		private float size;

		public ShockRing(Vector2 loc, float lifeSpan)
		{
			this.Reset(loc, lifeSpan);
		}

		public void Reset(Vector2 loc, float lifeSpan)
		{
			base.exists = Exists.Init;
			base.location = new Vector2(loc.X - 16f, loc.Y);
			this.size = 0.1f;
			this.frame = (this.LifeSpan = lifeSpan);
			base.renderState = RenderState.RefractOnly;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
				map.shockRingCount = Math.Max(map.shockRingCount - 1, 0);
			}
			this.frame -= gameTime;
			this.size += this.frame / this.size / 5f;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(0, 1660, 540, 540), new Color(1f, 1f, 1f, this.LifeSpan * (this.frame / this.LifeSpan) * worldScale), 0f, new Vector2(270f, 270f), new Vector2(this.size, this.size * 0.75f) * worldScale, SpriteEffects.None, 1f);
		}
	}
}
