using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class PondRipple : Particle
	{
		private float frame;

		private float size;

		private Rectangle renderRect = new Rectangle(-60, -60, Game1.screenWidth + 120, Game1.screenHeight + 120);

		public PondRipple(Vector2 loc, float size)
		{
			this.Reset(loc, size);
		}

		public void Reset(Vector2 loc, float size)
		{
			base.exists = Exists.Init;
			base.background = true;
			base.location = loc;
			this.size = size;
			this.frame = 1f;
			base.renderState = RenderState.RefractOnly;
			this.renderRect = new Rectangle(-60, -60, Game1.screenWidth + 120, Game1.screenHeight + 120);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				base.Reset();
			}
			this.size += Game1.FrameTime;
			this.frame -= Game1.FrameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l);
			Rectangle value = new Rectangle(1644, 128, 84, 32);
			float scale = this.size * worldScale;
			Color color = new Color(1f, 1f, 1f, this.frame);
			for (int i = 0; i < 3; i++)
			{
				int num = ((i == 1) ? 1 : 0);
				sprite.Draw(particlesTex[1], vector + new Vector2(i * 150 - 150, -num * 50) * worldScale, value, color, 0f, new Vector2(60f, 16f), scale, SpriteEffects.None, 1f);
			}
		}
	}
}
