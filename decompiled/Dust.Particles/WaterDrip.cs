using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class WaterDrip : Particle
	{
		private Vector2 pLoc;

		private Rectangle sRect;

		private Rectangle fadeRect;

		private bool bounce;

		private float frame;

		private float size;

		public WaterDrip(Vector2 loc, float size)
		{
			this.Reset(loc, size);
		}

		public void Reset(Vector2 loc, float size)
		{
			base.exists = Exists.Init;
			this.bounce = false;
			base.background = false;
			base.trajectory = new Vector2(0f, Rand.GetRandomFloat(0f, 500f));
			this.size = size;
			base.location = (this.pLoc = loc);
			this.frame = 2f;
			base.renderState = RenderState.AllEffects;
			base.owner = 99;
			if (Rand.GetRandomInt(0, 2) == 0)
			{
				this.sRect = new Rectangle(3780, 538, 180, 62);
			}
			else
			{
				this.sRect = new Rectangle(3960, 538, 117, 62);
			}
			this.fadeRect = new Rectangle(3780 + Rand.GetRandomInt(0, 3) * 100, 228, 100, 100);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			if (l != 5)
			{
				Vector2 vector = base.GameLocation(l);
				if (!new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight + 1000).Contains((int)vector.X, (int)vector.Y))
				{
					base.Reset();
					return;
				}
			}
			else if (!this.bounce && ((this.frame < 1.5f && map.CheckPCol(base.location, this.pLoc, canFallThrough: false, init: false) > 0f) || (Game1.longSkipFrame == 2 && HitManager.CheckPickup(base.location, c))))
			{
				Vector2 vector2 = base.GameLocation(l);
				if (!new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight + 1000).Contains((int)vector2.X, (int)vector2.Y))
				{
					base.Reset();
					return;
				}
				base.trajectory = new Vector2(0f, -120f);
				this.frame = 0.45f;
				this.size = Rand.GetRandomFloat(0.3f, 0.7f);
				this.sRect = new Rectangle(3780 + Rand.GetRandomInt(0, 3) * 100, 228, 100, 100);
				base.renderState = RenderState.AllEffects;
				this.bounce = true;
				Sound.PlayCue("drip_single", base.location, (base.location - c[0].Location).Length());
			}
			base.trajectory.Y += gameTime * 800f;
			base.location += base.trajectory * gameTime;
			this.pLoc = base.location;
			if (this.frame < 0f)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float num = (this.frame - 0.2f) / 2f;
			Vector2 position = base.GameLocation(l);
			if (!this.bounce)
			{
				float x = this.size * Math.Max(base.trajectory.Y / 800f, 1f);
				if (Game1.pManager.renderingAdditive)
				{
					sprite.Draw(particlesTex[2], position, this.sRect, new Color(1f, 1f, 1f, 0.9f - num), -1.57f, new Vector2(50f, 31f), new Vector2(x, this.size * (2f - this.frame)) * num * worldScale, SpriteEffects.None, 1f);
				}
				else if (Game1.refractive)
				{
					sprite.Draw(particlesTex[2], position, this.fadeRect, new Color(1f, 1f, 1f, Math.Max(this.frame / 8f, 0.1f)), this.frame, new Vector2(50f, 50f), this.size / 3f * worldScale * (2f - this.frame), SpriteEffects.None, 1f);
				}
			}
			else if (Game1.pManager.renderingAdditive || Game1.refractive)
			{
				sprite.Draw(particlesTex[2], position, this.sRect, new Color(1f, 1f, 1f, num), 0f, new Vector2(50f, 75f), new Vector2((1f - num) / 2f, num * 4f) * this.size * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
