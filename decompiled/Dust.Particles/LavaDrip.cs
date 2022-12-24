using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class LavaDrip : Particle
	{
		private Vector2 pLoc;

		private int sRectX;

		private float frame;

		private float size;

		private float rotation;

		public LavaDrip(Vector2 loc, Vector2 traj, float _size)
		{
			this.Reset(loc, traj, _size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size)
		{
			base.exists = Exists.Init;
			base.background = false;
			base.trajectory = traj;
			this.size = _size;
			base.location = (this.pLoc = loc);
			this.frame = 3f;
			base.renderState = RenderState.Normal;
			this.sRectX = 594 + 12 * Rand.GetRandomInt(0, 4);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			base.trajectory.Y += gameTime * 800f;
			base.trajectory.X *= 0.999999f;
			base.location += base.trajectory * gameTime;
			Vector2 vector = base.PlayerLayerLoc(base.location, l);
			if (this.frame < 2.5f && map.CheckPCol(vector, this.pLoc, canFallThrough: false, init: false) > 0f)
			{
				Vector2 vector2 = base.GameLocation(l);
				if (!new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight + 1000).Contains((int)vector2.X, (int)vector2.Y))
				{
					base.Reset();
					return;
				}
				this.frame = -1f;
				if (l >= 5)
				{
					pMan.AddEmitLava(vector, Vector2.Zero, 0.2f, colliding: true, 3, 6);
					if (!Game1.events.anyEvent)
					{
						Sound.PlayCue("lava_drip", vector, (vector - c[0].Location).Length() * 4f);
					}
				}
			}
			this.pLoc = vector;
			if (this.frame < 0f)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 position = base.GameLocation(l);
			sprite.Draw(particlesTex[2], position, new Rectangle(this.sRectX, 3648, 12, 38), Color.White * this.frame, GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f, new Vector2(6f, 19f), this.size * worldScale, SpriteEffects.None, 1f);
		}
	}
}
