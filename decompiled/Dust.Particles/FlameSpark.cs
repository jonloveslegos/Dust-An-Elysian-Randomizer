using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FlameSpark : Particle
	{
		private float lifeSpan;

		private byte type;

		private float frame;

		private float size;

		private float rotation;

		public FlameSpark(Vector2 loc, Vector2 traj, float rot, float size, int type)
		{
			this.Reset(loc, traj, rot, size, type);
		}

		public void Reset(Vector2 loc, Vector2 traj, float rot, float size, int type)
		{
			base.exists = Exists.Init;
			this.lifeSpan = Rand.GetRandomFloat(0.5f, 1.5f);
			base.location = loc;
			base.trajectory = traj;
			this.size = size;
			this.type = (byte)type;
			this.frame = this.lifeSpan;
			base.background = true;
			if (type == 1)
			{
				Game1.wManager.fireCount++;
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!new Rectangle(-10, -10, Game1.screenWidth + 20, Game1.screenHeight + 120).Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				if (Game1.wManager.weatherType == WeatherType.Fire && l == 8)
				{
					this.frame = Rand.GetRandomFloat(0.5f, 1.5f);
					base.trajectory = Rand.GetRandomVector2(-200f, 200f, -1000f, -200f);
					this.rotation = 90f;
					this.size = Rand.GetRandomFloat(1f, 2f);
					base.location = Game1.Scroll * base.LayerScale(l) / Game1.worldScale;
					base.location += new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - Game1.worldScale) / base.LayerScale(l);
					base.location += Rand.GetRandomVector2(0f, Game1.screenWidth, (float)Game1.screenHeight * 0.5f, Game1.screenHeight + 130) / Game1.worldScale;
				}
				else
				{
					base.Reset();
				}
			}
			base.trajectory.X += (Rand.GetRandomFloat(-0.25f, 2f) - this.frame) * Rand.GetRandomFloat(10f, 40f);
			base.location += base.trajectory * Game1.worldScale * gameTime;
			this.frame -= gameTime;
			if (this.frame < this.lifeSpan * 0.9f)
			{
				if (base.trajectory.Y < -10f)
				{
					base.trajectory.Y += gameTime * 20f;
				}
				if (base.trajectory.X < -10f)
				{
					base.trajectory.X += gameTime * 15f;
				}
				if (base.trajectory.X > 10f)
				{
					base.trajectory.X -= gameTime * 15f;
				}
			}
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory);
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[1], base.GameLocation(l), new Rectangle(12, 140, 6, 8), new Color(1f, 1f, 1f, this.frame / (this.lifeSpan * 0.2f)), this.rotation, new Vector2(3f, 4f), this.frame * this.size * new Vector2(4f, 1f) * worldScale, SpriteEffects.None, 1f);
		}
	}
}
