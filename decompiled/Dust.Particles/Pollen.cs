using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Pollen : Particle
	{
		private float lifeSpan;

		private int layerOffset;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float a;

		private float size;

		private float rotation;

		private Rectangle renderRect = new Rectangle(-Game1.screenWidth / 2, -100, Game1.screenWidth * 2, Game1.screenHeight + 200);

		public Pollen(int l)
		{
			this.Reset(l);
			Game1.wManager.pollenCount++;
		}

		public void Reset(int l)
		{
			base.exists = Exists.Init;
			this.lifeSpan = 5f * Rand.GetRandomFloat(0.75f, 1.25f);
			this.rotation = Rand.GetRandomFloat(0f, 6.28f);
			this.size = Rand.GetRandomFloat(0.25f, 1f) * Game1.hiDefScaleOffset;
			this.frame = this.lifeSpan;
			this.r = Rand.GetRandomFloat(0f, 1f);
			this.g = Rand.GetRandomFloat(0.5f, 1f);
			this.b = Rand.GetRandomFloat(0.5f, 1f);
			this.a = Rand.GetRandomFloat(0.1f, 0.6f);
			base.renderState = RenderState.AdditiveOnly;
			this.layerOffset = 0;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.layerOffset = Rand.GetRandomInt(2, 4);
			}
			else
			{
				this.layerOffset = Rand.GetRandomInt(-2, 1);
			}
			if (Rand.GetRandomInt(0, 20) == 0)
			{
				base.flag = 100;
			}
			this.ResetPos(l);
			base.exists = Exists.Exists;
		}

		private void ResetPos(int l)
		{
			this.renderRect = new Rectangle(-Game1.screenWidth / 2, -100, Game1.screenWidth * 2, Game1.screenHeight + 200);
			int num = this.renderRect.X;
			if (Game1.Scroll.X < Game1.pScroll.X)
			{
				num *= 2;
			}
			else if (Game1.Scroll.X > Game1.pScroll.X)
			{
				num = 0;
			}
			base.location = Game1.Scroll;
			base.location += new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - 1f / base.LayerScale(l + this.layerOffset)) * (Game1.hiDefScaleOffset - Game1.worldScale);
			base.location /= Game1.worldScale;
			base.location *= base.LayerScale(l + this.layerOffset);
			base.location += Rand.GetRandomVector2(num, num + this.renderRect.Width, this.renderRect.Y, this.renderRect.Height) / Game1.worldScale;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (base.flag == 100)
			{
				this.a = Rand.GetRandomFloat(0.1f, 0.6f);
			}
			Vector2 vector = base.GameLocation(l + this.layerOffset);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				if (Game1.wManager.weatherType == WeatherType.Pollen)
				{
					this.frame = this.lifeSpan;
					base.flag = 0;
					if (Rand.GetRandomInt(0, 20) == 0)
					{
						base.flag = 100;
					}
					this.ResetPos(l);
				}
				else
				{
					base.Reset();
					Game1.wManager.pollenCount--;
				}
			}
			this.rotation += Game1.FrameTime / 1.5f;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float num = (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan));
			sprite.Draw(particlesTex[1], base.GameLocation(l + this.layerOffset), new Rectangle(32, 128, 32, 32), new Color(this.r, this.g, this.b, this.a * num), this.rotation, new Vector2(-132f, 32f), this.size, SpriteEffects.None, 1f);
			sprite.Draw(particlesTex[1], base.GameLocation(l + this.layerOffset), new Rectangle(32, 128, 32, 32), new Color(this.r, this.g, this.b, this.a * num), 0f - this.rotation, new Vector2(132f, 32f), this.size / 2f, SpriteEffects.None, 1f);
		}
	}
}
