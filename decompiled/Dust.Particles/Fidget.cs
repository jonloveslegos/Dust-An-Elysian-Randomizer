using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Fidget : Particle
	{
		public enum FidgetState
		{
			Idle00,
			Idle01,
			Idle02,
			Idle03,
			Turning,
			Throwing,
			Scared,
			Landing
		}

		private enum Facing
		{
			Left,
			Right
		}

		private FidgetState state;

		private Facing face;

		private Facing prevFace;

		private float offSet;

		private float throwOffset;

		private float frame;

		private float wingFrame;

		private float legsFrame;

		private float rotation;

		private float tailRotation;

		private float tailDirection;

		private float tailFrame;

		private float holdFrame;

		private float yellTime;

		private int animFrame;

		private byte animFrameWing;

		private byte animFrameTail;

		private byte animFrameLegs;

		private int srectOffsetX = 1344;

		private int srectOffsetY;

		private int srectOffsetTailX;

		private int srectOffsetTailY;

		private int srectOffsetLegsX;

		private int srectOffsetLegsY;

		private Rectangle sRect = new Rectangle(0, 0, 64, 64);

		private Rectangle sRectWing1 = new Rectangle(0, 0, 64, 128);

		private Rectangle sRectWing2 = new Rectangle(512, 0, 64, 128);

		private Rectangle sRectTail = new Rectangle(0, 128, 64, 64);

		private Rectangle sRectLegs = new Rectangle(0, 256, 64, 64);

		private Rectangle sRectTurn = new Rectangle(2112, 0, 64, 128);

		public Fidget(Vector2 loc)
		{
			base.exists = Exists.Init;
			base.location = loc;
			base.background = true;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.yellTime < 10f)
			{
				this.yellTime += gameTime;
			}
			base.maskGlow = 0.1f;
			if (Game1.stats.fidgetState != 0)
			{
				base.trajectory = c[0].Location + new Vector2(6000f, -2000f);
				base.location += (base.trajectory - base.location) * gameTime / 4f;
				if ((base.location - c[0].Location).Length() > 2000f)
				{
					base.Reset();
					return;
				}
			}
			else
			{
				float num = (float)Math.Cos((double)map.MapSegFrame * 2.0) * 50f;
				float num2 = (float)Math.Sin((double)map.MapSegFrame * 10.0) * 50f;
				float num3 = (((c[0].Face != CharDir.Right || Game1.events.eventType != 0) && (c[0].Face != 0 || Game1.events.eventType == EventType.None)) ? 150f : (-150f));
				if (this.offSet != num3)
				{
					if (this.offSet > num3)
					{
						this.offSet = Math.Max(this.offSet - gameTime * 400f, num3);
					}
					else if (this.offSet < num3)
					{
						this.offSet = Math.Min(this.offSet + gameTime * 400f, num3);
					}
				}
				if (this.throwOffset != 0f)
				{
					if (this.throwOffset > 0f)
					{
						this.throwOffset = Math.Max(this.throwOffset - gameTime * 800f, 0f);
					}
					else if (this.throwOffset < 0f)
					{
						this.throwOffset = Math.Min(this.throwOffset + gameTime * 800f, 0f);
					}
				}
				_ = base.trajectory.X;
				if (base.trajectory.Y > 0f)
				{
					base.trajectory = new Vector2((c[0].Location.X - base.location.X + this.offSet + this.throwOffset + num) * 30f, (c[0].Location.Y - base.location.Y - 300f + num2) * 30f);
				}
				else
				{
					base.trajectory = new Vector2((c[0].Location.X - base.location.X + this.offSet + this.throwOffset + num) * 30f, (c[0].Location.Y - base.location.Y - 300f + num2) * 10f);
				}
				this.UpdateRegions(map, gameTime);
				base.location += base.trajectory * gameTime / 12f;
			}
			if (this.state != FidgetState.Throwing)
			{
				if (base.trajectory.X > 0f)
				{
					this.face = Facing.Right;
				}
				else
				{
					this.face = Facing.Left;
				}
			}
			this.rotation = MathHelper.Clamp(base.trajectory.X / 12000f, -0.75f, 0.75f);
			if (c[0].KeyThrow && Game1.stats.canThrow && Game1.stats.playerLifeState == 0 && c[0].StatusEffect != StatusEffects.Silent)
			{
				Game1.stats.curCharge -= Game1.stats.projectileCost;
				this.InitThrowAnim(c, pMan, throwProjectile: true);
			}
			if (this.state == FidgetState.Throwing)
			{
				this.frame += gameTime * 10f;
				if (this.frame > 1f)
				{
					while (this.frame > 1f)
					{
						this.frame -= 1f;
					}
					this.animFrame++;
					if (this.animFrame > 5)
					{
						this.holdFrame = Rand.GetRandomFloat(4f, 8f);
						this.animFrame = 0;
						this.srectOffsetX = 1600;
						this.srectOffsetY = 0;
						this.state = FidgetState.Idle03;
					}
				}
			}
			else if (this.state != FidgetState.Turning)
			{
				this.frame += gameTime * 12f;
				if (this.holdFrame > 0f)
				{
					this.holdFrame -= gameTime;
				}
				if (this.frame > 1f)
				{
					while (this.frame > 1f)
					{
						this.frame -= 1f;
					}
					if (this.holdFrame <= 0f)
					{
						if (this.state == FidgetState.Idle00)
						{
							this.animFrame--;
							if (this.animFrame < -5)
							{
								this.holdFrame = Rand.GetRandomFloat(0f, 2f);
								this.animFrame = 0;
								this.srectOffsetY = 0;
								if (Rand.GetRandomInt(0, 10) < 4)
								{
									this.state = FidgetState.Idle01;
									this.srectOffsetX = 1024;
								}
								else
								{
									this.state = FidgetState.Idle02;
									this.srectOffsetX = 1024;
								}
							}
						}
						else if (this.state == FidgetState.Idle01)
						{
							this.animFrame++;
							if (this.animFrame > 5)
							{
								this.holdFrame = Rand.GetRandomFloat(2f, 5f);
								this.animFrame = 0;
								this.srectOffsetX = 1344;
								this.srectOffsetY = 0;
								this.state = FidgetState.Idle00;
							}
						}
						else if (this.state == FidgetState.Idle02)
						{
							this.srectOffsetX = 1408;
							this.animFrame++;
							if (this.animFrame > 3)
							{
								this.holdFrame = Rand.GetRandomFloat(4f, 8f);
								this.animFrame = 0;
								this.srectOffsetX = 1600;
								this.srectOffsetY = 0;
								this.state = FidgetState.Idle03;
							}
						}
						else if (this.state == FidgetState.Idle03)
						{
							this.animFrame--;
							if (this.animFrame < -3)
							{
								this.holdFrame = Rand.GetRandomFloat(2f, 5f);
								this.animFrame = 0;
								this.srectOffsetY = 0;
								if (Rand.GetRandomInt(0, 10) < 4)
								{
									this.state = FidgetState.Idle01;
									this.srectOffsetX = 1024;
								}
								else
								{
									this.state = FidgetState.Idle02;
									this.srectOffsetX = 1024;
								}
							}
						}
					}
				}
			}
			if (Math.Abs(base.trajectory.X) / 3000f > 1f)
			{
				this.wingFrame += gameTime * 20f * (Math.Abs(base.trajectory.X) / 3000f);
			}
			else
			{
				this.wingFrame += gameTime * 20f;
			}
			if (this.wingFrame > 1f)
			{
				this.animFrameWing++;
				if (this.animFrameWing > 7)
				{
					this.animFrameWing = 0;
				}
				while (this.wingFrame > 1f)
				{
					this.wingFrame -= 1f;
				}
			}
			this.legsFrame += gameTime * 24f;
			if (this.legsFrame > 1f)
			{
				this.animFrameLegs++;
				if (this.animFrameLegs > 78)
				{
					this.animFrameLegs = 0;
				}
				if (this.animFrameLegs > 39)
				{
					this.srectOffsetLegsX = 2560;
					this.srectOffsetLegsY = 64;
				}
				else
				{
					this.srectOffsetLegsX = 0;
					this.srectOffsetLegsY = 0;
				}
				while (this.legsFrame > 1f)
				{
					this.legsFrame -= 1f;
				}
			}
			if (base.trajectory.X > 0f)
			{
				this.tailDirection += gameTime;
				if (this.tailDirection > 0.5f)
				{
					this.tailDirection = 0.5f;
				}
			}
			else
			{
				this.tailDirection -= gameTime;
				if (this.tailDirection < -0.5f)
				{
					this.tailDirection = -0.5f;
				}
			}
			this.tailRotation = MathHelper.Clamp(base.trajectory.X / 6000f + this.tailDirection, -2f, 2f);
			if (Math.Abs(base.trajectory.X) / 100f > 1f)
			{
				this.tailFrame += gameTime * 20f * (Math.Abs(base.trajectory.X) / 100f);
			}
			else
			{
				this.tailFrame += gameTime * 20f;
			}
			if (this.tailFrame > 1f)
			{
				this.animFrameTail++;
				if (this.animFrameTail > 78)
				{
					this.animFrameTail = 0;
				}
				if (this.animFrameTail > 39)
				{
					this.srectOffsetTailX = 2560;
					this.srectOffsetTailY = 64;
				}
				else
				{
					this.srectOffsetTailX = 0;
					this.srectOffsetTailY = 0;
				}
				while (this.tailFrame > 1f)
				{
					this.tailFrame -= 1f;
				}
			}
			if (this.prevFace != this.face && this.state != FidgetState.Throwing)
			{
				this.state = FidgetState.Turning;
				this.frame = 0f;
				this.animFrame = 0;
			}
			if (this.state == FidgetState.Turning)
			{
				this.frame += gameTime * 12f;
				if (this.frame > 1f)
				{
					this.animFrame++;
					if (this.animFrame > 6)
					{
						this.animFrame = 0;
						this.animFrameLegs = 74;
						this.srectOffsetLegsX = 2560;
						this.srectOffsetLegsY = 64;
						this.legsFrame = 0f;
						this.srectOffsetX = 1600;
						this.srectOffsetY = 0;
						this.state = FidgetState.Idle03;
					}
					while (this.frame > 1f)
					{
						this.frame -= 1f;
					}
				}
			}
			this.prevFace = this.face;
			this.sRect.X = this.animFrame * 64 + this.srectOffsetX;
			this.sRect.Y = this.srectOffsetY;
			this.sRectWing1.X = this.animFrameWing * 64;
			this.sRectWing2.X = 512 + this.animFrameWing * 64;
			this.sRectTail.X = this.animFrameTail * 64 - this.srectOffsetTailX;
			this.sRectTail.Y = 128 + this.srectOffsetTailY;
			this.sRectLegs.X = this.animFrameLegs * 64 - this.srectOffsetLegsX;
			this.sRectLegs.Y = 256 + this.srectOffsetLegsY;
			this.sRectTurn.X = 2112 + this.animFrame * 64;
		}

		private void UpdateRegions(Map map, float gameTime)
		{
			for (int i = 0; i < map.boostRegions.Count; i++)
			{
				if (map.boostRegions[i] != null && base.location.X > (float)map.boostRegions[i].Region.X && base.location.X < (float)(map.boostRegions[i].Region.X + map.boostRegions[i].Region.Width) && base.location.Y > (float)map.boostRegions[i].Region.Y && base.location.Y < (float)(map.boostRegions[i].Region.Y + map.boostRegions[i].Region.Height))
				{
					switch (map.boostRegions[i].Direction)
					{
					case 0:
					{
						float num4 = MathHelper.Clamp((float)map.boostRegions[i].Region.Height - Math.Abs(base.location.Y - (float)(map.boostRegions[i].Region.Y + map.boostRegions[i].Region.Height)), 10f, 500f);
						base.location.Y -= num4 * 4f * gameTime;
						break;
					}
					case 3:
					{
						float num3 = ((float)map.boostRegions[i].Region.Width - (base.location.X - (float)map.boostRegions[i].Region.X)) / (float)map.boostRegions[i].Region.Width * 1000f;
						base.location.X += num3 * 10f * gameTime;
						break;
					}
					case 6:
					{
						float num2 = ((float)map.boostRegions[i].Region.Height - (base.location.Y - (float)map.boostRegions[i].Region.Y)) / (float)map.boostRegions[i].Region.Height * 1000f;
						base.location.Y += num2 * 4f * gameTime;
						break;
					}
					case 9:
					{
						float num = (base.location.X - (float)map.boostRegions[i].Region.X) / (float)map.boostRegions[i].Region.Width * 1000f;
						base.location.X -= num * 10f * gameTime;
						break;
					}
					}
				}
			}
		}

		public override bool InitAction(int type)
		{
			this.InitThrowAnim(Game1.character, Game1.pManager, type == 1);
			return true;
		}

		private void InitThrowAnim(Character[] c, ParticleManager pMan, bool throwProjectile)
		{
			this.state = FidgetState.Throwing;
			Vector2 vector;
			if (c[0].Face == CharDir.Right)
			{
				this.throwOffset = Math.Max(this.throwOffset - 200f, -300f);
				this.face = Facing.Right;
				vector = new Vector2(50f, -20f);
			}
			else
			{
				this.throwOffset = Math.Min(this.throwOffset + 200f, 300f);
				this.face = Facing.Left;
				vector = new Vector2(-50f, -50f);
			}
			if (throwProjectile)
			{
				pMan.MakeProjectile(base.location + vector, new Vector2(1500f, 0f), 0f, 1f, CharacterType.Dust, c[0].Face, 0, 5);
				if (Rand.GetRandomInt(0, 10) == 0 || this.yellTime > 10f)
				{
					Sound.PlayCue("fidget_yell", base.location, (c[0].Location - base.location).Length() / 2f);
					this.yellTime = 0f;
				}
			}
			this.srectOffsetX = 1664;
			this.srectOffsetY = 0;
			this.frame = 0f;
			this.animFrame = 0;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l);
			sprite.Draw(particlesTex[0], vector + new Vector2(0f, 4f * worldScale), this.sRectTail, Color.White, this.tailRotation, new Vector2(32f, 6f), worldScale, SpriteEffects.None, 1f);
			if (this.face == Facing.Right)
			{
				Vector2 vector2 = new Vector2((float)(Math.Cos(this.rotation / 2f) * 12.0 * (double)worldScale), (float)(Math.Sin(this.rotation / 2f) * 12.0 * (double)worldScale));
				if (this.state != FidgetState.Turning)
				{
					sprite.Draw(particlesTex[0], vector + vector2 + new Vector2(-16f * worldScale, 0f), this.sRectWing2, Color.White, this.rotation / 2f, new Vector2(0f, 64f), worldScale, SpriteEffects.None, 1f);
					sprite.Draw(particlesTex[0], vector, this.sRectLegs, Color.White, this.rotation * 1.4f, new Vector2(32f, 16f), new Vector2(1f, 1.2f) * worldScale, SpriteEffects.None, 1f);
					sprite.Draw(particlesTex[0], vector + vector2, this.sRectWing1, Color.White, this.rotation / 2f, new Vector2(64f, 64f), worldScale, SpriteEffects.None, 1f);
					sprite.Draw(particlesTex[0], vector, this.sRect, Color.White, this.rotation / 2f, new Vector2(32f, 54f), worldScale, SpriteEffects.None, 1f);
				}
				else
				{
					sprite.Draw(particlesTex[0], vector + vector2 + new Vector2(-16f * worldScale, 0f), this.sRectWing2, Color.White, this.rotation / 2f, new Vector2(0f, 64f), worldScale, SpriteEffects.None, 1f);
					sprite.Draw(particlesTex[0], vector + vector2, this.sRectWing1, Color.White, this.rotation / 2f, new Vector2(64f, 64f), worldScale, SpriteEffects.None, 1f);
					sprite.Draw(particlesTex[0], vector, this.sRectTurn, Color.White, this.rotation / 2f, new Vector2(32f, 54f), worldScale, SpriteEffects.FlipHorizontally, 1f);
				}
			}
			else
			{
				Vector2 vector3 = new Vector2((float)(Math.Cos(this.rotation / 2f) * -12.0 * (double)worldScale), (float)(Math.Sin(this.rotation / 2f) * -12.0 * (double)worldScale));
				if (this.state != FidgetState.Turning)
				{
					sprite.Draw(particlesTex[0], vector + vector3 + new Vector2(16f * worldScale, 0f), this.sRectWing2, Color.White, this.rotation / 2f, new Vector2(64f, 64f), worldScale, SpriteEffects.FlipHorizontally, 1f);
					sprite.Draw(particlesTex[0], vector, this.sRectLegs, Color.White, this.rotation * 1.4f, new Vector2(32f, 16f), new Vector2(1f, 1.2f) * worldScale, SpriteEffects.FlipHorizontally, 1f);
					sprite.Draw(particlesTex[0], vector + vector3, this.sRectWing1, Color.White, this.rotation / 2f, new Vector2(0f, 64f), worldScale, SpriteEffects.FlipHorizontally, 1f);
					sprite.Draw(particlesTex[0], vector, this.sRect, Color.White, this.rotation / 2f, new Vector2(32f, 54f), worldScale, SpriteEffects.FlipHorizontally, 1f);
				}
				else
				{
					sprite.Draw(particlesTex[0], vector + vector3 + new Vector2(16f * worldScale, 0f), this.sRectWing2, Color.White, this.rotation / 2f, new Vector2(64f, 64f), worldScale, SpriteEffects.FlipHorizontally, 1f);
					sprite.Draw(particlesTex[0], vector + vector3, this.sRectWing1, Color.White, this.rotation / 2f, new Vector2(0f, 64f), worldScale, SpriteEffects.FlipHorizontally, 1f);
					sprite.Draw(particlesTex[0], vector, this.sRectTurn, Color.White, this.rotation / 2f, new Vector2(32f, 54f), worldScale, SpriteEffects.None, 1f);
				}
			}
		}
	}
}
