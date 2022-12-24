using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FuseFireBall : Particle
	{
		private SpriteEffects spriteDir;

		private Vector2 orig;

		private byte animFrame;

		private float animFrameTime;

		private float length;

		private float frame;

		private float a;

		private float size;

		private Rectangle renderRect;

		private Vector2 ploc = Vector2.Zero;

		public FuseFireBall(Vector2 loc, Vector2 traj, int owner)
		{
			this.Reset(loc, traj, owner);
		}

		public void Reset(Vector2 loc, Vector2 traj, int owner)
		{
			base.exists = Exists.Init;
			this.orig = new Vector2(50f, 160f);
			this.animFrame = (byte)Rand.GetRandomInt(0, 20);
			this.animFrameTime = 0f;
			this.length = 0f;
			this.a = 1f;
			this.frame = 2f;
			base.trajectory = traj;
			base.location = loc;
			base.owner = owner;
			this.size = Rand.GetRandomFloat(1.2f, 1.6f);
			base.strength = Game1.character[owner].Strength;
			this.frame = Rand.GetRandomFloat(3.9f, 4.2f);
			this.renderRect = new Rectangle(-2000, -2000, Game1.screenWidth + 4000, Game1.screenHeight + 4000);
			if (base.trajectory.X < 0f)
			{
				this.spriteDir = SpriteEffects.FlipVertically;
				this.orig = new Vector2(50f, 40f);
			}
			else
			{
				this.spriteDir = SpriteEffects.None;
				this.orig = new Vector2(50f, 160f);
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			if (this.a == 1f && (HitManager.CheckHit(this, c, pMan) || map.CheckCol(base.location) > 0))
			{
				base.Reset();
				Sound.PlayCue("explosion_01", base.location, (c[0].Location - base.location).Length() / 1.5f);
				pMan.AddExplosion(base.location, 1.5f, makeSmoke: false, 6);
			}
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y))
			{
				base.Reset();
			}
			if (base.trajectory.X > 0f)
			{
				base.trajectory.X += Game1.FrameTime * 2000f;
				if (base.trajectory.X > 1000f)
				{
					base.trajectory.X = 1000f;
				}
			}
			else
			{
				base.trajectory.X -= Game1.FrameTime * 2000f;
				if (base.trajectory.X < -1000f)
				{
					base.trajectory.X = -1000f;
				}
			}
			this.length += gameTime * 2f;
			if (this.length < this.size * 0.6f)
			{
				this.length = this.size * 0.6f;
			}
			else if (this.length > 3f)
			{
				this.length = 3f;
			}
			this.animFrameTime += gameTime * 24f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 23)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			base.location += base.trajectory * gameTime;
			this.ploc = base.location;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.frame < 1f)
			{
				this.a /= 1.09f;
			}
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(540 + 100 * this.animFrame, 1660, 100, 198), new Color(1f, 1f, 1f, this.a), -1.57f, this.orig, new Vector2(this.size, this.length) * worldScale, this.spriteDir, 1f);
		}
	}
}
