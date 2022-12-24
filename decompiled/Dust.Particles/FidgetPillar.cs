using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FidgetPillar : Particle
	{
		private Vector2 orig;

		private int audioID;

		private byte stage;

		private byte animFrame;

		private float animFrameTime;

		private float length;

		private float trailTime;

		private float frame;

		private float a;

		private float size;

		private float rotation;

		private Rectangle sRect;

		private Rectangle dRect;

		private Vector2 ploc = Vector2.Zero;

		public FidgetPillar(Vector2 loc, Vector2 traj, int owner, int spun, int i, int l)
		{
			this.Reset(loc, traj, owner, spun, i, l);
		}

		public void Reset(Vector2 loc, Vector2 traj, int owner, int spun, int i, int l)
		{
			base.exists = Exists.Init;
			this.orig = new Vector2(50f, 160f);
			this.animFrame = (byte)Rand.GetRandomInt(0, 24);
			this.animFrameTime = 0f;
			this.length = 0f;
			this.trailTime = Rand.GetRandomFloat(0f, 0.15f);
			this.audioID = i;
			base.strength = 10;
			base.isSpun = spun;
			base.maskGlow = 0.05f;
			base.trajectory.X = traj.X * Rand.GetRandomFloat(0.4f, 0.9f) + Game1.character[owner].Trajectory.X;
			base.trajectory.Y = Rand.GetRandomFloat(-600f, -100f);
			base.location = loc;
			base.owner = owner;
			base.background = false;
			this.size = Rand.GetRandomFloat(0.6f, 1f);
			base.renderState = RenderState.Refract;
			this.frame = 3f;
			this.a = 1f;
			this.sRect = new Rectangle(540, 1660, 100, 199);
			this.stage = 0;
			if (loc != Vector2.Zero)
			{
				Sound.PlayFollowParticleCue("fidgetproj_02", i, l);
			}
			base.exists = Exists.Exists;
		}

		public override bool InitAction(int actionType)
		{
			if (actionType == 0 && this.stage == 0 && base.exists == Exists.Exists)
			{
				Sound.PlayCue("fidgetproj_01_contact", base.location, (Game1.character[0].Location - base.location).Length() * Game1.worldScale);
				Game1.pManager.AddExplosion(base.location, 1f, makeSmoke: false, 6);
				base.Reset();
				return true;
			}
			return false;
		}

		public override void GetInfo(ref int intVar, ref float floatVar)
		{
			intVar = this.stage;
			floatVar = this.frame;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			if (this.stage == 0)
			{
				Vector2 vector = base.location * Game1.worldScale - Game1.Scroll;
				if (!new Rectangle(-1000, -1000, Game1.screenWidth + 2000, Game1.screenHeight + 1600).Contains((int)vector.X, (int)vector.Y))
				{
					base.Reset();
				}
				pMan.Seek(c, this, -1, 4f, 4000f);
				if (base.isSpun > 0f && base.trajectory.Y < 1500f)
				{
					base.trajectory.Y += Game1.FrameTime * 2000f;
				}
				if (base.isSpun == 0f)
				{
					base.trajectory.Y += Rand.GetRandomFloat(-80f, 90f);
					this.trailTime += Game1.FrameTime;
					if (HitManager.CheckHit(this, c, pMan) || this.frame < 0f)
					{
						Sound.PlayCue("fidgetproj_01_contact", base.location, (c[0].Location - base.location).Length() * Game1.worldScale);
						pMan.AddExplosion(base.location, 1f, makeSmoke: false, 6);
						base.Reset();
					}
					if (this.trailTime > 0.15f)
					{
						this.trailTime = 0f;
						pMan.AddSparkle(base.location, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.75f, Rand.GetRandomFloat(0.1f, 0.5f), Rand.GetRandomInt(40, 80), 6);
					}
				}
				else if (base.isSpun > 0.75f)
				{
					VibrationManager.SetScreenShake(0.5f);
					pMan.AddExplosion(base.location, 1f, makeSmoke: false, 6);
					if (map.shockRingCount < 2)
					{
						map.shockRingCount++;
						pMan.AddShockRing(base.location, 1f, l);
					}
					if (map.projectileCount < 12)
					{
						map.projectileCount++;
						this.stage = 1;
						base.maskGlow = 0f;
						base.location = new Vector2(MathHelper.Clamp(pMan.TargetLocation(c, this, 1000f).X + Rand.GetRandomFloat(-200f, 200f), c[base.owner].Location.X - 700f, c[base.owner].Location.X + 700f), c[base.owner].Location.Y);
						base.trajectory.X = Rand.GetRandomFloat(-300f, 300f);
						base.trajectory.Y = 0f;
						this.a = 1f;
						this.size = Rand.GetRandomFloat(1.2f, 1.6f);
						this.frame = 0.5f;
						base.renderState = RenderState.Refract;
						this.sRect = new Rectangle(3960, 1448, 136, 112);
						Sound.PlayFollowParticleCue("fidgetproj_02_pillar_beam", this.audioID, l);
					}
					else
					{
						base.Reset();
					}
				}
			}
			else if (this.stage == 1)
			{
				Vector2 vector2 = base.GameLocation(l);
				this.dRect = new Rectangle((int)vector2.X, (int)vector2.Y, (int)(400f * this.frame), 8000);
				if (this.frame < 0f)
				{
					this.frame = Rand.GetRandomFloat(1f, 2.5f);
					this.stage = 2;
					this.a = 1f;
					this.sRect = new Rectangle(540, 1861, 100, 148);
					this.dRect.Width = 0;
					this.dRect.Height = 10000;
					Game1.worldDark = MathHelper.Max(Game1.worldDark - 0.1f, 0f);
					Sound.PlayFollowParticleCue("fidgetproj_02_pillar", this.audioID, l);
					VibrationManager.SetScreenShake(1f);
					if (Rand.GetRandomInt(0, 2) == 0)
					{
						base.background = true;
						base.renderState = RenderState.Normal;
					}
				}
			}
			else if (this.stage == 2)
			{
				if (this.dRect.Width < 400 && this.frame > 0.5f)
				{
					this.dRect.Width += (int)(Game1.FrameTime * 3000f);
				}
				else
				{
					this.dRect.Width = (int)(400f * MathHelper.Clamp(this.frame * 10f, 0f, 1f));
				}
				this.dRect.X = (int)(base.location.X - Game1.Scroll.X);
				this.dRect.Y = (int)(base.location.Y - Game1.Scroll.Y);
				if (this.frame > 0.25f)
				{
					for (int i = 1; i < c.Length; i++)
					{
						if (HitManager.CheckHitPossible(this, c, i, new Rectangle((int)((float)this.dRect.X + Game1.Scroll.X - (float)(this.dRect.Width / 2)), (int)((float)this.dRect.Y + Game1.Scroll.Y) - 4000, this.dRect.Width, this.dRect.Height)) && HitManager.CheckIDHit(this, c, pMan, i))
						{
							Sound.PlayCue("enemy_die_flame", base.location, (c[0].Location - base.location).Length());
							for (int j = 0; j < 6; j++)
							{
								pMan.AddExplosion(c[i].Location + Rand.GetRandomVector2(-100f, 100f, -c[i].Height, 0f), Rand.GetRandomFloat(0.5f, 1.5f), makeSmoke: false, 6);
							}
						}
					}
				}
				if (this.frame < 0f)
				{
					map.projectileCount--;
					base.Reset();
				}
			}
			this.length = this.size * ((Math.Abs(base.trajectory.X) + Math.Abs(base.trajectory.Y)) / 2000f) * (Game1.FrameTime * 60f);
			if (this.length < 0.75f)
			{
				this.length = 0.75f;
			}
			base.location += base.trajectory * gameTime;
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f;
			this.animFrameTime += gameTime * 96f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 22)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			this.ploc = base.location;
		}

		public override void GetAudio(ref Vector2 loc, ref Vector2 traj, ref bool _exists)
		{
			loc = new Vector2((base.location.X + Game1.character[0].Location.X) / 2f, Game1.character[0].Location.Y);
			_exists = base.exists != Exists.Dead;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.frame < 1f)
			{
				this.a = this.frame;
			}
			if (this.stage == 0)
			{
				this.sRect.X = 540 + 100 * this.animFrame;
				if (Game1.refractive)
				{
					sprite.Draw(particlesTex[2], base.location * worldScale - Game1.Scroll, this.sRect, new Color(1f, 1f, 1f, MathHelper.Clamp(this.a, 0f, 0.2f)), this.rotation, this.orig, new Vector2(this.size, this.length) * 2f * worldScale, SpriteEffects.None, 1f);
				}
				else
				{
					sprite.Draw(particlesTex[2], base.location * worldScale - Game1.Scroll, this.sRect, new Color(1f, 1f, 1f, this.a), this.rotation, this.orig, new Vector2(this.size, this.length) * worldScale, SpriteEffects.None, 1f);
				}
			}
			else if (this.stage == 1)
			{
				sprite.Draw(particlesTex[2], this.dRect, this.sRect, new Color(1f, 1f, 1f, this.a * 3f), 0f, new Vector2(68f, 61f), SpriteEffects.None, 1f);
			}
			else
			{
				if (this.stage != 2)
				{
					return;
				}
				if (base.location.Y > Game1.Scroll.Y - 100f)
				{
					base.location.Y -= 592f;
				}
				else if (base.location.Y < Game1.Scroll.Y - 592f)
				{
					base.location.Y += 592f;
				}
				this.sRect.X = 540 + 100 * this.animFrame;
				float max = 1f;
				if (!base.background)
				{
					max = 0.8f;
				}
				float num = (int)base.GameLocation(l).X;
				for (int i = 0; i < 3; i++)
				{
					if (Game1.refractive)
					{
						sprite.Draw(particlesTex[2], new Vector2(num, this.dRect.Y + i * 592), this.sRect, new Color(1f, 1f, 1f, this.a * 4f), 0f, new Vector2(50f, 0f), new Vector2((float)this.dRect.Width / 60f * worldScale, 4f), SpriteEffects.FlipHorizontally, 1f);
					}
					if (!Game1.refractive)
					{
						sprite.Draw(particlesTex[2], new Vector2(num + Rand.GetRandomFloat(-2f, 2f), this.dRect.Y + i * 592), this.sRect, new Color(0.9f, 0.9f, 0.9f, MathHelper.Clamp(this.a * 8f, 0f, max)), 0f, new Vector2(50f, 0f), new Vector2((float)this.dRect.Width / 100f * worldScale, 4f), SpriteEffects.None, 1f);
					}
				}
			}
		}
	}
}
