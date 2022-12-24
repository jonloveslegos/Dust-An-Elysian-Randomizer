using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class SquirtFlame : Particle
	{
		private byte animFrame;

		private byte frameRate;

		private int sRectOffsetX;

		private float animFrameTime;

		private float heatColor;

		private float g;

		private float b;

		private float size;

		private float rotation;

		private Vector2 ploc;

		private Rectangle sRect;

		public SquirtFlame(Vector2 loc, Vector2 traj, int owner)
		{
			this.Reset(loc, traj, owner);
		}

		public void Reset(Vector2 loc, Vector2 traj, int owner)
		{
			base.exists = Exists.Init;
			this.animFrame = 0;
			this.sRectOffsetX = 1740 + Rand.GetRandomInt(0, 2) * 1024;
			this.frameRate = 0;
			base.location = (this.ploc = loc);
			this.g = Rand.GetRandomFloat(0.25f, 1f);
			this.b = Rand.GetRandomFloat(0f, 0.5f);
			this.animFrameTime = 0f;
			base.trajectory = traj * 1000f;
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) - 1.57f;
			this.size = Rand.GetRandomFloat(1f, 1.4f);
			base.owner = owner;
			base.strength = (int)MathHelper.Clamp(Math.Min((float)Game1.character[0].MaxHP * Game1.stats.bonusHealth, 9999f) * 0.15f, 0f, 60f);
			if (Rand.GetRandomInt(0, 10) == 0)
			{
				this.frameRate = (byte)Rand.GetRandomInt(12, 36);
				base.renderState = RenderState.RefractOnly;
			}
			else
			{
				this.frameRate = 28;
				base.renderState = RenderState.Additive;
				this.heatColor = Rand.GetRandomFloat(0f, 0.2f);
			}
			this.sRect = new Rectangle(0, 2074, 64, 126);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.ploc = base.location;
			base.trajectory.Y -= gameTime * 200f;
			base.location += base.trajectory * gameTime;
			this.heatColor += gameTime * 1.5f;
			if (this.heatColor > 1f)
			{
				this.heatColor = 1f;
			}
			_ = base.renderState;
			_ = 1;
			this.animFrameTime += gameTime * (float)(int)this.frameRate;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 15)
				{
					base.Reset();
				}
				this.animFrameTime = 0f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			this.sRect.X = this.animFrame * 64 + this.sRectOffsetX;
			if (!Game1.refractive)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(this.heatColor, this.g * this.heatColor, this.b, 1f), this.rotation, new Vector2(32f, 96f), new Vector2(this.size * 2f, this.size * 1.2f) * worldScale, SpriteEffects.None, 1f);
			}
			else
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(1f, 1f, 1f, 0.1f), this.rotation, new Vector2(32f, 64f), new Vector2(this.size * 6f, this.size * 4f) * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
