using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class SnowTime : Particle
	{
		private int origX;

		private float alpha;

		private float size;

		private float rotation;

		private Rectangle renderRect = new Rectangle(-300, -300, Game1.screenWidth + 600, Game1.screenHeight + 600);

		private Vector2[] flakes = new Vector2[5];

		private byte[] flakeType = new byte[5];

		private bool render;

		public SnowTime(Vector2 traj, float size, int layer)
		{
			this.Reset(traj, size, layer);
		}

		public void Reset(Vector2 traj, float _size, int layer)
		{
			base.exists = Exists.Init;
			base.trajectory = traj;
			this.alpha = 0f;
			this.ResetPos(layer);
			this.flakeType[0] = (byte)Rand.GetRandomInt(0, 5);
			this.origX = (int)(Rand.GetRandomFloat(-1f, 1f) * base.trajectory.Y);
			this.rotation = Rand.GetRandomFloat(0f, 6.28f);
			if (base.CanPrecipitate(base.location, layer))
			{
				this.render = true;
			}
			else
			{
				this.render = false;
			}
			if (layer == 8)
			{
				Game1.wManager.precipCount++;
			}
			this.size = _size;
			base.renderState = RenderState.AdditiveOnly;
			base.exists = Exists.Exists;
		}

		private void ResetPos(int l)
		{
			this.renderRect = new Rectangle(-300, -300, Game1.screenWidth + 600, Game1.screenHeight + 600);
			int num = this.renderRect.X;
			if (Game1.Scroll.X < Game1.pScroll.X - 10f)
			{
				num *= 2;
			}
			else if (Game1.Scroll.X > Game1.pScroll.X + 10f)
			{
				num = 0;
			}
			base.location = Game1.Scroll;
			base.location += new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - 1f / base.LayerScale(l)) * (1f - Game1.worldScale);
			base.location /= Game1.worldScale;
			base.location *= base.LayerScale(l);
			int num2 = this.renderRect.Height;
			switch (Game1.wManager.weatherType)
			{
			case WeatherType.SnowHeavy:
				num2 /= 4;
				break;
			case WeatherType.SnowLight:
				num2 /= 2;
				break;
			}
			base.location += Rand.GetRandomVector2(num, num + this.renderRect.Width, this.renderRect.Y, num2) / Game1.worldScale;
			if (Game1.character[0].Trajectory.Y > base.trajectory.Y)
			{
				base.location.Y += 720f / Game1.worldScale;
			}
		}

		public override void Relocate(Vector2 loc)
		{
			if (!base.CanPrecipitate(loc, 8))
			{
				this.alpha = -1f;
			}
			base.Relocate(loc);
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			float num = (c[0].WallInWay ? 0f : (c[0].Trajectory.X / -100000f));
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || Game1.wManager.weatherType != WeatherType.SnowTime)
			{
				this.alpha = -1f;
			}
			else if (this.alpha >= 0f && c[0].Trajectory.Length() > 4f)
			{
				this.alpha = Math.Min(this.alpha + gameTime * 4f, 1f);
			}
			if (this.alpha < 0f)
			{
				if (l == 8)
				{
					Game1.wManager.precipCount--;
				}
				WeatherType weatherType = Game1.wManager.weatherType;
				if (weatherType == WeatherType.SnowTime)
				{
					this.Reset(Rand.GetRandomVector2(-200f, 0f, 50f, 600f), Rand.GetRandomFloat(1f, 2f), l);
				}
				else
				{
					base.Reset();
				}
			}
			base.trajectory.X = MathHelper.Clamp(base.trajectory.X + num * Game1.wManager.windStrength, -400f, 400f);
			if (this.origX < 0)
			{
				this.rotation -= num * 2f;
			}
			else
			{
				this.rotation += num * 2f;
			}
			base.location += base.trajectory * num / Game1.worldScale;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.render)
			{
				Vector2 vector = base.GameLocation(l);
				Color color = new Color(1f, 1f, 1f, this.alpha);
				Rectangle value = new Rectangle(4080, 128 + this.flakeType[0] * 16, 16, 16);
				sprite.Draw(particlesTex[2], vector, value, color, this.rotation, new Vector2(this.origX, 8f), this.size * worldScale, SpriteEffects.None, 1f);
				sprite.Draw(particlesTex[2], vector + new Vector2(200f, -100f) * worldScale, value, color, 0f - this.rotation, new Vector2(this.origX, 8f), this.size * worldScale / 1.5f, SpriteEffects.None, 1f);
				sprite.Draw(particlesTex[2], vector + new Vector2(150f, 100f) * worldScale, value, color, this.rotation, new Vector2(this.origX, 8f), this.size * worldScale / 2f, SpriteEffects.None, 1f);
			}
		}
	}
}
