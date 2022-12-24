using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class GroundDust : Particle
	{
		private SpriteEffects flip;

		private byte animFrame;

		private byte frameRate;

		private float frame;

		private Color color;

		private float size;

		private float rotation;

		public GroundDust(Vector2 loc, Vector2 traj, float size, float a, int type)
		{
			this.Reset(loc, traj, size, a, type);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size, float alpha, int type)
		{
			base.exists = Exists.Init;
			this.animFrame = 0;
			this.frame = 0f;
			this.size = size * Rand.GetRandomFloat(0.9f, 1.1f);
			switch (type)
			{
			case 2:
			case 3:
				base.background = false;
				base.renderState = RenderState.Refract;
				this.frameRate = (byte)Rand.GetRandomInt(45, 50);
				this.color = new Color(1f, 1f, 1f, alpha);
				break;
			case 4:
			case 5:
				base.background = true;
				base.renderState = RenderState.AdditiveOnly;
				this.frameRate = (byte)Rand.GetRandomInt(45, 50);
				this.color = new Color(1f, 1f, 1f, alpha);
				break;
			default:
			{
				base.background = true;
				base.renderState = RenderState.Additive;
				this.frameRate = (byte)Rand.GetRandomInt(20, 28);
				float num = alpha * Rand.GetRandomFloat(0.1f, 0.2f);
				this.color = new Color(num, num, num, alpha * Rand.GetRandomFloat(0.9f, 1.1f));
				break;
			}
			}
			base.flag = type - type / 2 * 2;
			if (base.flag == 0)
			{
				base.background = false;
				this.size *= 3f;
				base.location = loc + new Vector2(0f, 10f);
				if (Rand.GetRandomInt(0, 2) == 0)
				{
					this.flip = SpriteEffects.FlipHorizontally;
				}
				else
				{
					this.flip = SpriteEffects.None;
				}
			}
			else
			{
				this.size *= 2f;
				base.trajectory = traj;
				if (traj.X > 0f)
				{
					this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) - 3.14f;
					this.flip = SpriteEffects.None;
				}
				else
				{
					this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory);
					this.flip = SpriteEffects.FlipHorizontally;
				}
				base.location = loc + new Vector2(0f, 100f * size);
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame += gameTime * (float)(int)this.frameRate;
			if (this.frame > 1f)
			{
				this.frame = 0f;
				this.animFrame++;
				if (this.animFrame > 19)
				{
					this.animFrame = 19;
					base.Reset();
				}
			}
			if (base.flag == 1)
			{
				base.location += base.trajectory * gameTime;
				this.size += gameTime / 2f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (base.flag == 0)
			{
				sprite.Draw(particlesTex[2], base.location * worldScale - Game1.Scroll, new Rectangle(200 * this.animFrame, 600, 200, 200), this.color, 0f, new Vector2(100f, 190f), new Vector2(this.size * 1.5f, this.size) * worldScale, this.flip, 1f);
			}
			else
			{
				sprite.Draw(particlesTex[2], base.location * worldScale - Game1.Scroll, new Rectangle(200 * this.animFrame, 800, 200, 200), this.color, this.rotation, new Vector2(100f, 200f), this.size * worldScale, this.flip, 1f);
			}
		}
	}
}
