using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class ComboBreak : Particle
	{
		private float frame;

		private float size;

		private float rotation;

		public ComboBreak(Vector2 loc, float size, int flag)
		{
			this.Reset(loc, size, flag);
		}

		public void Reset(Vector2 loc, float size, int flag)
		{
			base.exists = Exists.Init;
			if (flag < 6)
			{
				base.flag = flag;
			}
			else if (flag < 12)
			{
				base.flag = flag - 6;
			}
			else
			{
				base.flag = flag - 12;
			}
			base.location = loc + Rand.GetRandomVector2(-100f, 100f, -100f, 100f) * size;
			base.trajectory = Rand.GetRandomVector2(-800f, 500f, -800f, 100f) * size;
			this.size = size * Rand.GetRandomFloat(0.8f, 1.2f);
			this.rotation = Rand.GetRandomFloat(0f, 3.14f);
			base.renderState = RenderState.AdditiveOnly;
			this.frame = 0.65f;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame <= 0f || Game1.gameMode != Game1.GameModes.Game)
			{
				base.Reset();
			}
			base.trajectory.X *= 0.9f;
			if (base.trajectory.Y < 1000f)
			{
				base.trajectory.Y += gameTime * 2000f;
			}
			base.location += base.trajectory * gameTime;
			this.rotation += gameTime;
			this.frame -= Game1.FrameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (Game1.FrameTime > 0f)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(540 + base.flag * 50, 2130, 50, 70), Color.White * this.frame, this.rotation, new Vector2(25f, 35f), this.size, SpriteEffects.None, 1f);
			}
		}
	}
}
