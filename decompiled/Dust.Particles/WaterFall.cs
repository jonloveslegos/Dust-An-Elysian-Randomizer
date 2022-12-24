using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class WaterFall : Particle
	{
		private int stopPoint;

		private float frame;

		private float a;

		private float size;

		private int sRectX;

		public WaterFall(Vector2 loc, float traj, float size, int stopPoint)
		{
			this.Reset(loc, traj, size, stopPoint);
		}

		public void Reset(Vector2 loc, float traj, float size, int stopPoint)
		{
			base.exists = Exists.Init;
			base.background = true;
			base.location = loc;
			base.trajectory.Y = traj * size;
			this.size = size;
			this.stopPoint = stopPoint;
			this.frame = 1f;
			base.renderState = RenderState.Normal;
			this.sRectX = 3780 + Rand.GetRandomInt(0, 3) * 100;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (base.location.Y > (float)(this.stopPoint - 200))
			{
				this.frame -= gameTime * 10f;
			}
			this.frame -= gameTime;
			if (this.frame < 0f)
			{
				base.Reset();
			}
			this.size += gameTime;
			base.location.Y += base.trajectory.Y * gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(this.sRectX, 128, 100, 100), Color.White * Math.Min(this.frame, 0.4f), 0f, new Vector2(50f, 50f), new Vector2(this.size, this.size * 1.5f) * worldScale, SpriteEffects.None, 1f);
		}
	}
}
