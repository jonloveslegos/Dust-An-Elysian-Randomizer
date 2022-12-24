using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class PollenCave : Particle
	{
		private int layerOffset;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float size;

		private float rotation;

		private Rectangle renderRect;

		public PollenCave(int l)
		{
			this.Reset(l);
		}

		public void Reset(int l)
		{
			base.exists = Exists.Init;
			Game1.wManager.cavePollenCount++;
			this.frame = 0f;
			this.size = Rand.GetRandomFloat(0.1f, 0.5f) * Game1.hiDefScaleOffset;
			base.flag = Rand.GetRandomInt(4, 20);
			this.r = Rand.GetRandomFloat(0f, 1f);
			this.g = Rand.GetRandomFloat(0f, 1f);
			this.b = Rand.GetRandomFloat(0f, 1f);
			this.rotation = Rand.GetRandomFloat(0f, 6.28f);
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
			base.trajectory = base.location;
			this.frame = 0f;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l + this.layerOffset);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y))
			{
				Game1.wManager.cavePollenCount--;
				if (Game1.wManager.weatherType == WeatherType.CaveCalm || Game1.wManager.weatherType == WeatherType.CaveRumble)
				{
					this.ResetPos(l);
				}
				else
				{
					base.Reset();
				}
			}
			this.rotation += Game1.FrameTime / 4f;
			this.frame += gameTime;
			if (this.frame > (float)base.flag)
			{
				this.frame = 1f;
				base.flag = Rand.GetRandomInt(4, 20);
				base.trajectory = base.location + Rand.GetRandomVector2(-400f, 400f, -400f, 400f) * Game1.hiDefScaleOffset;
				if (l + this.layerOffset == 6)
				{
					pMan.AddSparkle(base.location, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.75f, this.size, 12, 6);
				}
			}
			base.trajectory.Y += gameTime * 10f * base.LayerScale(l + this.layerOffset) * Game1.hiDefScaleOffset;
			if (map.transInFrame > 0f)
			{
				base.location = base.trajectory;
			}
			else
			{
				base.location += (base.trajectory - base.location) * gameTime;
			}
			if (Game1.skipFrame > 1 && c[0].AnimName.StartsWith("attack"))
			{
				float num = base.LayerScale(l + this.layerOffset);
				Vector2 scroll = Game1.Scroll;
				scroll += new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - 1f / num) * (Game1.hiDefScaleOffset - Game1.worldScale) / Game1.hiDefScaleOffset;
				scroll /= Game1.worldScale;
				scroll *= num;
				scroll += ((c[0].Location + new Vector2((c[0].Face == CharDir.Left) ? (-180) : 180, -260f)) * Game1.worldScale - Game1.Scroll) / Game1.worldScale;
				if (c[0].AnimName == "attack01")
				{
					base.trajectory += (scroll - base.trajectory) * gameTime;
				}
				else
				{
					base.trajectory -= (scroll - base.trajectory) * gameTime * 0.4f;
				}
			}
			if (Game1.wManager.weatherType == WeatherType.CaveRumble)
			{
				base.trajectory.X += Game1.wManager.windStrength;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[1], base.GameLocation(l + this.layerOffset), new Rectangle(32, 128, 32, 32), new Color(this.r, this.g, this.b, this.frame), this.rotation, new Vector2(100f, 16f), this.size * worldScale, SpriteEffects.None, 1f);
		}
	}
}
