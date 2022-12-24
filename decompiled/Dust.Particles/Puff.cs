using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Puff : Particle
	{
		private float lifeSpan;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float a;

		private float size;

		private int srectX;

		public Puff(Vector2 loc, Vector2 traj, float rot, float r, float g, float b, float a, float size)
		{
			this.Reset(loc, traj, rot, r, g, b, a, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float rot, float r, float g, float b, float a, float size)
		{
			base.exists = Exists.Init;
			this.lifeSpan = Rand.GetRandomFloat(0.75f, 1.25f);
			base.location = new Vector2(loc.X - 16f, loc.Y);
			base.trajectory = traj;
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
			this.size = size;
			this.frame = this.lifeSpan;
			base.flag = Rand.GetRandomInt(0, 4);
			this.srectX = 910 + base.flag * 120 - base.flag / 2 * 240;
			base.flag = 1000 + base.flag / 2 * 120;
			base.exists = Exists.Exists;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(this.srectX, base.flag, 120, 120), new Color(new Vector4(this.r, this.g, this.b, this.a * this.frame)), 0f, new Vector2(60f, 60f), new Vector2(this.size, this.size * (this.lifeSpan - this.frame)) * worldScale, SpriteEffects.None, 1f);
		}
	}
}
