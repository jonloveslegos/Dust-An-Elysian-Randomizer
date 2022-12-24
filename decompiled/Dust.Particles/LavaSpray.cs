using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class LavaSpray : Particle
	{
		private Rectangle sRect;

		private float rotation;

		private float frame;

		private float size;

		private float maxSize;

		public LavaSpray(Vector2 loc, Vector2 _traj, float size, int type)
		{
			this.Reset(loc, _traj, size, type);
		}

		public void Reset(Vector2 _loc, Vector2 _traj, float _size, int type)
		{
			base.exists = Exists.Init;
			base.location = _loc;
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, _traj) + Rand.GetRandomFloat(-0.4f, 0.4f);
			base.trajectory = new Vector2((float)Math.Cos(this.rotation), (float)Math.Sin(this.rotation)) * (0f - _traj.Length()) * Rand.GetRandomFloat(0.7f, 1.5f);
			this.maxSize = Rand.GetRandomFloat(0.9f, 1.1f) * _size;
			this.size = 0f;
			if (base.trajectory.Y < 0f)
			{
				this.frame = Rand.GetRandomFloat(0.5f, 0.8f);
			}
			else
			{
				this.frame = Rand.GetRandomFloat(1f, 2f);
			}
			base.renderState = RenderState.AdditiveOnly;
			this.sRect = new Rectangle(964 + 120 * Rand.GetRandomInt(0, 3), 3840, 120, 120);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.trajectory.Y += gameTime * 400f;
			base.location += base.trajectory * gameTime;
			if (base.trajectory.X > 0f)
			{
				this.rotation += gameTime;
			}
			else
			{
				this.rotation -= gameTime;
			}
			this.frame -= gameTime;
			this.size = Math.Min(this.maxSize, this.size + gameTime * 4f);
			if (this.frame < 0f)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(1f, 0.8f, 0f, this.frame * this.size), this.rotation, new Vector2(this.sRect.Width, this.sRect.Height) / 2f, this.size * worldScale, SpriteEffects.None, 1f);
		}
	}
}
