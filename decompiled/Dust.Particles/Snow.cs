using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Snow : Particle
	{
		private int origX;

		private float alpha;

		private float size;

		private float rotation;

		private Rectangle renderRect;

		private Vector2[] flakes = new Vector2[5];

		private byte[] flakeType = new byte[5];

		private bool render;

		public Snow(Vector2 traj, float size, int type, int layer)
		{
			this.Reset(traj, size, type, layer);
		}

		public void Reset(Vector2 traj, float _size, int type, int layer)
		{
			base.exists = Exists.Init;
			base.trajectory = traj;
			base.flag = type;
			this.alpha = 0f;
			this.ResetPos(layer);
			if (base.flag == 0)
			{
				this.flakeType[0] = (byte)Rand.GetRandomInt(0, 5);
				this.origX = (int)(Rand.GetRandomFloat(-1f, 1f) * base.trajectory.Y);
			}
			else
			{
				if (base.flag == 1)
				{
					base.trajectory.X = MathHelper.Clamp(base.trajectory.X + Game1.wManager.windStrength, -4000f, 4000f);
				}
				for (int i = 0; i < this.flakes.Length; i++)
				{
					ref Vector2 reference = ref this.flakes[i];
					reference = Rand.GetRandomVector2(-200f, 200f, -200f, 200f);
					this.flakeType[i] = (byte)Rand.GetRandomInt(0, 3);
				}
				this.origX = Rand.GetRandomInt(-100, 100);
			}
			base.trajectory *= Game1.hiDefScaleOffset;
			this.rotation = Rand.GetRandomFloat(0f, 6.28f);
			if (base.CanPrecipitate(base.location, layer))
			{
				this.render = true;
			}
			else
			{
				this.render = false;
			}
			Game1.wManager.precipCount++;
			this.size = _size * Game1.hiDefScaleOffset;
			base.renderState = RenderState.AdditiveOnly;
			base.exists = Exists.Exists;
		}

		private void ResetPos(int l)
		{
			this.renderRect = new Rectangle((int)(-400f * Game1.hiDefScaleOffset), (int)(-400f * Game1.hiDefScaleOffset), Game1.screenWidth + (int)(800f * Game1.hiDefScaleOffset), Game1.screenHeight + (int)(400f * Game1.hiDefScaleOffset));
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
			base.location += Rand.GetRandomVector2(num + ((base.flag >= 2) ? (this.renderRect.Width / 2) : 0), num + this.renderRect.Width, this.renderRect.Y, num2) / Game1.worldScale;
			if (Game1.character[0].Trajectory.Y > base.trajectory.Y)
			{
				base.location.Y += (float)Game1.screenHeight / Game1.worldScale;
				if (Game1.wManager.weatherType == WeatherType.SnowLight)
				{
					this.alpha = 1f;
				}
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
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y))
			{
				if (base.flag == 0)
				{
					this.alpha = -1f;
				}
				else
				{
					this.alpha -= gameTime * 2f;
				}
			}
			else if (this.alpha >= 0f)
			{
				this.alpha = Math.Min(this.alpha + gameTime * 4f, (base.flag == 0) ? 1f : 0.4f);
			}
			if (this.alpha < 0f)
			{
				Game1.wManager.precipCount--;
				switch (Game1.wManager.weatherType)
				{
				case WeatherType.SnowLight:
					this.Reset(Rand.GetRandomVector2(-200f, 0f, 50f, 300f), Rand.GetRandomFloat(0.4f, 0.8f), 0, l);
					break;
				case WeatherType.SnowHeavy:
					if (Rand.GetRandomInt(0, 4) == 0)
					{
						this.Reset(Rand.GetRandomVector2(-1000f, 0f, 800f, 1000f), 1.5f, 0, l);
					}
					else
					{
						this.Reset(_size: (base.flag != 0) ? Rand.GetRandomFloat(1f, 1.75f) : Rand.GetRandomFloat(0.5f, 1f), traj: Rand.GetRandomVector2(-600f, 0f, 400f, 600f), type: 1, layer: l);
					}
					break;
				case WeatherType.SnowFierce:
					this.Reset(Rand.GetRandomVector2(-2000f, -1000f, 0f, 500f), Rand.GetRandomFloat(1.5f, 2f), 2, l);
					break;
				default:
					base.Reset();
					break;
				}
			}
			if (base.flag == 0)
			{
				base.trajectory.X = MathHelper.Clamp(base.trajectory.X + gameTime * Game1.wManager.windStrength, -400f, 400f);
				if (this.origX < 0)
				{
					this.rotation -= gameTime * 2f;
				}
				else
				{
					this.rotation += gameTime * 2f;
				}
			}
			else
			{
				if (this.origX < 0)
				{
					this.rotation -= gameTime * 1f;
				}
				else
				{
					this.rotation += gameTime * 1f;
				}
				if (Game1.wManager.weatherType == WeatherType.SnowLight)
				{
					this.size = Math.Max(this.size - gameTime, 0.2f);
					this.alpha -= gameTime * 4f;
				}
			}
			base.location += base.trajectory * gameTime / Game1.worldScale;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (!this.render)
			{
				return;
			}
			Vector2 vector = base.GameLocation(l);
			Color color = new Color(1f, 1f, 1f, this.alpha);
			if (base.flag == 0)
			{
				Vector2 vector2 = new Vector2(1f, Math.Abs((float)Math.Cos(Game1.map.MapSegFrameLocked * 20f + (float)(int)this.flakeType[0]))) * this.size;
				Rectangle value = new Rectangle(4080, 128 + this.flakeType[0] * 16, 16, 16);
				sprite.Draw(particlesTex[2], vector, value, color, this.rotation, new Vector2(this.origX, 8f), vector2, SpriteEffects.None, 1f);
				sprite.Draw(particlesTex[2], vector + new Vector2(200f, -100f), value, color, 0f - this.rotation, new Vector2(this.origX, 8f), vector2 / 1.5f, SpriteEffects.None, 1f);
				sprite.Draw(particlesTex[2], vector + new Vector2(150f, 100f), value, color, this.rotation, new Vector2(this.origX, 8f), vector2 / 2f, SpriteEffects.None, 1f);
				return;
			}
			Vector2 vector3 = (Game1.character[0].Location - new Vector2(0f, 100f)) * worldScale - Game1.Scroll;
			float num = Math.Min(((vector - vector3) / vector3).Length(), 1f);
			for (int i = 0; i < this.flakes.Length; i++)
			{
				_ = new Vector2(1f, Math.Max(Math.Abs((float)Math.Cos(Game1.map.MapSegFrameLocked * 10f + this.flakes[0].X)), 0.2f)) * this.size;
				float num2 = this.flakes[i].X * (float)Math.Cos(Game1.map.MapSegFrameLocked * 20f + (float)i);
				sprite.Draw(position: new Vector2((float)(Math.Cos(this.rotation) * (double)num2 - Math.Sin(this.rotation) * (double)this.flakes[i].Y), (float)(Math.Cos(this.rotation) * (double)this.flakes[i].Y + Math.Sin(this.rotation) * (double)num2)) + vector, texture: particlesTex[2], sourceRectangle: new Rectangle(2212, 1000 + this.flakeType[i] * 80, 80, 80), color: new Color(1f, 1f, 1f, this.alpha * num), rotation: 0f - this.rotation + (float)i, origin: new Vector2(this.origX, 0f), scale: this.size + (float)i / 20f, effects: SpriteEffects.None, layerDepth: 1f);
			}
		}
	}
}
