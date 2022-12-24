using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class HeatWave : Particle
	{
		private float lifeSpan;

		private float frame;

		private float size;

		private float rotation;

		private int srectX;

		public HeatWave(Vector2 loc, Vector2 traj, float _size, float _lifeSpan)
		{
			this.Reset(loc, traj, _size, _lifeSpan);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size, float _lifeSpan)
		{
			base.exists = Exists.Init;
			base.background = true;
			base.location = loc;
			base.trajectory = traj;
			this.size = _size * Game1.hiDefScaleOffset;
			this.frame = (this.lifeSpan = _lifeSpan);
			base.renderState = RenderState.RefractOnly;
			base.flag = Rand.GetRandomInt(0, 4);
			this.srectX = 910 + base.flag * 120 - base.flag / 2 * 240;
			base.flag = 1000 + base.flag / 2 * 120;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			base.trajectory.Y *= 0.994f;
			base.trajectory.X *= 0.99f;
			base.location += base.trajectory * gameTime;
			if (base.trajectory.X < 0f)
			{
				this.rotation += base.trajectory.Y * Game1.FrameTime / 600f;
			}
			else
			{
				this.rotation -= base.trajectory.Y * Game1.FrameTime / 600f;
			}
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(this.srectX, base.flag, 120, 120), new Color(1f, 1f, 1f, 0.5f * (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan))), this.rotation, new Vector2(60f, 60f), new Vector2(this.size + (1f - this.frame / this.lifeSpan), this.size + this.frame * this.size * 0.1f + (1f - this.frame / this.lifeSpan)), SpriteEffects.None, 1f);
		}
	}
}
