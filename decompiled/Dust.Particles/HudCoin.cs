using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class HudCoin : Particle
	{
		private byte animFrame;

		private float animFrameTime;

		public HudCoin(Vector2 loc, Vector2 traj)
		{
			this.Reset(loc, traj);
		}

		public void Reset(Vector2 loc, Vector2 traj)
		{
			base.exists = Exists.Init;
			base.location = loc;
			base.trajectory = traj;
			if (base.flag == 1)
			{
				this.animFrame = (byte)Rand.GetRandomInt(0, 11);
				this.animFrameTime = 0f;
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			_ = base.location;
			if (Math.Abs(base.trajectory.X) > 1f)
			{
				base.trajectory.X /= 1.04f;
			}
			else
			{
				base.trajectory.X = 0f;
			}
			if (base.trajectory.Y < 1500f)
			{
				base.trajectory.Y += Game1.HudTime * 2000f;
			}
			if (base.location.Y > (float)(Game1.screenHeight + 20))
			{
				base.Reset();
			}
			this.animFrameTime += Game1.HudTime * 30f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 11)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			base.location += base.trajectory * Game1.HudTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[1], base.location, new Rectangle(192 + this.animFrame * 32, 128, 32, 32), Color.White, 0f, new Vector2(16f, 32f), 1f, SpriteEffects.None, 1f);
		}
	}
}
