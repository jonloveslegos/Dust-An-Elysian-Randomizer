using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class ChainLink : Particle
	{
		private byte animFrame;

		private float animFrameTime;

		private float frame;

		private float rotation;

		private Vector2 ploc;

		public ChainLink(Vector2 loc, Vector2 traj)
		{
			this.Reset(loc, traj);
		}

		public void Reset(Vector2 loc, Vector2 traj)
		{
			base.exists = Exists.Init;
			base.location = (this.ploc = loc);
			base.trajectory = traj;
			this.frame = Rand.GetRandomFloat(1f, 3f);
			this.animFrame = (byte)Rand.GetRandomInt(0, 4);
			this.animFrameTime = 0f;
			this.rotation = Rand.GetRandomFloat(0f, 3.14f);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.GameLocation(l);
			if (this.frame <= 0f)
			{
				base.Reset();
			}
			if (map.CheckCol(base.location) > 0)
			{
				base.location.X = this.ploc.X;
				base.trajectory.X = 0f - base.trajectory.X;
			}
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
				base.trajectory.Y += Game1.FrameTime * 2000f;
			}
			if (base.trajectory.Y < 0f && (map.CheckCol(new Vector2(base.location.X, base.location.Y - 32f)) > 0 || base.location.Y < map.topEdge))
			{
				base.trajectory.Y = 0f;
				base.location.Y = this.ploc.Y;
			}
			if (base.trajectory.Y > -200f && map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) > 0f && l == 5)
			{
				if (base.trajectory.Y > 200f)
				{
					base.trajectory.Y = (0f - base.trajectory.Y) / 1.5f;
				}
				else
				{
					base.trajectory.Y = 0f;
				}
			}
			this.animFrameTime += gameTime * 20f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 4)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			this.ploc = base.location;
			base.location += base.trajectory * gameTime;
			this.frame -= gameTime * 2f;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float num = Math.Min(this.frame, 1f);
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(840 + this.animFrame * 48, 2130, 48, 48), new Color(1f, 1f, 1f, num), this.rotation, new Vector2(24f, 24f), num * 0.6f * worldScale, SpriteEffects.None, 1f);
		}
	}
}
