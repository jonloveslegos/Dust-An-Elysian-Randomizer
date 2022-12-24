using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class GlowSpark : Particle
	{
		private float lifeSpan;

		private float frame;

		private float a;

		private float size;

		private Rectangle renderRect;

		public GlowSpark(Vector2 loc, Vector2 traj, float lifeSpan, float a, float size)
		{
			this.Reset(loc, traj, lifeSpan, a, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float lifeSpan, float a, float size)
		{
			base.exists = Exists.Init;
			base.location = loc;
			base.trajectory = traj;
			this.a = a;
			this.size = size;
			this.frame = (this.lifeSpan = lifeSpan);
			base.background = true;
			base.owner = (int)base.trajectory.Y;
			base.renderState = RenderState.AdditiveOnly;
			this.renderRect = new Rectangle(-200, -200, Game1.screenWidth + 400, Game1.screenHeight + 400);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				base.Reset();
			}
			this.frame -= gameTime;
			base.location += base.trajectory * gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float num = MathHelper.Clamp(MathHelper.Clamp((float)Math.Cos((double)Game1.map.MapSegFrame * 100.0 + (double)(base.owner * 4)), 0f, 1f), 0.5f, 1f);
			sprite.Draw(particlesTex[1], base.GameLocation(l), new Rectangle(1760, 128, 32, 32), new Color(1f, 1f, 1f, this.a * num * (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan))), 0f, new Vector2(16f, 16f), this.size * num * worldScale, SpriteEffects.None, 1f);
		}
	}
}
