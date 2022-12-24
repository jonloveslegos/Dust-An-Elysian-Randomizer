using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class HudRain : Particle
	{
		private float r;

		private float gb;

		private float alpha;

		private float size;

		private float rotation;

		public HudRain(Vector2 loc, Vector2 traj, float size)
		{
			this.Reset(loc, traj, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size)
		{
			base.exists = Exists.Init;
			base.location = loc;
			this.alpha = Rand.GetRandomFloat(0.1f, 0.8f);
			base.trajectory = traj * Game1.hiDefScaleOffset;
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f;
			this.size = _size * Game1.hiDefScaleOffset;
			this.r = Rand.GetRandomFloat(0f, 1f);
			this.gb = Rand.GetRandomFloat(0.5f, 1f);
			base.renderState = RenderState.AdditiveOnly;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (!new Rectangle(0, -100, Game1.screenWidth + 200, Game1.screenHeight + 300).Contains((int)base.location.X, (int)base.location.Y))
			{
				this.Reset(Rand.GetRandomVector2(0f, (float)Game1.screenWidth + 200f * Game1.hiDefScaleOffset, -100f, 0f), Rand.GetRandomVector2(-800f, -200f, 1200f, 2000f), Rand.GetRandomFloat(0.5f, 2f));
			}
			base.location += base.trajectory * Game1.HudTime / Game1.worldScale;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[1], base.location, new Rectangle(143, 128, 4, 32), new Color(this.r, this.gb, this.gb, this.alpha), this.rotation, new Vector2(2f, 16f), this.size, SpriteEffects.None, 1f);
			sprite.Draw(particlesTex[1], base.location + new Vector2(200f, -100f), new Rectangle(143, 128, 4, 32), new Color(this.r, this.gb, this.gb, this.alpha), this.rotation, new Vector2(2f, 16f), this.size, SpriteEffects.None, 1f);
			sprite.Draw(particlesTex[1], base.location + new Vector2(-200f, 100f), new Rectangle(143, 128, 4, 32), new Color(this.r, this.gb, this.gb, this.alpha), this.rotation, new Vector2(2f, 16f), this.size, SpriteEffects.None, 1f);
		}
	}
}
