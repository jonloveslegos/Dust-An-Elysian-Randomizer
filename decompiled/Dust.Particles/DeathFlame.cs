using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class DeathFlame : Particle
	{
		private int sRectOffsetX;

		private byte frameRate;

		private byte animFrame;

		private float animFrameTime;

		private float r;

		private float g;

		private float b;

		private float a;

		private float size;

		private Rectangle renderRect;

		public DeathFlame(Vector2 loc, Vector2 traj, float r, float g, float b, float size, int id, bool audio)
		{
			this.Reset(loc, traj, r, g, b, size, id, audio);
		}

		public void Reset(Vector2 loc, Vector2 traj, float r, float g, float b, float size, int id, bool audio)
		{
			base.exists = Exists.Init;
			this.animFrame = 0;
			this.sRectOffsetX = 1740 + Rand.GetRandomInt(0, 2) * 1024;
			this.frameRate = 0;
			base.location = loc;
			base.trajectory = traj;
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 1f;
			this.size = size;
			base.owner = id;
			this.animFrameTime = 0f;
			if (Rand.GetRandomInt(0, 5) == 0)
			{
				this.frameRate = (byte)Rand.GetRandomInt(12, 36);
				base.renderState = RenderState.RefractOnly;
				if (audio)
				{
					Sound.PlayCue("enemy_die_flame", loc, (Game1.character[0].Location - loc).Length() / 1.5f);
				}
			}
			else
			{
				this.frameRate = (byte)Rand.GetRandomInt(24, 48);
				base.renderState = RenderState.AdditiveOnly;
			}
			this.renderRect = new Rectangle(-40, -40, Game1.screenWidth + 80, Game1.screenHeight + 80);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y))
			{
				base.Reset();
			}
			base.location += base.trajectory * gameTime;
			this.animFrameTime += gameTime * (float)(int)this.frameRate;
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
			int x = this.animFrame * 64 + this.sRectOffsetX;
			if (!Game1.refractive)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(x, 2074, 64, 126), new Color(this.r, this.g, this.b, this.a), 0f, new Vector2(32f, 64f), new Vector2(this.size * 2f, this.size) * worldScale, SpriteEffects.None, 1f);
			}
			else
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(x, 2074, 64, 126), new Color(1f, 1f, 1f, this.a / 10f), 0f, new Vector2(32f, 64f), new Vector2(this.size * 6f, this.size * 4f) * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
