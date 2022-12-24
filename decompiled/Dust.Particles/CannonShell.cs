using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class CannonShell : Particle
	{
		private byte animFrame;

		private float animFrameTime;

		private float frame;

		private float size;

		private Vector2 ploc;

		public CannonShell(Vector2 loc, Vector2 traj, float _size)
		{
			this.Reset(loc, traj, _size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size)
		{
			base.exists = Exists.Init;
			this.size = _size;
			base.location = (this.ploc = loc);
			base.trajectory = traj;
			this.frame = 1.5f;
			base.background = true;
			this.animFrame = (byte)Rand.GetRandomInt(0, 11);
			this.animFrameTime = 0f;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.location * Game1.worldScale - Game1.Scroll;
			if (!new Rectangle(-600, -400, Game1.screenWidth + 1200, Game1.screenHeight + 500).Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				base.Reset();
			}
			if (Math.Abs(base.trajectory.X) > 1f)
			{
				base.trajectory.X /= 1.02f;
			}
			else
			{
				base.trajectory.X = 0f;
			}
			if (base.trajectory.Y < 1500f)
			{
				base.trajectory.Y += gameTime * 2000f;
			}
			if (base.trajectory.Y < 0f && (map.CheckCol(new Vector2(base.location.X, base.location.Y - 32f)) > 0 || base.location.Y < map.topEdge))
			{
				base.trajectory.Y = 0f;
				base.location.Y = this.ploc.Y;
			}
			if (base.trajectory.Y > -200f)
			{
				float num = map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false);
				if (num > 0f)
				{
					base.location.Y = num;
					Sound.PlayDropCue("coin_bounce", base.location, base.trajectory.Y);
					base.trajectory.Y = (0f - base.trajectory.Y) / 1.5f;
				}
			}
			this.animFrameTime += gameTime * 36f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 19)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			this.ploc = base.location;
			base.location += base.trajectory * gameTime;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			SpriteEffects effects = ((!(base.trajectory.X > 0f)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			sprite.Draw(particlesTex[2], (base.location - new Vector2(0f, 60f)) * Game1.worldScale - Game1.Scroll, new Rectangle(1303 + 64 * this.animFrame, 3266, 64, 64), new Color(0.5f, 0.5f, 0.5f, this.frame * 4f), 0f, new Vector2(32f, 32f), this.size * worldScale, effects, 0f);
		}
	}
}
