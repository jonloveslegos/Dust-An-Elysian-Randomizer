using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class HudFire : Particle
	{
		private float lifeSpan;

		private byte type;

		private float frame;

		private float size;

		private float rotation;

		public HudFire(Vector2 loc, Vector2 traj, float rot, float size, int type)
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
			base.GameLocation(l);
			if (this.frame < 0f)
			{
				base.Reset();
			}
			base.trajectory.X += (Rand.GetRandomFloat(-0.25f, 2f) - this.frame) * Rand.GetRandomFloat(10f, 40f);
			base.location += base.trajectory * Game1.worldScale * Game1.HudTime;
			this.frame -= Game1.HudTime;
			if (this.frame < this.lifeSpan * 0.9f)
			{
				if (base.trajectory.Y < -10f)
				{
					base.trajectory.Y += Game1.HudTime * 20f;
				}
				if (base.trajectory.X < -10f)
				{
					base.trajectory.X += Game1.HudTime * 15f;
				}
				if (base.trajectory.X > 10f)
				{
					base.trajectory.X -= Game1.HudTime * 15f;
				}
			}
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory);
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 position = base.location;
			if (Game1.navManager.worldMap != null)
			{
				position = base.location + Game1.navManager.worldMap.WorldMapPos(background: true);
			}
			sprite.Draw(particlesTex[1], position, new Rectangle(12, 140, 6, 8), new Color(1f, 1f, 1f, this.frame / (this.lifeSpan * 0.2f)), this.rotation, new Vector2(3f, 4f), this.frame * this.size * new Vector2(4f, 1f) * worldScale, SpriteEffects.None, 1f);
		}
	}
}
