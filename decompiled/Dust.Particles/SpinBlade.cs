using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class SpinBlade : Particle
	{
		private float frame;

		private float size;

		private float rotation;

		private float lifeSpan;

		private float origX;

		private SpriteEffects spriteDir;

		public SpinBlade(Vector2 loc, CharDir dir)
		{
			this.Reset(loc, dir);
		}

		public void Reset(Vector2 loc, CharDir dir)
		{
			base.exists = Exists.Init;
			this.lifeSpan = Rand.GetRandomFloat(0.2f, 0.4f);
			base.location = new Vector2(loc.X - 16f, loc.Y);
			base.trajectory = new Vector2(Rand.GetRandomFloat(-1400f, 1400f), Rand.GetRandomFloat(-1400f, 1400f));
			if ((dir == CharDir.Right && base.trajectory.X < 0f) || (dir == CharDir.Left && base.trajectory.X > 0f))
			{
				base.trajectory.X /= 2f;
			}
			if (dir == CharDir.Left)
			{
				this.origX = 0f;
				this.spriteDir = SpriteEffects.FlipHorizontally;
			}
			else
			{
				this.origX = 316f;
				this.spriteDir = SpriteEffects.None;
			}
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f;
			this.size = Game1.hiDefScaleOffset;
			this.frame = this.lifeSpan;
			base.renderState = RenderState.Refract;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			this.size += Math.Abs((base.trajectory.X + base.trajectory.Y) / 2f) / 2000f;
			base.trajectory *= 0.9f;
			base.location += base.trajectory * gameTime;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float num = (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan));
			if (Game1.refractive)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(3780, 0, 316, 127), new Color(1f, 1f, 1f, 0.5f * num), this.rotation, new Vector2(this.origX, 0f), this.size, this.spriteDir, 1f);
			}
			else
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(3780, 0, 316, 127), new Color(1f, 1f, 1f, 0.25f * num), this.rotation, new Vector2(this.origX, 0f), this.size, this.spriteDir, 1f);
			}
		}
	}
}
