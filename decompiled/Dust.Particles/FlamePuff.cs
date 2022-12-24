using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FlamePuff : Particle
	{
		private int sRectOffsetY;

		private byte animFrame;

		private float animFrameTime;

		private float size;

		public FlamePuff(Vector2 loc, Vector2 traj, float size)
		{
			this.Reset(loc, traj, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size)
		{
			base.exists = Exists.Init;
			this.animFrame = 0;
			this.sRectOffsetY = 3066 + Rand.GetRandomInt(0, 2) * 100;
			base.location = loc;
			base.trajectory = traj;
			this.size = _size;
			this.animFrameTime = 0f;
			base.flag = Rand.GetRandomInt(0, 2);
			base.renderState = RenderState.AdditiveOnly;
			base.background = true;
			base.maskGlow = 0f;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.location += base.trajectory * gameTime;
			if (this.animFrame < 4)
			{
				base.maskGlow += gameTime * 2f;
			}
			else
			{
				base.maskGlow -= gameTime * 1f;
			}
			this.animFrameTime += gameTime * 24f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 15)
				{
					this.animFrame = 15;
					base.Reset();
				}
				this.animFrameTime = 0f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(1303 + this.animFrame * 80, this.sRectOffsetY, 80, 100), Color.White, 0f, new Vector2(40f, 50f), this.size * worldScale, (base.flag != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
		}
	}
}
