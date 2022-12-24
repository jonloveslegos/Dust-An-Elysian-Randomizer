using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FootStepSnow : Particle
	{
		private float size;

		private float targetSize;

		private float rotation;

		private float frame;

		private float r;

		private float g;

		private float b;

		public FootStepSnow(Vector2 loc, float size, float rot)
		{
			this.Reset(loc, size, rot);
		}

		public void Reset(Vector2 loc, float _size, float rot)
		{
			base.exists = Exists.Init;
			base.location = loc + new Vector2(0f, 24f);
			this.targetSize = _size * Rand.GetRandomFloat(0.9f, 1.4f);
			for (int i = 0; i < 2; i++)
			{
				Vector2 vector = base.location + new Vector2((float)(-50 + i * 100) * this.targetSize, -100f);
				bool flag = false;
				do
				{
					Vector2 pLoc = vector;
					vector.Y += 120f;
					if (Game1.map.CheckPCol(vector, pLoc, canFallThrough: false, init: false) > 0f)
					{
						flag = true;
					}
				}
				while (!flag && vector.Y < base.location.Y);
				if (!flag)
				{
					base.exists = Exists.Dead;
					base.Reset();
					return;
				}
			}
			base.flag = 1270 + 100 * (byte)Rand.GetRandomInt(0, 4);
			this.rotation = rot;
			this.size = 0f;
			this.frame = 4f;
			this.r = (float)(int)Game1.map.playerLayerColor.R / 255f;
			this.g = (float)(int)Game1.map.playerLayerColor.G / 255f;
			this.b = (float)(int)Game1.map.playerLayerColor.B / 255f;
			base.renderState = RenderState.Normal;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (c[0].Location.X > base.location.X - 120f && c[0].Location.X < base.location.X + 120f && c[0].Location.Y > base.location.Y - 300f && c[0].Location.Y < base.location.Y + 300f)
			{
				base.background = c[0].Location.Y > base.location.Y;
			}
			if (this.frame > 1f && this.size < this.targetSize)
			{
				this.size = Math.Min(this.size + gameTime * 10f, this.targetSize);
			}
			if (c[0].Trajectory.Length() > 2f || this.frame < 1f)
			{
				this.frame -= gameTime;
			}
			if (this.frame < 0f)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(base.flag, 2110, 100, 63), new Color(this.r, this.g, this.b, this.size * Math.Min(this.frame, 1f)), this.rotation, new Vector2(50f, 40f), new Vector2(this.targetSize * 1.2f, this.size) * worldScale, SpriteEffects.None, 1f);
		}
	}
}
