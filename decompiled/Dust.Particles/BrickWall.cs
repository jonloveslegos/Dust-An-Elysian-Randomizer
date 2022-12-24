using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class BrickWall : Particle
	{
		private int animXOffset;

		private Rectangle sRect;

		private SpriteEffects flip;

		private float frame;

		private float size;

		private float rotation;

		private Vector2 ploc = Vector2.Zero;

		public BrickWall(Vector2 loc, Vector2 traj, float size)
		{
			this.Reset(loc, traj, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size)
		{
			base.exists = Exists.Init;
			this.frame = 0f;
			this.flip = ((Rand.GetRandomInt(0, 2) != 0) ? SpriteEffects.FlipVertically : SpriteEffects.None);
			base.location = (this.ploc = loc);
			base.trajectory = traj;
			base.trajectory.Y *= 2f;
			this.rotation = 0f;
			base.renderState = RenderState.Normal;
			this.size = _size;
			switch (Rand.GetRandomInt(0, 3))
			{
			default:
				this.sRect = new Rectangle(3198, 2970, 139, 59);
				break;
			case 1:
				this.sRect = new Rectangle(3198, 3029, 139, 60);
				break;
			case 2:
				this.sRect = new Rectangle(3337, 2970, 182, 114);
				break;
			case 3:
				this.sRect = new Rectangle(3519, 2970, 231, 158);
				break;
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.trajectory.Y += gameTime * 2000f;
			this.rotation += gameTime * base.trajectory.X / 100f;
			if (map.CheckCol(base.location) > 0)
			{
				base.location.X = this.ploc.X;
				base.trajectory.X = 0f - base.trajectory.X;
			}
			if (map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) > 0f)
			{
				if (this.size > 0.4f)
				{
					Sound.PlayDropCue("destruct_debris", base.location, this.size * 200f);
					base.Reset();
					float num = base.trajectory.Y * -0.5f;
					base.trajectory.Y = num;
					this.size /= 2f;
					for (int i = 0; i < 2; i++)
					{
						pMan.AddBlood(base.location - new Vector2(0f, 20f), new Vector2(Rand.GetRandomFloat(0f - base.trajectory.X, base.trajectory.X), num / 1.5f), 1f, 1f, 0.5f, 1f, this.size, (CharacterType)1000, 0, 5);
					}
				}
				else
				{
					base.Reset();
				}
			}
			this.ploc = base.location;
			base.location += base.trajectory * gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, Color.White, this.rotation, new Vector2(this.sRect.Width, this.sRect.Height) / 2f, this.size * worldScale, SpriteEffects.None, 1f);
		}
	}
}
