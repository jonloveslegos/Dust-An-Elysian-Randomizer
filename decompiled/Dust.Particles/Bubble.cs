using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Bubble : Particle
	{
		private float lifeSpan;

		private int sRectOffset;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float size;

		private float rotation;

		private Rectangle renderRect;

		public Bubble(Vector2 loc, Vector2 traj, float size, float r, float g, float b)
		{
			this.Reset(loc, traj, size, r, g, b);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size, float r, float g, float b)
		{
			base.exists = Exists.Init;
			this.r = r;
			this.g = g;
			this.b = b;
			this.sRectOffset = 0;
			this.lifeSpan = Rand.GetRandomFloat(0.6f, 2f);
			this.frame = this.lifeSpan;
			this.size = size;
			base.location = loc + Rand.GetRandomVector2(-40f, 40f, -40f, 40f) * size;
			base.trajectory = traj * size;
			this.rotation = 0f;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				base.background = true;
			}
			base.renderState = RenderState.AdditiveOnly;
			this.renderRect = new Rectangle(-200, -100, Game1.screenWidth + 400, Game1.screenHeight + 200);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.trajectory.X = (float)Math.Cos((double)map.MapSegFrame + (double)(this.frame * 4f)) * 50f;
			float num = this.frame;
			this.frame -= gameTime;
			if (this.frame < 0.05f && num > 0.05f)
			{
				this.sRectOffset = 50;
				this.rotation = Rand.GetRandomFloat(0f, 3.14f);
			}
			base.location += base.trajectory * gameTime;
			this.size += Game1.FrameTime / 3f;
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(1640 + this.sRectOffset, 2060, 50, 50), new Color(this.r, this.g, this.b, 1f), this.rotation, new Vector2(25f, 25f), this.size * worldScale, SpriteEffects.None, 0f);
		}
	}
}
