using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class HudSmoke : Particle
	{
		private float lifeSpan;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float a;

		private float size;

		private float rotation;

		private int srectX;

		public HudSmoke(Vector2 loc, Vector2 traj, float r, float g, float b, float a, float size, float _lifeSpan)
		{
			this.Reset(loc, traj, r, g, b, a, size, _lifeSpan);
		}

		public void Reset(Vector2 loc, Vector2 traj, float r, float g, float b, float a, float size, float _lifeSpan)
		{
			base.exists = Exists.Init;
			base.background = true;
			base.location = loc;
			base.trajectory = traj;
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
			this.size = size;
			this.frame = (this.lifeSpan = _lifeSpan);
			base.renderState = RenderState.AdditiveOnly;
			base.flag = Rand.GetRandomInt(0, 4);
			this.srectX = 910 + base.flag * 120 - base.flag / 2 * 240;
			base.flag = 1000 + base.flag / 2 * 120;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			base.trajectory.Y *= 0.994f;
			base.trajectory.X *= 0.99f;
			base.location += base.trajectory * Game1.HudTime;
			if (base.trajectory.X < 0f)
			{
				this.rotation += base.trajectory.Y * Game1.HudTime / 600f;
			}
			else
			{
				this.rotation -= base.trajectory.Y * Game1.HudTime / 600f;
			}
			this.frame -= Game1.HudTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 position = base.location;
			if (Game1.navManager.worldMap != null)
			{
				position = base.location + Game1.navManager.worldMap.WorldMapPos(background: true);
			}
			sprite.Draw(particlesTex[2], position, new Rectangle(this.srectX, base.flag, 120, 120), new Color(new Vector4(this.r, this.g, this.b, this.a * (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan)))), this.rotation, new Vector2(60f, 60f), new Vector2(this.size + (1f - this.frame / this.lifeSpan), this.size * 1f + this.frame * this.size * 0.1f + (1f - this.frame / this.lifeSpan)), SpriteEffects.None, 1f);
		}
	}
}
