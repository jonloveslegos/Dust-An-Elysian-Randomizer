using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class LampDebris : Particle
	{
		private Rectangle sRect;

		private float size;

		private float rotation;

		private Vector2 ploc = Vector2.Zero;

		public LampDebris(Vector2 loc, Vector2 traj, float size, int flag)
		{
			this.Reset(loc, traj, size, flag);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size, int _flag)
		{
			base.exists = Exists.Init;
			base.flag = _flag;
			base.location = (this.ploc = loc);
			base.trajectory = traj;
			base.trajectory.Y *= 2f;
			this.rotation = Rand.GetRandomFloat(0f, 6.28f);
			this.size = _size;
			base.renderState = RenderState.Normal;
			switch (base.flag)
			{
			case 0:
				this.sRect = new Rectangle(390, 1000, 73, 86);
				break;
			case 1:
				this.sRect = new Rectangle(390, 1086, 46, 73);
				break;
			case 2:
				this.sRect = new Rectangle(390, 1159, 46, 53);
				break;
			case 3:
				this.sRect = new Rectangle(390, 1212, 46, 28);
				break;
			case 4:
				this.sRect = new Rectangle(436, 1086, 51, 105);
				break;
			case 5:
				this.sRect = new Rectangle(439, 1192, 74, 49);
				break;
			case 6:
				this.sRect = new Rectangle(463, 1000, 35, 47);
				break;
			case 7:
				this.sRect = new Rectangle(463, 1047, 24, 16);
				break;
			case 8:
				this.sRect = new Rectangle(463, 1063, 24, 23);
				break;
			case 9:
				this.sRect = new Rectangle(487, 1102, 23, 89);
				break;
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (base.GameLocation(l).Y > map.bottomEdge)
			{
				base.Reset();
			}
			base.trajectory.Y += Game1.HudTime * 2000f;
			if (base.trajectory.X > 0f)
			{
				this.rotation += Game1.HudTime * 2f;
			}
			else
			{
				this.rotation -= Game1.HudTime * 2f;
			}
			if (map.CheckCol(base.location) > 0)
			{
				base.location.X = this.ploc.X;
				base.trajectory.X = 0f - base.trajectory.X;
			}
			if (map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) > 0f)
			{
				this.sRect = new Rectangle(Rand.GetRandomInt(0, 3) * 64 + 1984, 2660, 64, 64);
				if (this.size > 0.4f)
				{
					Sound.PlayCue("challenge_lamp_debris", base.location, (base.location - c[0].Location).Length() / 2f);
					float y = base.trajectory.Y * -0.3f;
					base.trajectory.Y = y;
					this.size /= 4f;
				}
				else
				{
					base.Reset();
					pMan.AddFootStep(base.location, 1f, 0.5f, 5);
				}
			}
			this.ploc = base.location;
			base.location += base.trajectory * Game1.HudTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, Color.White, this.rotation, new Vector2(this.sRect.Width / 2, this.sRect.Height / 2), this.size * worldScale, SpriteEffects.None, 1f);
		}
	}
}
