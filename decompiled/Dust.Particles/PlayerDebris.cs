using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class PlayerDebris : Particle
	{
		private SpriteEffects flip;

		private Rectangle sRect;

		private Texture2D texture;

		private float size;

		private float rotation;

		private float lifeSpan;

		private float r;

		private float g;

		private float b;

		private Vector2 ploc = Vector2.Zero;

		public PlayerDebris(Vector2 loc, Vector2 traj, Texture2D texture, Rectangle sourceRect, float size, float lifeSpan, int dieType, byte _r, byte _g, byte _b)
		{
			this.Reset(loc, traj, texture, sourceRect, size, lifeSpan, dieType, _r, _g, _b);
		}

		public void Reset(Vector2 loc, Vector2 traj, Texture2D _texture, Rectangle sourceRect, float _size, float _lifeSpan, int dieType, byte _r, byte _g, byte _b)
		{
			base.exists = Exists.Init;
			this.texture = _texture;
			this.sRect = sourceRect;
			base.flag = dieType;
			this.lifeSpan = _lifeSpan;
			base.location = loc;
			base.trajectory = traj;
			this.rotation = Rand.GetRandomFloat(0f, 6.28f);
			this.size = _size;
			this.r = (float)(int)_r / 255f;
			this.g = (float)(int)_g / 255f;
			this.b = (float)(int)_b / 255f;
			this.flip = ((Rand.GetRandomInt(0, 2) != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!new Rectangle(-64, -400, Game1.screenWidth + 128, Game1.screenHeight + 528).Contains((int)vector.X, (int)vector.Y))
			{
				base.Reset();
			}
			base.trajectory.Y += Game1.FrameTime * 1600f;
			if (base.trajectory.X > 0f)
			{
				this.rotation += Game1.FrameTime * 4f;
			}
			else
			{
				this.rotation -= Game1.FrameTime * 4f;
			}
			bool flag = false;
			if (map.CheckCol(base.location) > 0)
			{
				if (base.flag > 10)
				{
					flag = true;
				}
				else
				{
					base.location.X = this.ploc.X;
					base.trajectory.X = (0f - base.trajectory.X) / 2f;
				}
			}
			if (Math.Abs(base.trajectory.X) > 2f)
			{
				base.trajectory.X /= 1.02f;
			}
			else
			{
				base.trajectory.X = 0f;
			}
			if (base.trajectory.Y < 1500f)
			{
				base.trajectory.Y += Game1.FrameTime * 2000f;
			}
			if (base.trajectory.Y < 0f)
			{
				if (map.CheckCol(new Vector2(base.location.X, base.location.Y - 32f)) > 0 || base.location.Y < map.topEdge)
				{
					base.trajectory.Y = 0f;
					base.location.Y = this.ploc.Y;
				}
			}
			else
			{
				float num = map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false);
				if (num > 0f)
				{
					base.location.Y = num;
					if (base.flag < 10)
					{
						base.trajectory.Y = Math.Min((0f - base.trajectory.Y) / 2f, -500f);
					}
					else
					{
						base.trajectory = Vector2.Zero;
						flag = true;
					}
				}
			}
			if (flag || this.lifeSpan < 0f)
			{
				int num2 = base.flag;
				if (num2 != 1)
				{
					base.Reset();
				}
				else
				{
					base.Reset();
					int num3 = this.sRect.Width / 2;
					pMan.AddDeathFlame(base.location + Rand.GetRandomVector2(-num3, num3, -num3, num3), Rand.GetRandomVector2(-40f, 40f, -150f, -20f), 0.3f, Rand.GetRandomFloat(0.2f, 0.75f), Rand.GetRandomFloat(0.5f, 0.8f), Rand.GetRandomFloat(0.6f, 1f), 0, audio: true, 5);
				}
			}
			this.ploc = base.location;
			base.location += base.trajectory * gameTime;
			this.lifeSpan -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 position = base.GameLocation(l);
			if (this.texture != null && !this.texture.IsDisposed)
			{
				sprite.Draw(this.texture, position, this.sRect, new Color(this.r, this.g, this.b, this.lifeSpan * 4f), this.rotation, new Vector2(this.sRect.Width, this.sRect.Height) / 2f, this.size * worldScale, this.flip, 1f);
			}
		}
	}
}
