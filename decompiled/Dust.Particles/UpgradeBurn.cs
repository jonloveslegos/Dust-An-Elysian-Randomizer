using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class UpgradeBurn : Particle
	{
		private float lifeSpan;

		private SpriteEffects spriteDir;

		private float frame;

		private float size;

		private float rotation;

		public UpgradeBurn(Vector2 loc, float _size)
		{
			this.Reset(loc, _size);
		}

		public void Reset(Vector2 loc, float _size)
		{
			base.exists = Exists.Init;
			base.location = loc;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.spriteDir = SpriteEffects.FlipVertically;
			}
			else
			{
				this.spriteDir = SpriteEffects.None;
			}
			this.rotation = Rand.GetRandomFloat(0f, 6.28f);
			this.size = _size;
			if (this.size == 0.8f)
			{
				this.lifeSpan = Rand.GetRandomFloat(0.2f, 1f);
				base.renderState = RenderState.Refract;
			}
			else
			{
				this.lifeSpan = Rand.GetRandomFloat(1f, 2f);
				base.renderState = RenderState.RefractOnly;
			}
			this.frame = this.lifeSpan;
			if (this.size < 0.8f)
			{
				this.frame *= this.size;
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			this.size += gameTime;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (l != 9 || Game1.hud.eventLeftOffset == 0f)
			{
				float num = (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan));
				if (Game1.refractive)
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(3997, 1860, 99, 150), new Color(new Vector4(1f, 1f, 1f, 0.2f * num)), this.rotation, new Vector2(-8f, 75f), this.size * worldScale, this.spriteDir, 1f);
				}
				else
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(3997, 1860, 99, 150), new Color(new Vector4(0.5f, 1f, 1f, 0.5f * num)), this.rotation, new Vector2(-8f, 75f), this.size * worldScale, this.spriteDir, 1f);
				}
			}
		}
	}
}
