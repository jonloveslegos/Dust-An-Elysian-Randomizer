using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class DustSword : Particle
	{
		private float offSet;

		private int animFrame;

		private float frame;

		private float a;

		private float[] rotation;

		private Vector2[] trailLoc;

		public DustSword(Vector2 loc, Vector2 traj)
		{
			this.Reset(loc, traj);
		}

		public void Reset(Vector2 loc, Vector2 traj)
		{
			base.exists = Exists.Init;
			this.offSet = 0f;
			this.animFrame = 0;
			this.a = 1f;
			this.rotation = new float[16];
			this.trailLoc = new Vector2[this.rotation.Length];
			base.location = loc;
			for (int i = 0; i < this.rotation.Length; i++)
			{
				ref Vector2 reference = ref this.trailLoc[i];
				reference = base.location;
			}
			base.trajectory = traj;
			this.frame = 0f;
			base.renderState = RenderState.Additive;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			float num = (float)Math.Cos((double)map.MapSegFrame * 10.0) * 50f;
			float num2 = (float)Math.Sin((double)map.MapSegFrame * 10.0) * 50f;
			if (Game1.character[0].Face == CharDir.Left)
			{
				this.offSet -= Game1.FrameTime * 400f;
				if (this.offSet < -150f)
				{
					this.offSet = -150f;
				}
			}
			else
			{
				this.offSet += Game1.FrameTime * 400f;
				if (this.offSet > 150f)
				{
					this.offSet = 150f;
				}
			}
			if (Game1.events.subEvent < 15)
			{
				base.trajectory = new Vector2((Game1.character[0].Location.X - base.location.X + this.offSet + num) * 5f, (Game1.character[0].Location.Y - base.location.Y - 150f + num2) * 5f);
				this.rotation[0] = (Game1.character[0].Location.X + base.location.X) / 80f - 2.2f;
			}
			else
			{
				base.trajectory = new Vector2((Game1.character[0].Location.X + 50f - base.location.X) * 5f, (Game1.character[0].Location.Y - base.location.Y - 50f) * 5f);
				this.rotation[0] += (155f - this.rotation[0]) * gameTime;
			}
			base.location += base.trajectory * gameTime / 8f;
			if (Rand.GetRandomInt(0, 4) == 0)
			{
				pMan.AddSparkle(base.location + Rand.GetRandomVector2(-80f, 80f, -80f, 80f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.75f, Rand.GetRandomFloat(0.05f, 0.3f), Rand.GetRandomInt(24, 48), 6);
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
			for (int i = 0; i < this.rotation.Length; i++)
			{
				if (i == 0)
				{
					this.trailLoc[i] += (base.location - this.trailLoc[i]) * gameTime * 20f;
					continue;
				}
				this.rotation[i] += (this.rotation[i - 1] - this.rotation[i]) * gameTime * 20f;
				this.trailLoc[i] += (this.trailLoc[i - 1] - this.trailLoc[i]) * gameTime * 20f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Rectangle value = new Rectangle(632 + this.animFrame * 40 - 800 * (this.animFrame / 20), 2596 + 100 * (this.animFrame / 20), 40, 100);
			if (!Game1.pManager.renderingAdditive)
			{
				sprite.Draw(particlesTex[2], base.location * Game1.worldScale - Game1.Scroll, value, new Color(1f, 1f, 1f, this.a), this.rotation[0], new Vector2(20f, 50f), 1.5f * worldScale, SpriteEffects.None, 1f);
				return;
			}
			for (int i = 1; i < 16; i++)
			{
				sprite.Draw(particlesTex[2], this.trailLoc[i] * Game1.worldScale - Game1.Scroll, value, new Color(1f, 1f, 1f, (this.a - (float)i * 0.05f) / 4f), this.rotation[i], new Vector2(20f, 50f), (1.5f + (float)i * 0.075f) * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
