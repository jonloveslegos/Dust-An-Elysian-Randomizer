using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class BounceSpark : Particle
	{
		private Vector2 ploc;

		private float lifeSpan;

		private float frame;

		private float size;

		public BounceSpark(Vector2 loc, Vector2 traj, float size)
		{
			this.Reset(loc, traj, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size)
		{
			base.exists = Exists.Init;
			base.location = (this.ploc = loc);
			this.lifeSpan = Rand.GetRandomFloat(0.5f, 3f);
			this.frame = this.lifeSpan;
			base.renderState = RenderState.AdditiveOnly;
			this.size = size * Rand.GetRandomFloat(0.8f, 1.1f) * Game1.hiDefScaleOffset;
			base.trajectory = (traj *= Rand.GetRandomFloat(1f, 2f));
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.trajectory.Y += gameTime * 1000f;
			base.location += base.trajectory * gameTime;
			this.frame -= gameTime;
			Vector2 vector = base.GameLocation(l);
			if (!new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight).Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				base.Reset();
			}
			if ((l == 5 || l == 6) && map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) > 0f)
			{
				if (base.trajectory.Y > 500f)
				{
					this.Reset(base.location, Rand.GetRandomVector2(-100f, 100f, -300f, -100f), 0.2f);
					for (int i = 0; i < 4; i++)
					{
						pMan.AddBounceSpark(base.location, Rand.GetRandomVector2(-100f, 100f, -300f, -100f), 0.2f, l);
					}
				}
				else
				{
					base.Reset();
				}
			}
			this.ploc = base.location;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f;
			sprite.Draw(particlesTex[1], base.GameLocation(l), new Rectangle(1760, 128, 32, 32), new Color(1f, 1f, 1f, this.frame / (this.lifeSpan * 0.2f)), rotation, new Vector2(1f, 16f), new Vector2(this.size, this.size * 2f), SpriteEffects.None, 1f);
		}
	}
}
