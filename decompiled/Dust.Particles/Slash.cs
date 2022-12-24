using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Slash : Particle
	{
		private float color;

		private float a;

		private float size;

		private float rotation;

		private float animFrameTime;

		private byte animFrame;

		private byte animSpeed;

		private bool slashCurved;

		private bool slashCurveDown;

		private SpriteEffects flip;

		public Slash(Vector2 loc, float size, float rot, CharDir dir)
		{
			this.Reset(loc, size, rot, dir);
		}

		public void Reset(Vector2 loc, float size, float rot, CharDir dir)
		{
			base.exists = Exists.Init;
			this.animFrameTime = 0f;
			this.animFrame = 0;
			this.animSpeed = (byte)Rand.GetRandomInt(20, 28);
			this.slashCurved = ((Rand.GetRandomInt(0, 2) == 0) ? true : false);
			this.slashCurveDown = ((Rand.GetRandomInt(0, 2) == 0) ? true : false);
			base.location = loc;
			this.size = size * Rand.GetRandomFloat(0.5f, 2.2f) * Game1.hiDefScaleOffset;
			this.rotation = rot + Rand.GetRandomFloat(-0.3f, 0.3f);
			this.color = Rand.GetRandomFloat(0.75f, 1f);
			this.a = 1f;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				base.renderState = RenderState.AdditiveOnly;
			}
			else
			{
				base.renderState = RenderState.Refract;
			}
			if (dir == CharDir.Right)
			{
				this.flip = SpriteEffects.None;
			}
			else
			{
				this.flip = SpriteEffects.FlipHorizontally;
			}
			if (!this.slashCurved)
			{
				base.flag = 2456;
			}
			else
			{
				base.flag = 2526;
				if (this.slashCurveDown)
				{
					if (this.flip == SpriteEffects.FlipHorizontally)
					{
						this.flip = SpriteEffects.None;
					}
					else
					{
						this.flip = SpriteEffects.FlipHorizontally;
					}
				}
			}
			Game1.map.slashCount++;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.animFrameTime += gameTime * (float)(int)this.animSpeed;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				this.a *= 0.4f;
				if (this.animFrame > 3)
				{
					this.animFrame = 3;
					Game1.map.slashCount--;
					base.Reset();
				}
				this.animFrameTime = 0f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (l == 9 && !(Game1.FrameTime > 0f))
			{
				return;
			}
			Rectangle value = new Rectangle(640 * this.animFrame, base.flag, 640, 70);
			if (!this.slashCurved)
			{
				if (!Game1.refractive)
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), value, new Color(this.color, this.color, this.color, this.a), this.rotation, new Vector2(320f, 35f), new Vector2(this.size * 1.2f, this.size * 0.75f), this.flip, 0f);
				}
				else if (Game1.refractive)
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), value, new Color(this.color, this.color, this.color, this.a * 0.6f), this.rotation, new Vector2(320f, 35f), new Vector2(this.size * 1.4f, this.size * 0.9f), this.flip, 0f);
				}
			}
			else if (!this.slashCurveDown)
			{
				if (!Game1.refractive)
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), value, new Color(this.color, this.color, this.color, this.a), this.rotation, new Vector2(320f, 70f), new Vector2(this.size * 1.2f, this.size * 1.2f), this.flip, 0f);
				}
				else if (Game1.refractive)
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), value, new Color(this.color, this.color, this.color, this.a * 0.6f), this.rotation, new Vector2(320f, 70f), new Vector2(this.size * 1.4f, this.size * 1.4f), this.flip, 0f);
				}
			}
			else if (!Game1.refractive)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), value, new Color(this.color, this.color, this.color, this.a), this.rotation + 3.14f, new Vector2(320f, 70f), new Vector2(this.size * 1.2f, this.size * 1.2f), this.flip, 0f);
			}
			else if (Game1.refractive)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), value, new Color(this.color, this.color, this.color, this.a * 0.6f), this.rotation + 3.14f, new Vector2(320f, 70f), new Vector2(this.size * 1.4f, this.size * 1.4f), this.flip, 0f);
			}
		}
	}
}
