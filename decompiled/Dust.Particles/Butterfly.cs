using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Butterfly : Particle
	{
		private float frameAlpha;

		private float grow;

		private byte animFrame;

		private byte moveSpeed;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float size;

		private float rotation;

		private Rectangle renderRect;

		public Butterfly(Vector2 loc, Vector2 traj, float size, int flag, int owner)
		{
			this.Reset(loc, traj, size, flag, owner);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size, int flag, int owner)
		{
			base.exists = Exists.Init;
			this.frameAlpha = 0f;
			this.grow = 0f;
			this.animFrame = 0;
			this.moveSpeed = (byte)Rand.GetRandomInt(6, 12);
			base.background = true;
			base.location = loc;
			base.trajectory = traj;
			this.rotation = 0f;
			this.size = size;
			base.owner = owner;
			base.flag = flag;
			if (flag == 1)
			{
				this.r = Rand.GetRandomFloat(0f, 1f);
				this.g = Rand.GetRandomFloat(0f, 1f);
				this.b = Rand.GetRandomFloat(0f, 1f);
			}
			else
			{
				this.r = (this.g = (this.b = 1f));
			}
			if (flag == 3)
			{
				this.size = size / 3f;
			}
			this.renderRect = new Rectangle(0, -100, Game1.screenWidth + (int)(4000f * Game1.hiDefScaleOffset), Game1.screenHeight + 200);
			Game1.map.bugCount++;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y))
			{
				base.Reset();
				map.bugCount--;
			}
			if (Game1.Scroll.X < Game1.pScroll.X)
			{
				this.renderRect.X = -this.renderRect.Width + Game1.screenWidth + 100;
			}
			else if (Game1.Scroll.X > Game1.pScroll.X)
			{
				this.renderRect.X = -100;
			}
			float num = (float)Math.Cos((double)map.MapSegFrame * 5.0 + (double)base.owner);
			float num2 = (float)Math.Sin((double)map.MapSegFrame * 30.0 + (double)base.owner);
			if (this.frameAlpha < 1f)
			{
				this.frameAlpha += Game1.FrameTime / 2f;
				this.grow += Game1.FrameTime / 2f;
				if (this.frameAlpha > 1f)
				{
					this.frameAlpha = 1f;
					this.grow = 1f;
				}
			}
			if (Game1.wManager.weatherType == WeatherType.RainHeavy)
			{
				base.location.Y += 2f;
			}
			if (base.trajectory.X > 0f)
			{
				this.rotation += gameTime / 2f;
				if (this.rotation > 0.4f)
				{
					this.rotation = 0.4f;
				}
			}
			else
			{
				this.rotation -= gameTime / 2f;
				if (this.rotation < -0.4f)
				{
					this.rotation = -0.4f;
				}
			}
			if (base.flag < 2)
			{
				base.trajectory = new Vector2(num * 1000f, num2 * 300f);
				this.frame += gameTime * 18f;
				if (this.frame > 1f)
				{
					this.animFrame++;
					if (this.animFrame > 4)
					{
						this.animFrame = 0;
					}
					this.frame = 0f;
				}
			}
			else
			{
				base.trajectory = new Vector2(num * 1000f, num2 * 600f);
				this.frame += gameTime * 30f;
				this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 3f;
				this.size = 0.2f;
				if (this.frame > 1f)
				{
					this.animFrame++;
					if (this.animFrame > 1)
					{
						this.animFrame = 0;
					}
					this.frame = 0f;
				}
			}
			base.location += base.trajectory * gameTime / (int)this.moveSpeed;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (base.flag < 2)
			{
				if (base.trajectory.X > 0f)
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(1472 + base.flag * 320 + this.animFrame * 64, 2724, 64, 64), new Color(this.r, this.g, this.b, this.frameAlpha), this.rotation, new Vector2(32f, 32f), this.size * this.grow * worldScale, SpriteEffects.None, 1f);
				}
				else
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(1472 + base.flag * 320 + this.animFrame * 64, 2724, 64, 64), new Color(this.r, this.g, this.b, this.frameAlpha), this.rotation, new Vector2(32f, 32f), this.size * this.grow * worldScale, SpriteEffects.FlipHorizontally, 1f);
				}
			}
			else if (base.trajectory.X < 0f)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(1472 + base.flag * 320 + this.animFrame * 64, 2724, 64, 64), new Color(this.r, this.g, this.b, this.frameAlpha), this.rotation, new Vector2(32f, 32f), this.size * this.grow * worldScale, SpriteEffects.FlipVertically, 1f);
			}
			else
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(1472 + base.flag * 320 + this.animFrame * 64, 2724, 64, 64), new Color(this.r, this.g, this.b, this.frameAlpha), this.rotation, new Vector2(32f, 32f), this.size * this.grow * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
