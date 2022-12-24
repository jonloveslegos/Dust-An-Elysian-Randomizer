using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class DustSwordFall : Particle
	{
		private float offSet;

		private int animFrame;

		private float frame;

		private float rotation;

		public DustSwordFall(Vector2 loc, Vector2 traj)
		{
			this.Reset(loc, traj);
		}

		public void Reset(Vector2 loc, Vector2 traj)
		{
			base.exists = Exists.Init;
			this.offSet = 0f;
			this.animFrame = 0;
			this.rotation = 0f;
			base.location = loc;
			base.trajectory = traj;
			this.frame = 0f;
			base.renderState = RenderState.Additive;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (base.trajectory.Y < 1500f)
			{
				base.trajectory.Y += gameTime * 1000f;
			}
			base.location += base.trajectory * gameTime;
			if (base.location.Y > map.bottomEdge)
			{
				base.Reset();
			}
			if (base.trajectory.X > 0f)
			{
				this.rotation += gameTime * 20f;
			}
			else
			{
				this.rotation -= gameTime * 20f;
			}
			this.frame += gameTime * 24f;
			if (this.frame > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 38)
				{
					this.animFrame = 0;
				}
				this.frame = 0f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(sourceRectangle: new Rectangle(632 + this.animFrame * 40 - 800 * (this.animFrame / 20), 2596 + 100 * (this.animFrame / 20), 40, 100), texture: particlesTex[2], position: base.location * Game1.worldScale - Game1.Scroll, color: Color.White, rotation: this.rotation, origin: new Vector2(20f, 50f), scale: 1.8f * worldScale, effects: SpriteEffects.None, layerDepth: 1f);
		}
	}
}
