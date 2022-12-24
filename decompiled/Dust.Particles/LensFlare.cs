using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class LensFlare : Particle
	{
		private float LifeSpan;

		private float innerSize;

		private float frame;

		private float size;

		public LensFlare(Vector2 loc, float lifeSpan, int _flag)
		{
			this.Reset(loc, lifeSpan, _flag);
		}

		public void Reset(Vector2 loc, float lifeSpan, int _flag)
		{
			base.exists = Exists.Init;
			base.location = new Vector2(loc.X - 16f, loc.Y);
			this.size = (this.innerSize = 0.1f);
			this.frame = (this.LifeSpan = lifeSpan);
			base.renderState = RenderState.AllEffects;
			base.flag = _flag;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			this.frame -= gameTime;
			this.size += this.frame / this.size / 5f;
			this.innerSize += this.frame / this.size / 1f;
		}

		private void DrawCorona(SpriteBatch sprite, Texture2D[] particlesTex, Vector2 gameLoc, float alpha, int l)
		{
			Vector2 vector = new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2);
			Vector2 vector2 = gameLoc - vector;
			float num = Math.Abs(vector2.X + vector2.Y) / 2000f;
			if (!(num < 1f))
			{
				return;
			}
			Color color = new Color(1f, 1f, 1f, (1f - num) * alpha);
			for (int i = 0; i < 8; i++)
			{
				Vector2 vector3 = vector - vector2 * ((float)i / 2f - 1.75f);
				Rectangle value;
				Vector2 origin;
				Vector2 vector4;
				float rotation;
				switch (i)
				{
				case 1:
					value = new Rectangle(3879, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(1f, 1f);
					rotation = 0f;
					break;
				case 2:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, (0.5f - num) * alpha);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.5f, 0.5f);
					rotation = 0f;
					break;
				case 3:
					value = new Rectangle(3879, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.25f, 0.25f);
					rotation = 0f;
					break;
				case 5:
					value = new Rectangle(3437, 1860, 560, 150);
					color = new Color(1f, 1f, 1f, (0.5f - num) * alpha);
					origin = new Vector2(280f, 100f);
					vector4 = new Vector2(1f, 2f) * (num + 0.75f);
					rotation = GlobalFunctions.GetAngle(vector3, vector) + 1.57f;
					break;
				case 4:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, (0.5f - num) * alpha);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.5f, 0.5f);
					rotation = 0f;
					break;
				default:
					value = new Rectangle(3978, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(1f, 1f);
					rotation = 0f;
					break;
				}
				sprite.Draw(particlesTex[2], vector3, value, color, rotation, origin, vector4 * Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
			}
			sprite.Draw(particlesTex[2], gameLoc, new Rectangle(2940, 1860, 497, 150), new Color(1f, 1f, 1f, (1f - num) * alpha), 0f, new Vector2(248f, 75f), Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l);
			float num = this.LifeSpan * (this.frame / this.LifeSpan);
			if (Game1.pManager.renderingAdditive)
			{
				this.DrawCorona(sprite, particlesTex, vector, num * 2f, l);
			}
			else if (base.flag > 0)
			{
				sprite.Draw(particlesTex[2], vector, new Rectangle(2240, 2724, 64, 64), new Color(new Vector4(1f, 1f, 1f, num)), 0f, new Vector2(32f, 32f), this.size * worldScale, SpriteEffects.None, 1f);
				sprite.Draw(particlesTex[2], vector, new Rectangle(2304, 2724, 64, 64), new Color(new Vector4(1f, 1f, 1f, num)), 0f, new Vector2(32f, 32f), this.innerSize * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
