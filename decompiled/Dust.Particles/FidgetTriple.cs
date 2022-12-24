using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FidgetTriple : Particle
	{
		private SpriteEffects spriteDir;

		private byte animFrame;

		private float animFrameTime;

		private float length;

		private float trailTime;

		private float frame;

		private float size;

		private Vector2 ploc = Vector2.Zero;

		public FidgetTriple(Vector2 loc, Vector2 traj, int owner, int spun, int audioID, int l)
		{
			this.Reset(loc, traj, owner, spun, audioID, l);
		}

		public void Reset(Vector2 loc, Vector2 traj, int owner, int spun, int audioID, int l)
		{
			base.exists = Exists.Init;
			this.animFrameTime = 0f;
			base.isSpun = spun;
			base.maskGlow = 0.05f;
			base.strength = 10;
			base.flag = Rand.GetRandomInt(1, 5);
			if (base.isSpun < 10f)
			{
				base.trajectory.X = traj.X * Rand.GetRandomFloat(0.4f, 0.9f) + Game1.character[owner].Trajectory.X;
				base.trajectory.Y = Rand.GetRandomFloat(-600f, -100f);
				this.animFrame = (byte)Rand.GetRandomInt(0, 13);
				this.size = Rand.GetRandomFloat(0.7f, 1.2f);
				this.frame = Rand.GetRandomFloat(3.9f, 4.2f);
				if (Rand.GetRandomInt(0, 2) == 0)
				{
					this.spriteDir = SpriteEffects.FlipHorizontally;
				}
				else
				{
					this.spriteDir = SpriteEffects.None;
				}
				this.trailTime = Rand.GetRandomFloat(0f, 0.15f);
				Sound.PlayFollowParticleCue("fidgetproj_01", audioID, l);
			}
			else
			{
				this.animFrame = 0;
				this.frame = 2f;
				this.size = 1f;
				this.frame = 4f;
				base.trajectory = traj;
			}
			base.location = loc;
			base.owner = owner;
			base.renderState = RenderState.AdditiveOnly;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			pMan.Seek(c, this, -1, 70f, 1000f);
			if (base.isSpun == 0f || base.isSpun == 10f)
			{
				if (base.flag == Game1.longSkipFrame && HitManager.CheckHit(this, c, pMan))
				{
					this.KillProjectile(map, pMan);
					Sound.PlayCue("fidgetproj_01_contact", base.location, (c[0].Location - base.location).Length() * Game1.worldScale);
					if ((map.projectileCount < 20 || Rand.GetRandomInt(0, 10) == 0) && Rand.GetRandomInt(0, 2) == 0)
					{
						pMan.AddFidgetPuff(base.location, 6);
					}
				}
			}
			else if (base.isSpun > 0.75f)
			{
				if (!Game1.events.anyEvent)
				{
					Game1.awardsManager.EarnAchievement(Achievement.DustStorm, forceCheck: false);
				}
				this.KillProjectile(map, pMan);
				Sound.PlayCue("dust_storm_01");
				VibrationManager.SetScreenShake(1f);
				if (map.shockRingCount < 2)
				{
					map.shockRingCount++;
					pMan.AddShockRing(base.location, 1f, l);
				}
				for (int i = 0; i < 16; i++)
				{
					if (map.projectileCount < 140)
					{
						map.projectileCount++;
						pMan.AddFidgetTriple(base.location, new Vector2(Rand.GetRandomFloat(-1000f, 1000f), Rand.GetRandomFloat(-1000f, 1000f)), base.owner, 10, l);
					}
				}
				pMan.AddFidgetPuff(base.location, 6);
				return;
			}
			this.frame -= gameTime;
			if (this.frame < 0f)
			{
				this.KillProjectile(map, pMan);
				Sound.PlayCue("fidgetproj_01_contact", base.location, (c[0].Location - base.location).Length() * Game1.worldScale);
				pMan.AddFidgetPuff(base.location, 6);
			}
			this.length = this.size * ((Math.Abs(base.trajectory.X) + Math.Abs(base.trajectory.Y)) / 2000f) * (Game1.FrameTime * 60f);
			if (this.length < 0.5f)
			{
				this.length = 0.5f;
			}
			if (base.isSpun < 10f)
			{
				Vector2 vector = base.location * Game1.worldScale - Game1.Scroll;
				if (!new Rectangle(-1000, -1000, Game1.screenWidth + 2000, Game1.screenHeight + 1600).Contains((int)vector.X, (int)vector.Y))
				{
					this.KillProjectile(map, pMan);
				}
				if (base.trajectory.Y < 1500f)
				{
					base.trajectory.Y += Game1.FrameTime * 2000f;
				}
				if (base.isSpun == 0f)
				{
					this.trailTime += Game1.FrameTime;
					if (this.trailTime > 0.15f)
					{
						this.trailTime = 0f;
						pMan.AddSparkle(base.location, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.75f, Rand.GetRandomFloat(0.1f, 0.5f), Rand.GetRandomInt(40, 80), 6);
					}
				}
			}
			base.location += base.trajectory * gameTime;
			this.animFrameTime += gameTime * 24f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 13)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			this.ploc = base.location;
		}

		private void KillProjectile(Map map, ParticleManager pMan)
		{
			if (map.projectileCount > 0)
			{
				map.projectileCount--;
			}
			base.Reset();
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.location * worldScale - Game1.Scroll, new Rectangle(50 * this.animFrame, 1240, 50, 140), Color.White, GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f, new Vector2(25f, 70f), new Vector2(this.size, this.length) * worldScale, this.spriteDir, 1f);
			if (Rand.GetRandomInt(0, 10) == 0)
			{
				sprite.Draw(particlesTex[2], base.location * worldScale - Game1.Scroll, new Rectangle(2940, 1860, 497, 150), Color.White * this.frame * 4f, 0f, new Vector2(248f, 75f), 0.75f * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
