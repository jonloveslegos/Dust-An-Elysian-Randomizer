using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FuseSeekBall : Particle
	{
		private Vector2 orig;

		protected int targ;

		private int animFrame;

		private float animFrameTime;

		private float frame;

		private float a;

		private float size;

		private float rotation;

		private Rectangle renderRect;

		private Vector2 ploc = Vector2.Zero;

		public FuseSeekBall(Vector2 loc, Vector2 traj, int owner, int spun)
		{
			this.Reset(loc, traj, owner, spun);
		}

		public void Reset(Vector2 loc, Vector2 traj, int owner, int spun)
		{
			base.exists = Exists.Init;
			this.orig = new Vector2(50f, 160f);
			this.targ = -1;
			this.animFrame = Rand.GetRandomInt(0, 24);
			this.animFrameTime = 0f;
			this.a = 1f;
			base.isSpun = spun;
			base.trajectory.X = traj.X * Rand.GetRandomFloat(0.4f, 0.9f) + Game1.character[owner].Trajectory.X;
			base.trajectory.Y = Rand.GetRandomFloat(-600f, -100f);
			base.location = loc;
			base.owner = owner;
			this.size = Rand.GetRandomFloat(0.6f, 1f);
			base.renderState = RenderState.Refract;
			base.strength = Game1.character[owner].Strength / 4;
			this.frame = Rand.GetRandomFloat(8f, 10f);
			this.renderRect = new Rectangle(-2000, -5000, Game1.screenWidth + 4000, Game1.screenHeight + 10000);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			if (base.isSpun < 2f)
			{
				if (this.a == 1f && HitManager.CheckHit(this, c, pMan))
				{
					base.Reset();
					Sound.PlayCue("enemy_die_flame", base.location, (c[0].Location - base.location).Length());
					pMan.AddExplosion(base.location, 1f, makeSmoke: false, 6);
				}
			}
			else
			{
				base.Reset();
				Sound.PlayCue("enemy_die_flame");
				VibrationManager.SetScreenShake(1f);
				pMan.AddExplosion(base.location, 1f, makeSmoke: false, 6);
			}
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || this.frame < 0f)
			{
				base.Reset();
			}
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f;
			base.location += base.trajectory * gameTime;
			this.animFrameTime += gameTime * 96f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 22)
				{
					this.animFrame = 0;
					Sound.PlayCue("fuse_seek_beep", base.location, (Math.Abs(base.location.X - Game1.character[0].Location.X) + Math.Abs(base.location.Y - Game1.character[0].Location.Y)) / 2f);
				}
				this.animFrameTime = 0f;
			}
			pMan.Seek(c, this, this.targ, 4f, 4000f);
			this.ploc = base.location;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			int x = 540 + 100 * this.animFrame;
			if (this.frame < 1f)
			{
				this.a = this.frame;
			}
			if (Game1.refractive)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(x, 1660, 100, 199), new Color(1f, 1f, 1f, MathHelper.Clamp(this.a, 0f, 0.3f)), this.rotation, this.orig, this.size * 1.2f * worldScale, SpriteEffects.None, 1f);
			}
			else
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(x, 1660, 100, 199), new Color((float)this.animFrame / 24f, 1f, 1f, this.a), this.rotation, this.orig, this.size * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
