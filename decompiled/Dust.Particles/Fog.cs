using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Fog : Particle
	{
		private int layerOffset;

		private int srectX;

		private float lifeSpan;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float size;

		private Rectangle renderRect;

		public Fog(int layer)
		{
			this.Reset(layer);
		}

		public void Reset(int layer)
		{
			base.exists = Exists.Init;
			Game1.wManager.fogCount++;
			this.frame = (this.lifeSpan = 7f * Rand.GetRandomFloat(0.75f, 1.25f));
			base.trajectory = Rand.GetRandomVector2(-50f, 50f, -5f, 5f);
			this.size = Rand.GetRandomFloat(3f, 4f) / Game1.worldScale;
			base.flag = Rand.GetRandomInt(0, 4);
			this.srectX = 910 + base.flag * 120 - base.flag / 2 * 240;
			base.flag = 1000 + base.flag / 2 * 120;
			this.r = Rand.GetRandomFloat(0.5f, 1f);
			this.g = Rand.GetRandomFloat(0.75f, 1f);
			this.b = Rand.GetRandomFloat(0.75f, 1f);
			base.renderState = RenderState.AdditiveOnly;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.layerOffset = Rand.GetRandomInt(2, 4);
			}
			else
			{
				this.layerOffset = Rand.GetRandomInt(-2, 1);
			}
			this.ResetPos(layer);
			base.exists = Exists.Exists;
		}

		private void ResetPos(int l)
		{
			this.renderRect = new Rectangle((int)((0f - this.size) * 480f), (int)((0f - this.size) * 120f), Game1.screenWidth + (int)(this.size * 960f), Game1.screenHeight + (int)(this.size * 240f));
			int num = 0;
			if ((int)Game1.Scroll.X < (int)Game1.pScroll.X)
			{
				num = this.renderRect.X / 2;
			}
			else if ((int)Game1.Scroll.X > (int)Game1.pScroll.X)
			{
				num = Game1.screenWidth / 2;
			}
			base.location = Game1.Scroll;
			base.location += new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - 1f / base.LayerScale(l + this.layerOffset)) * (Game1.hiDefScaleOffset - Game1.worldScale);
			base.location /= Game1.worldScale;
			base.location *= base.LayerScale(l + this.layerOffset);
			base.location += Rand.GetRandomVector2(num, num + Game1.screenWidth, -100f, Game1.screenHeight + 200) / Game1.worldScale;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l + this.layerOffset);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				Game1.wManager.fogCount--;
				if (Game1.wManager.weatherType == WeatherType.Fog || Game1.wManager.weatherType == WeatherType.Mansion)
				{
					this.Reset(l);
				}
				else
				{
					base.Reset();
				}
			}
			base.location += base.trajectory * gameTime;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float w = (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan));
			sprite.Draw(particlesTex[2], base.GameLocation(l + this.layerOffset), new Rectangle(this.srectX, base.flag, 120, 120), new Color(new Vector4(this.r, this.g, this.b, w)), 0f, new Vector2(60f, 60f), new Vector2(this.size * 4f, this.size) * worldScale, SpriteEffects.None, 1f);
		}
	}
}
