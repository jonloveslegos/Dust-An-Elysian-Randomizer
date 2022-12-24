using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Feather : Particle
	{
		private byte animFrame;

		private float animFrameTime;

		private float lifeSpan;

		private float frame;

		private float size;

		private float rotation;

		private float swingX;

		public Feather(Vector2 loc, Vector2 traj, float size, float _lifeSpan)
		{
			this.Reset(loc, traj, size, _lifeSpan);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size, float _lifeSpan)
		{
			base.exists = Exists.Init;
			this.lifeSpan = _lifeSpan;
			this.animFrame = (byte)Rand.GetRandomInt(0, 45);
			this.animFrameTime = 0f;
			base.location = loc;
			base.trajectory = traj;
			this.size = _size;
			this.frame = this.lifeSpan;
			base.renderState = RenderState.AdditiveOnly;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			this.animFrameTime += gameTime * 40f * this.frame;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 47)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			this.swingX = (float)Math.Cos((double)map.MapSegFrame + (double)this.frame) * 50f;
			if (base.trajectory.Y < 200f)
			{
				base.trajectory.Y += gameTime * 100f;
			}
			if (Math.Abs(base.trajectory.X) > 1f)
			{
				base.trajectory.X /= 1.01f;
			}
			else
			{
				base.trajectory.X = 0f;
			}
			base.location += base.trajectory * gameTime;
			this.rotation = 2f * GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) - 135f;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan));
			sprite.Draw(particlesTex[2], base.GameLocation(l, base.location + new Vector2(this.swingX, 0f)), new Rectangle(1740 + this.animFrame * 32, 2042, 32, 32), new Color(1f, 1f, 1f, this.frame), this.rotation, new Vector2(16f, 16f), new Vector2(2f, 1f) * this.size * worldScale, SpriteEffects.None, 1f);
		}
	}
}
