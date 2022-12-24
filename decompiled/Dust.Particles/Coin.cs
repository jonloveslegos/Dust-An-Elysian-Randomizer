using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Coin : Particle
	{
		private byte groundOffset;

		private byte animFrame;

		private byte flipSpeed;

		private byte idleSpeed;

		private bool stopped;

		private float animFrameTime;

		private float scaleY;

		private float frame;

		private float size;

		private float rotation;

		private Vector2 origin;

		private Vector2 ploc;

		public Coin(Vector2 loc, Vector2 traj, int flag)
		{
			this.Reset(loc, traj, flag);
		}

		public void Reset(Vector2 loc, Vector2 traj, int _flag)
		{
			base.exists = Exists.Init;
			this.stopped = false;
			this.scaleY = 1f;
			base.location = (this.ploc = loc);
			this.rotation = 0f;
			base.trajectory = traj;
			base.flag = _flag;
			this.groundOffset = (byte)Rand.GetRandomInt(0, 20);
			this.frame = 12f;
			base.background = false;
			if (base.flag == 5)
			{
				this.rotation = Rand.GetRandomFloat(0f, 3.14f);
			}
			else
			{
				this.animFrame = (byte)Rand.GetRandomInt(0, 11);
				this.flipSpeed = (byte)Rand.GetRandomInt(24, 60);
				this.idleSpeed = (byte)Rand.GetRandomInt(20, 48);
				this.animFrameTime = 0f;
				if (base.flag > 1)
				{
					base.renderState = RenderState.Additive;
				}
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			bool flag = true;
			_ = base.location * Game1.worldScale - Game1.Scroll;
			if (pMan.Seek(c, this, 0, 60f, 1000f))
			{
				flag = false;
				this.stopped = false;
				if (base.isSpun < 0.02f)
				{
					base.trajectory.Y = -1000f;
				}
				base.trajectory.X = MathHelper.Clamp(base.trajectory.X, -3000f, 3000f);
				base.trajectory.Y = MathHelper.Clamp(base.trajectory.Y, -3000f, 3000f);
			}
			bool flag2 = false;
			int num = base.flag;
			if (num != 5)
			{
				Vector2 vector = ((base.flag == 1) ? Vector2.Zero : new Vector2(0f, 80f));
				if (HitManager.CheckPickup(base.location + new Vector2(0f, -32f), c))
				{
					flag2 = true;
				}
				if (this.frame <= 0f)
				{
					base.Reset();
				}
				if (!this.stopped)
				{
					if (map.CheckCol(base.location + new Vector2(0f, -10f) + vector) > 0)
					{
						base.location.X = this.ploc.X;
						base.trajectory.X = 0f - base.trajectory.X;
					}
					if (Math.Abs(base.trajectory.X) > 1f)
					{
						base.trajectory.X /= 1.04f;
					}
					else
					{
						base.trajectory.X = 0f;
					}
					if (base.trajectory.Y < 1500f && !this.stopped)
					{
						base.trajectory.Y += gameTime * 2000f;
					}
					if (base.trajectory.Y < 0f && (map.CheckCol(new Vector2(base.location.X, base.location.Y - 32f) + vector) > 0 || base.location.Y < map.topEdge))
					{
						base.trajectory.Y = 0f;
						base.location.Y = this.ploc.Y;
					}
					if (base.trajectory.Y > -200f)
					{
						float num2 = map.CheckPCol(base.location + vector, this.ploc + vector, canFallThrough: false, init: false);
						if (num2 > 0f)
						{
							base.location.Y = num2 - vector.Y;
							if (base.trajectory.Y > 250f)
							{
								if (flag)
								{
									Sound.PlayDropCue("coin_bounce", base.location, base.trajectory.Y);
								}
								base.trajectory.Y = (0f - base.trajectory.Y) / 1.5f;
							}
							else
							{
								base.trajectory.Y = 0f;
								base.isSpun = 0f;
								this.stopped = true;
							}
						}
					}
					this.animFrameTime += gameTime * (float)(int)this.flipSpeed;
					if (this.animFrameTime > 1f)
					{
						this.animFrame++;
						if (this.animFrame > ((base.flag == 1) ? 11 : 23))
						{
							this.animFrame = 0;
						}
						this.animFrameTime = 0f;
					}
				}
				else
				{
					base.trajectory.X = 0f;
					this.animFrameTime += gameTime * (float)(int)this.idleSpeed;
					if (this.animFrameTime > 1f)
					{
						this.animFrame++;
						if (this.animFrame > ((base.flag == 1) ? 23 : 46))
						{
							this.animFrame = 0;
						}
						this.animFrameTime = 0f;
					}
				}
			}
			else
			{
				if (this.frame <= 0f)
				{
					base.Reset();
				}
				if (!this.stopped)
				{
					if (HitManager.CheckPickup(base.location + new Vector2(0f, -64f), c))
					{
						flag2 = true;
					}
					this.size = (this.scaleY = 1f);
					this.origin = new Vector2(32f, 32f);
					if (base.trajectory.X > 0f)
					{
						this.rotation += gameTime * 16f;
					}
					else
					{
						this.rotation -= gameTime * 16f;
					}
					if (map.CheckCol(base.location + new Vector2(0f, -10f)) > 0)
					{
						base.location.X = this.ploc.X;
						base.trajectory.X = 0f - base.trajectory.X;
					}
					if (Math.Abs(base.trajectory.X) > 1f)
					{
						base.trajectory.X /= 1.04f;
					}
					else
					{
						base.trajectory.X = 0f;
					}
					if (base.trajectory.Y < 1500f && !this.stopped)
					{
						base.trajectory.Y += gameTime * 2000f;
					}
					if (base.trajectory.Y < 0f)
					{
						if (map.CheckCol(new Vector2(base.location.X, base.location.Y - 32f)) > 0 || base.location.Y < map.topEdge)
						{
							base.trajectory.Y = 0f;
							base.location.Y = this.ploc.Y;
						}
					}
					else if (base.trajectory.Y > 200f)
					{
						float num3 = map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false);
						if (num3 > 0f)
						{
							if (flag)
							{
								Sound.PlayCue("money_bag_land");
							}
							base.trajectory.Y = 0f;
							base.location.Y = num3;
							base.isSpun = 0f;
							this.size = 1.6f;
							this.scaleY = 0.25f;
							pMan.AddFootStep(base.location, 1f, 1f, 5);
							this.StopBag();
						}
					}
				}
				else
				{
					if (HitManager.CheckPickup(base.location + new Vector2(0f, -32f), c))
					{
						flag2 = true;
					}
					this.StopBag();
				}
			}
			if (flag2)
			{
				Sound.PlayCue("coin_pickup");
				Game1.stats.Gold += base.flag;
				Game1.stats.Score += base.flag;
				pMan.AddSparkle(base.location + new Vector2(0f, -32f), 1f, 1f, 0.5f, 1f, 0.5f, 60, l);
				base.Reset();
			}
			this.ploc = base.location;
			base.location += base.trajectory * gameTime;
			if (!Game1.events.anyEvent && Game1.menu.prompt == promptDialogue.None)
			{
				this.frame -= gameTime;
			}
		}

		private void StopBag()
		{
			this.stopped = true;
			base.trajectory.X = 0f;
			this.rotation = 0f;
			this.origin = new Vector2(32f, 57f);
			if (this.size > 1f)
			{
				this.size -= Game1.FrameTime * 6f;
				if (this.size < 1f)
				{
					this.size = 1f;
				}
			}
			if (this.scaleY < 1f)
			{
				this.scaleY += Game1.FrameTime * 6f;
				if (this.scaleY > 1f)
				{
					this.scaleY = 1f;
				}
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			switch (base.flag)
			{
			case 1:
			{
				int x = (this.stopped ? (576 + this.animFrame * 32) : (192 + this.animFrame * 32));
				SpriteEffects effects = (((int)this.groundOffset % 2 != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
				sprite.Draw(particlesTex[1], (base.location + new Vector2(0f, (int)this.groundOffset)) * Game1.worldScale - Game1.Scroll, new Rectangle(x, 128, 32, 32), new Color(1f, 1f, 1f, this.frame), this.rotation, new Vector2(16f, 32f), 1.2f * worldScale, effects, 0f);
				return;
			}
			case 5:
			{
				int y;
				Vector2 position;
				if (!this.stopped)
				{
					y = 64;
					position = (base.location + new Vector2(0f, -64f)) * Game1.worldScale - Game1.Scroll;
				}
				else
				{
					y = 0;
					position = (base.location + new Vector2(0f, (int)this.groundOffset)) * Game1.worldScale - Game1.Scroll;
				}
				sprite.Draw(particlesTex[1], position, new Rectangle(102, y, 64, 64), new Color(1f, 1f, 1f, this.frame), this.rotation, this.origin, new Vector2(this.size, this.scaleY) * 1.3f * worldScale, SpriteEffects.None, 0f);
				return;
			}
			}
			Rectangle value = (this.stopped ? new Rectangle(2340 + this.animFrame * 64 - (int)this.animFrame / 24 * 24 * 64, 1000 + (int)this.animFrame / 24 * 64, 64, 64) : new Rectangle(2340 + this.animFrame * 64, 1128, 64, 64));
			Color color;
			switch (base.flag)
			{
			default:
				this.size = 0.8f;
				color = new Color(0.5f, 0.5f, 1f, this.frame);
				break;
			case 100:
				this.size = 1f;
				color = new Color(0.6f, 1f, 0.6f, this.frame);
				break;
			case 400:
				this.size = 1.2f;
				color = new Color(1f, 0.5f, 0.5f, this.frame);
				break;
			}
			SpriteEffects effects2 = (((int)this.groundOffset % 2 != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			sprite.Draw(particlesTex[2], (base.location + new Vector2(0f, (int)this.groundOffset)) * Game1.worldScale - Game1.Scroll, value, color, this.rotation, new Vector2(32f, 64f), this.size * worldScale, effects2, 0f);
		}
	}
}
