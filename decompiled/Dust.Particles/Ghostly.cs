using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Ghostly : Particle
	{
		private SpriteEffects spriteDir;

		private float lifeSpan;

		private float frame;

		private float alpha;

		private float size;

		private Rectangle renderRect;

		public Ghostly(Vector2 loc, Vector2 traj, float size)
		{
			this.Reset(loc, traj, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size)
		{
			base.exists = Exists.Init;
			this.lifeSpan = Rand.GetRandomFloat(0.2f, 0.8f);
			base.background = true;
			base.location = loc;
			base.trajectory = traj;
			this.alpha = Rand.GetRandomFloat(0.2f, 1f) * size;
			this.size = size;
			this.frame = this.lifeSpan;
			base.renderState = RenderState.AdditiveOnly;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.spriteDir = SpriteEffects.FlipHorizontally;
			}
			else
			{
				this.spriteDir = SpriteEffects.None;
			}
			this.renderRect = new Rectangle(-400, -400, Game1.screenWidth + 800, Game1.screenHeight + 800);
			base.flag = 3780 + Rand.GetRandomInt(0, 3) * 100;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			this.size += Game1.FrameTime;
			base.location += base.trajectory * gameTime;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (this.renderRect.Contains((int)vector.X, (int)vector.Y))
			{
				for (int i = 0; i < 3; i++)
				{
					int num = ((i == 1) ? 1 : 0);
					sprite.Draw(particlesTex[2], base.GameLocation(l) + new Vector2((float)i * (60f * this.size) - 60f * this.size, (float)(-num) * (300f * this.size)), new Rectangle(base.flag, 427, 99, 111), new Color(new Vector4(1f, 1f, 1f, this.alpha * (this.lifeSpan - (this.lifeSpan - this.frame)))), 0f, new Vector2(50f, 10f), new Vector2(this.size, this.size * 2f) * worldScale, this.spriteDir, 1f);
				}
			}
		}
	}
}
