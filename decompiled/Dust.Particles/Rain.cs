using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Rain : Particle
	{
		private float r;

		private float gb;

		private float alpha;

		private float size;

		private float rotation;

		private Rectangle renderRect;

		public Rain(Vector2 loc, Vector2 traj, float size, int type)
		{
			this.Reset(loc, traj, size, type);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size, int type)
		{
			base.exists = Exists.Init;
			base.location = loc;
			base.flag = type;
			if (base.flag == 0)
			{
				this.alpha = Rand.GetRandomFloat(0.1f, 0.8f);
			}
			else
			{
				this.alpha = Rand.GetRandomFloat(0f, 0.6f);
				base.flag = 1472 + Rand.GetRandomInt(0, 4) * 128;
			}
			if (!base.CanPrecipitate(base.location, 8))
			{
				this.alpha = 0f;
			}
			this.renderRect = new Rectangle(-Game1.screenWidth / 2, -100, Game1.screenWidth * 2, Game1.screenHeight + 300);
			Game1.wManager.precipCount++;
			base.trajectory = traj * Game1.hiDefScaleOffset;
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f;
			this.size = _size * Game1.hiDefScaleOffset;
			this.r = Rand.GetRandomFloat(0f, 1f);
			this.gb = Rand.GetRandomFloat(0.5f, 1f);
			base.renderState = RenderState.AdditiveOnly;
			base.exists = Exists.Exists;
		}

		public override void Relocate(Vector2 loc)
		{
			if (!base.CanPrecipitate(loc, 8))
			{
				this.alpha = 0f;
			}
			base.Relocate(loc);
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y))
			{
				Game1.wManager.precipCount--;
				if (Game1.wManager.weatherType == WeatherType.RainLight || Game1.wManager.weatherType == WeatherType.RainHeavy)
				{
					float y = ((!(c[0].Trajectory.Y > base.trajectory.Y)) ? ((float)Rand.GetRandomInt(-100, 0)) : ((float)Rand.GetRandomInt(0, Game1.screenHeight + (int)(200f * Game1.hiDefScaleOffset))));
					if (Game1.wManager.weatherType == WeatherType.RainLight)
					{
						this.Reset(Game1.Scroll / Game1.worldScale * base.LayerScale(l) + new Vector2((float)(Game1.screenWidth / 2) + (float)Rand.GetRandomInt(-700, 1000) * Game1.hiDefScaleOffset, y) / Game1.worldScale, Rand.GetRandomVector2(-800f, -200f, 1200f, 2000f), Rand.GetRandomFloat(0.5f, 2f), 0);
					}
					else
					{
						this.Reset(Game1.Scroll / Game1.worldScale * base.LayerScale(l) + new Vector2((float)(Game1.screenWidth / 2) + (float)Rand.GetRandomInt(-600, 1200) * Game1.hiDefScaleOffset, y) / Game1.worldScale, Rand.GetRandomVector2(-1000f, 0f, 1200f, 1600f), 1.75f, 1);
					}
				}
				else
				{
					base.Reset();
				}
			}
			base.location += base.trajectory * gameTime / Game1.worldScale;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (base.flag == 0)
			{
				sprite.Draw(particlesTex[1], vector, new Rectangle(143, 128, 4, 32), new Color(this.r, this.gb, this.gb, this.alpha), this.rotation, new Vector2(2f, 16f), this.size, SpriteEffects.None, 1f);
				sprite.Draw(particlesTex[1], vector + new Vector2(200f, -100f), new Rectangle(143, 128, 4, 32), new Color(this.r, this.gb, this.gb, this.alpha), this.rotation, new Vector2(2f, 16f), this.size, SpriteEffects.None, 1f);
			}
			else
			{
				sprite.Draw(particlesTex[2], vector, new Rectangle(base.flag, 2596, 128, 128), new Color(this.r, this.gb, this.gb, this.alpha), this.rotation, new Vector2(64f, 64f), new Vector2(this.size, this.size * 1.5f), SpriteEffects.None, 1f);
			}
		}
	}
}
