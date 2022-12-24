using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class HitSpark : Particle
	{
		private Vector2 origin;

		private byte animFrame;

		private byte animSpeed;

		private byte maxFrames;

		private byte type;

		private SpriteEffects flip;

		private Rectangle sRect;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float a;

		private float size;

		private float rotation;

		public HitSpark(Vector2 loc, float size, float rot, CharDir dir)
		{
			this.Reset(loc, size, rot, dir);
		}

		public void Reset(Vector2 loc, float size, float rot, CharDir dir)
		{
			base.exists = Exists.Init;
			this.frame = 0f;
			this.animFrame = 0;
			this.animSpeed = (byte)Rand.GetRandomInt(24, 30);
			this.type = (byte)Rand.GetRandomInt(0, 3);
			base.location = loc;
			this.size = size * Rand.GetRandomFloat(0.6f, 1.4f);
			this.rotation = rot + Rand.GetRandomFloat(-0.3f, 0.3f);
			this.r = Rand.GetRandomFloat(0.5f, 1f);
			this.b = Rand.GetRandomFloat(0.5f, 1f);
			this.g = Rand.GetRandomFloat(0.5f, 1f);
			base.renderState = RenderState.AdditiveOnly;
			if (dir == CharDir.Right)
			{
				this.flip = SpriteEffects.None;
				this.origin = new Vector2(0f, 64f);
			}
			else
			{
				this.flip = SpriteEffects.FlipHorizontally;
				this.origin = new Vector2(128f, 64f);
			}
			if (this.type == 0)
			{
				this.maxFrames = 4;
				this.sRect = new Rectangle(3008, 2328, 128, 128);
			}
			else if (this.type == 1 || this.type == 2)
			{
				this.maxFrames = 5;
				this.origin.Y = 35f;
				if (this.type == 1)
				{
					this.sRect = new Rectangle(2560, 2456, 128, 70);
				}
				else if (this.type == 2)
				{
					this.sRect = new Rectangle(3328, 2456, 128, 70);
				}
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame += gameTime * (float)(int)this.animSpeed;
			if (this.frame > 1f)
			{
				this.animFrame++;
				this.a /= 1.75f;
				if (this.animFrame > this.maxFrames)
				{
					this.animFrame = 20;
					base.Reset();
				}
				this.frame -= 1f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.type == 0)
			{
				this.sRect.X = 3008 + 128 * this.animFrame;
			}
			else if (this.type == 1)
			{
				this.sRect.X = 2560 + 128 * this.animFrame;
			}
			else if (this.type == 2)
			{
				this.sRect.X = 3328 + 128 * this.animFrame;
			}
			else
			{
				this.sRect.X = 3008 + 128 * this.animFrame;
			}
			sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(this.r, this.g, this.b, this.a), this.rotation, this.origin, new Vector2(this.size + (float)(int)this.animFrame * 0.2f, this.size * 0.75f) * worldScale, this.flip, 0f);
		}
	}
}
